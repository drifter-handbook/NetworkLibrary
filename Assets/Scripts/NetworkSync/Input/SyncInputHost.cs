using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncInputHost : NetworkMonoBehaviour, ISyncHost
{
    NetworkSyncToHost syncFromClients;

    // Start is called before the first frame update
    protected override void NetworkStart()
    {
        syncFromClients = GetComponent<NetworkSyncToHost>();
    }

    // Update is called once per frame
    protected override void NetworkUpdate()
    {

    }
}
