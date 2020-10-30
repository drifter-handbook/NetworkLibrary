using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkHost : MonoBehaviour, ISyncHost
{
    public NetManager netManager;
    EventBasedNetListener netEvent = new EventBasedNetListener();
    EventBasedNatPunchListener natPunchEvent = new EventBasedNatPunchListener();
    public NetPacketProcessor netPacketProcessor = new NetPacketProcessor();

    public NetworkObjectData data = new NetworkObjectData();
    // clients -> host
    public Dictionary<int, NetworkObjectData> clientData = new Dictionary<int, NetworkObjectData>();
    public Dictionary<int, NetworkMessages> clientMessages = new Dictionary<int, NetworkMessages>();

    static int currentObjectID = 0;
    public static int NextObjectID { get { return currentObjectID++; } }

    // peers
    [NonSerialized]
    public List<int> Peers = new List<int>();

    [NonSerialized]
    public NetworkObjects networkObjects;

    public void Initialize()
    {
        networkObjects = GetComponent<NetworkObjects>();
        // network handlers
        natPunchEvent.NatIntroductionSuccess += (point, addrType, token) =>
        {
            var peer = netManager.Connect(point, GameController.Instance.RoomCode);
            Debug.Log($"NatIntroductionSuccess. Connecting to client: {point}, type: {addrType}, connection created: {peer != null}");
        };
        netEvent.PeerConnectedEvent += peer => {
            Peers.Add(peer.Id);
            Debug.Log("PeerConnected: " + peer.EndPoint);
        };
        netEvent.ConnectionRequestEvent += request => { request.AcceptIfKey(GameController.Instance.RoomCode); };
        netEvent.NetworkReceiveEvent += (peer, reader, deliveryMethod) => {
            netPacketProcessor.ReadAllPackets(reader, peer);
        };
        netEvent.PeerDisconnectedEvent += (peer, disconnectInfo) => { Debug.Log($"Peer {peer} Disconnected: {disconnectInfo.Reason}"); };
        // packet handlers
        netPacketProcessor.SubscribeReusable<NetworkMessagePacket, NetPeer>((packet, peer) =>
        {
            if (!clientMessages.ContainsKey(peer.Id))
            {
                clientMessages[peer.Id] = new NetworkMessages();
            }
            clientMessages[peer.Id].SyncFromPacket(packet);
        });
        netPacketProcessor.SubscribeReusable<NetworkObjectDataPacket, NetPeer>((packet, peer) =>
        {
            if (!clientData.ContainsKey(peer.Id))
            {
                clientData[peer.Id] = new NetworkObjectData();
            }
            clientData[peer.Id].SyncFromPacket(packet);
        });
        // connect
        netManager = new NetManager(netEvent)
        {
            IPv6Enabled = IPv6Mode.Disabled,
            NatPunchEnabled = true
        };
        netManager.NatPunchModule.Init(natPunchEvent);
        netManager.Start();
        netManager.NatPunchModule.SendNatIntroduceRequest(GameController.Instance.NatPunchServer.Address.ToString(),
            GameController.Instance.NatPunchServer.Port, GameController.Instance.NatPunchCode);
        LoadObjectsInNewScene();
    }

    // update from network
    void FixedUpdate()
    {
        netManager.PollEvents();
        // send data packets
        netManager.SendToAll(netPacketProcessor.Write(data.ToPacket()), DeliveryMethod.Sequenced);
        // cleanup
        foreach (int peerID in clientMessages.Keys)
        {
            clientMessages[peerID].Update();
        }
    }

    public void SetScene(string scene)
    {
        StartCoroutine(SetSceneCoroutine(scene));
    }
    // coroutine for loading a scene
    IEnumerator SetSceneCoroutine(string scene)
    {
        yield return SceneManager.LoadSceneAsync(scene);
        // send scene change event to clients
        NetworkUtils.SendNetworkMessage(new SceneChangePacket()
        {
            scene = scene,
            startingObjectID = currentObjectID
        }, DeliveryMethod.ReliableOrdered);
        LoadObjectsInNewScene();
    }
    // when scene loads, init all starting network objects
    void LoadObjectsInNewScene()
    {
        List<GameObject> startingEntities =
            GameObject.FindGameObjectWithTag("NetworkStartingEntities").GetComponent<NetworkStartingEntities>().startingEntities;
        foreach (GameObject obj in startingEntities)
        {
            NetworkObjects.RemoveIncorrectComponents(obj);
            NetworkSync sync = obj.GetComponent<NetworkSync>();
            sync.Initialize(NextObjectID, sync.NetworkType);
        }
    }

    void OnApplicationQuit()
    {
        netManager.Stop();
    }
}

public class SceneChangePacket
{
    public string scene;
    public int startingObjectID;
}

public class CreateNetworkObjectPacket
{
    public int objectID;
    public string networkType;
}

public class DestroyNetworkObjectPacket
{
    public int objectID;
}
