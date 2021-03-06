﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncTransform : MonoBehaviour
{
    NetworkSync sync;

    SyncableTransform2D netTransform
    {
        get { return NetworkUtils.GetNetworkData<SyncableTransform2D>(sync["transform"]); }
        set { sync["transform"] = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        sync = GetComponent<NetworkSync>();
        Update();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.Instance.IsHost)
        {
            netTransform = new SyncableTransform2D()
            {
                position = new SyncableVector3(transform.position),
                rotation = transform.eulerAngles.z,
                scale = new SyncableVector2(transform.localScale)
            };
        }
        else if (netTransform != null)
        {
            transform.position = netTransform.position.ToVector3();
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, netTransform.rotation);
            Vector2 netScale = netTransform.scale.ToVector2();
            transform.localScale = new Vector3(netScale.x, netScale.y, transform.localScale.z);
        }
    }
}

public class SyncableVector2
{
    public float x = 0;
    public float y = 0;
    public SyncableVector2(Vector2 v)
    {
        x = v.x;
        y = v.y;
    }
    public Vector2 ToVector2()
    {
        return new Vector2(x, y);
    }
}

public class SyncableVector3
{
    public float x = 0;
    public float y = 0;
    public float z = 0;
    public SyncableVector3(Vector3 v)
    {
        x = v.x;
        y = v.y;
        z = v.z;
    }
    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
}

public class SyncableTransform2D : INetworkData
{
    public string Type { get; set; }
    public SyncableVector3 position;
    public float rotation;
    public SyncableVector2 scale;
}