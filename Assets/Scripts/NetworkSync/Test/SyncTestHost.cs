using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncTestHost : MonoBehaviour, ISyncHost
{
    NetworkSync sync;
    NetworkSyncToHost syncFromClients;

    // Start is called before the first frame update
    void Start()
    {
        sync = GetComponent<NetworkSync>();
        syncFromClients = GetComponent<NetworkSyncToHost>();
        sync["test"] = "Test data.";
    }

    // Update is called once per frame
    void Update()
    {
        foreach (int peerID in GameController.Instance.host.Peers)
        {
            Debug.Log($"Test data from host: {NetworkUtils.Convert<string>(syncFromClients["test2", peerID])}");
        }
    }

    public void ReceiveNetworkMessage(NetworkMessage message)
    {
        Debug.Log($"Received message from host: {NetworkUtils.Convert<string>(message.contents)}");
    }
}
