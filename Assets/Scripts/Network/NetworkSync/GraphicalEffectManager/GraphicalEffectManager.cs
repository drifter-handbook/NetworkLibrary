using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GraphicalEffectType
{

}

public class GraphicalEffectManager : MonoBehaviour, INetworkMessageReceiver
{
    public static GraphicalEffectManager Instance => GameObject.FindGameObjectWithTag("GraphicalEffectManager").GetComponent<GraphicalEffectManager>();

    NetworkSync sync;

    // Start is called before the first frame update
    void Start()
    {
        sync = GetComponent<NetworkSync>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // add methods that spawn a graphical effect and send a message to clients if host.

    public void ReceiveNetworkMessage(NetworkMessage message)
    {
        if (!GameController.Instance.IsHost)
        {
            GraphicalEffectPacket effect = NetworkUtils.GetNetworkData<GraphicalEffectPacket>(message.contents);
            if (effect != null)
            {
                switch ((GraphicalEffectType)effect.effect)
                {
                    // spawn graphical effect for client
                    default:
                        break;
                }
            }
        }
    }
}

public class GraphicalEffectPacket : INetworkData
{
    public string Type { get; set; }
    public int effect;
    public int mode;
    public SyncableVector3 pos;
    public float angle;
    public SyncableVector2 scale;
}