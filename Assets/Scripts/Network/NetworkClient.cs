using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkClient : MonoBehaviour, ISyncClient, INetworkMessageReceiver
{
    public NetManager netManager;
    EventBasedNetListener netEvent = new EventBasedNetListener();
    EventBasedNatPunchListener natPunchEvent = new EventBasedNatPunchListener();
    public NetPacketProcessor netPacketProcessor = new NetPacketProcessor();

    // host -> client
    public NetworkObjectData data = new NetworkObjectData();
    public NetworkObjectData dataToHost = new NetworkObjectData();
    public NetworkMessages messages = new NetworkMessages();

    [NonSerialized]
    public NetworkObjects networkObjects;

    public void Initialize()
    {
        networkObjects = GetComponent<NetworkObjects>();
        // network handlers
        natPunchEvent.NatIntroductionSuccess += (point, addrType, token) =>
        {
            var peer = netManager.Connect(point, GameController.Instance.RoomCode);
            Debug.Log($"NatIntroductionSuccess. Connecting to peer: {point}, type: {addrType}, connection created: {peer != null}");
        };
        netEvent.PeerConnectedEvent += peer => { Debug.Log("PeerConnected: " + peer.EndPoint); };
        netEvent.ConnectionRequestEvent += request => { request.AcceptIfKey(GameController.Instance.RoomCode); };
        netEvent.NetworkReceiveEvent += (peer, reader, deliveryMethod) => {
            netPacketProcessor.ReadAllPackets(reader, peer);
        };
        netEvent.PeerDisconnectedEvent += (peer, disconnectInfo) => { Debug.Log($"Peer {peer} Disconnected: {disconnectInfo.Reason}"); };
        // packet handlers
        netPacketProcessor.SubscribeReusable<NetworkMessagePacket, NetPeer>((packet, peer) =>
        {
            messages.SyncFromPacket(packet);
        });
        netPacketProcessor.SubscribeReusable<NetworkObjectDataPacket, NetPeer>((packet, peer) =>
        {
            data.SyncFromPacket(packet);
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
        LoadObjectsInNewScene(0);
    }

    // update from network
    void FixedUpdate()
    {
        netManager.PollEvents();
        // send data packets
        netManager.SendToAll(netPacketProcessor.Write(dataToHost.ToPacket()), DeliveryMethod.Sequenced);
        // cleanup
        messages.Update();
    }

    public void ReceiveNetworkMessage(NetworkMessage message)
    {
        SceneChangePacket sceneChange = NetworkUtils.Convert<SceneChangePacket>(message.contents);
        if (sceneChange != null)
        {
            Debug.Log($"Scene change received: {sceneChange.scene}: {sceneChange.startingObjectID}.");
            SetScene(sceneChange.scene, sceneChange.startingObjectID);
        }
        CreateNetworkObjectPacket createObject = NetworkUtils.Convert<CreateNetworkObjectPacket>(message.contents);
        if (createObject != null)
        {
            networkObjects.CreateNetworkObject(createObject.objectID, createObject.networkType);
        }
        DestroyNetworkObjectPacket destroyObject = NetworkUtils.Convert<DestroyNetworkObjectPacket>(message.contents);
        if (destroyObject != null)
        {
            networkObjects.DestroyNetworkObject(destroyObject.objectID);
        }
    }

    public void SetScene(string scene, int sceneStartingObjectID)
    {
        StartCoroutine(SetSceneCoroutine(scene, sceneStartingObjectID));
    }
    // coroutine for loading a scene
    IEnumerator SetSceneCoroutine(string scene, int sceneStartingObjectID)
    {
        yield return SceneManager.LoadSceneAsync(scene);
        LoadObjectsInNewScene(sceneStartingObjectID);
    }
    // when scene loads, init all starting network objects
    void LoadObjectsInNewScene(int sceneStartingObjectID)
    {
        List<GameObject> startingEntities =
            GameObject.FindGameObjectWithTag("NetworkStartingEntities").GetComponent<NetworkStartingEntities>().startingEntities;
        for (int i = 0; i < startingEntities.Count; i++)
        {
            NetworkObjects.RemoveIncorrectComponents(startingEntities[i]);
            NetworkSync sync = startingEntities[i].GetComponent<NetworkSync>();
            if (string.IsNullOrWhiteSpace(sync.NetworkType))
            {
                throw new InvalidTypeException($"NetworkType field is empty in starting network object {startingEntities[i].name}");
            }
            sync.Initialize(sceneStartingObjectID + i, sync.NetworkType);
        }
    }

    void OnApplicationQuit()
    {
        netManager.Stop();
    }
}
