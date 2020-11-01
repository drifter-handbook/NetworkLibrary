using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncTransformClient : NetworkMonoBehaviour, ISyncClient
{
    NetworkSync sync;

    // Start is called before the first frame update
    protected override void NetworkStart()
    {
        sync = GetComponent<NetworkSync>();
        Update();
    }

    // Update is called once per frame
    protected override void NetworkUpdate()
    {
        SyncableTransform2D netTransform = NetworkUtils.GetNetworkData<SyncableTransform2D>(sync["transform"]);
        if (netTransform != null)
        {
            transform.position = netTransform.position.ToVector3();
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, netTransform.rotation);
            Vector2 netScale = netTransform.scale.ToVector2();
            transform.localScale = new Vector3(netScale.x, netScale.y, transform.localScale.z);
        }
    }
}
