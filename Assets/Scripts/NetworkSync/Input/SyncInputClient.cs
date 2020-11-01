using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncInputClient : NetworkMonoBehaviour, ISyncClient
{
    NetworkSyncToHost syncToHost;

    // Start is called before the first frame update
    protected override void NetworkStart()
    {
        syncToHost = GetComponent<NetworkSyncToHost>();
    }

    // Update is called once per frame
    protected override void NetworkUpdate()
    {
        int X = 0;
        if (Input.GetKey(KeyCode.D))
        {
            X++;
        }
        if (Input.GetKey(KeyCode.A))
        {
            X--;
        }
        int Y = 0;
        if (Input.GetKey(KeyCode.W))
        {
            Y++;
        }
        if (Input.GetKey(KeyCode.S))
        {
            Y--;
        }
        syncToHost["X"] = X;
        syncToHost["Y"] = Y;
    }
}
