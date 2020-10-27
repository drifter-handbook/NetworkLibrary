using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetClient
{
    private const int ServerPort = 6996;
    private const string ConnectionKey = "test_key";

    public NetManager net;
    EventBasedNetListener netEvent = new EventBasedNetListener();
    EventBasedNatPunchListener natPunchEvent = new EventBasedNatPunchListener();

    public NetClient()
    {
        netEvent.PeerConnectedEvent += peer =>
        {
            Console.WriteLine("PeerConnected: " + peer.EndPoint);
            NetDataWriter writer = new NetDataWriter();
            writer.Put("Hello, fellow client!");
            peer.Send(writer, DeliveryMethod.ReliableUnordered);
        };

        netEvent.ConnectionRequestEvent += request =>
        {
            request.AcceptIfKey(ConnectionKey);
        };

        netEvent.PeerDisconnectedEvent += (peer, disconnectInfo) =>
        {
            Console.WriteLine("PeerDisconnected: " + disconnectInfo.Reason);
            if (disconnectInfo.AdditionalData.AvailableBytes > 0)
            {
                Console.WriteLine("Disconnect data: " + disconnectInfo.AdditionalData.GetInt());
            }
        };

        netEvent.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod) =>
        {
            Console.WriteLine("We got: {0}", dataReader.GetString(1000));
            dataReader.Recycle();
        };

        natPunchEvent.NatIntroductionSuccess += (point, addrType, token) =>
        {
            var peer = net.Connect(point, ConnectionKey);
            Console.WriteLine($"NatIntroductionSuccess. Connecting to other client: {point}, type: {addrType}, connection created: {peer != null}");
        };

        net = new NetManager(netEvent)
        {
            IPv6Enabled = IPv6Mode.Disabled,
            NatPunchEnabled = true
        };
        net.NatPunchModule.Init(natPunchEvent);
        net.Start();

        net.NatPunchModule.SendNatIntroduceRequest("75.134.27.221", ServerPort, "token1");
    }
}

public class NetworkHandler : MonoBehaviour
{
    NetClient client;
    // Start is called before the first frame update
    void Start()
    {
        client = new NetClient();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            Console.WriteLine("C1 stopped");
            client.net.DisconnectPeer(client.net.FirstPeer, new byte[] { 1, 2, 3, 4 });
            client.net.Stop();
        }
        client.net.NatPunchModule.PollEvents();
        client.net.PollEvents();
    }

    private void OnApplicationQuit()
    {
        client.net.Stop();
    }
}
