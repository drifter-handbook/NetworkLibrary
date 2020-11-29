using LiteNetLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncInput : MonoBehaviour, ISyncClient
{
    NetworkSyncToHost syncToHost;

    // Start is called before the first frame update
    void Start()
    {
        syncToHost = GetComponent<NetworkSyncToHost>();
    }

    // Update is called once per frame
    void Update()
    {
        syncToHost["input"] = NetworkPlayers.GetInput();
    }
}
