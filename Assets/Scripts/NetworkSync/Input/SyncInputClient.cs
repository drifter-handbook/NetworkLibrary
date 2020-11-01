using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncInputClient : MonoBehaviour, ISyncClient
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
