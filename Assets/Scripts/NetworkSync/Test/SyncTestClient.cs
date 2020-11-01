﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncTestClient : NetworkMonoBehaviour, ISyncClient, INetworkMessageReceiver
{
    NetworkSync sync;
    NetworkSyncToHost syncToHost;

    // Start is called before the first frame update
    protected override void NetworkStart()
    {
        sync = GetComponent<NetworkSync>();
        syncToHost = GetComponent<NetworkSyncToHost>();
        syncToHost["test2"] = "I am the client.";
        sync.SendNetworkMessage("Message");
    }

    // Update is called once per frame
    protected override void NetworkUpdate()
    {
        Debug.Log($"Test data from host: {sync["test"] as string}");
    }

    public void ReceiveNetworkMessage(NetworkMessage message)
    {
        Debug.Log($"Received message from host: {message.contents as string}");
    }
}
