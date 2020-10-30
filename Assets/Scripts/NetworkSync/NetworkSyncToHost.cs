﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkSyncToHost : MonoBehaviour
{
    NetworkSync networkSync;

    public object this[string key] {
        get {
            if (GameController.Instance.IsHost) { throw new InvalidOperationException("Invalid operation as host."); }
            return NetworkUtils.GetNetworkDataToHost(networkSync.ObjectID)[key];
        }
        set {
            if (GameController.Instance.IsHost) { throw new InvalidOperationException("Invalid operation as host."); }
            NetworkUtils.GetNetworkDataToHost(networkSync.ObjectID)[key] = value;
        }
    }
    public object this[string key, int peerId]
    {
        get {
            if (!GameController.Instance.IsHost) { throw new InvalidOperationException("Invalid operation as client."); }
            return NetworkUtils.GetNetworkDataFromClient(networkSync.ObjectID, peerId)[key];
        }
        set {
            if (!GameController.Instance.IsHost) { throw new InvalidOperationException("Invalid operation as client."); }
            NetworkUtils.GetNetworkDataFromClient(networkSync.ObjectID, peerId)[key] = value;
        }
    }

    public void Awake()
    {
        networkSync = GetComponent<NetworkSync>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}

