using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncTransformHost : NetworkMonoBehaviour, ISyncHost
{
    NetworkSync sync;

    // Start is called before the first frame update
    protected override void NetworkStart()
    {
        sync = GetComponent<NetworkSync>();
        NetworkUpdate();
    }

    // Update is called once per frame
    protected override void NetworkUpdate()
    {
        sync["transform"] = new SyncableTransform2D()
        {
            position = new SyncableVector3(transform.position),
            rotation = transform.eulerAngles.z,
            scale = new SyncableVector2(transform.localScale)
        };
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

public class SyncableTransform2D
{
    public SyncableVector3 position;
    public float rotation;
    public SyncableVector2 scale;
}