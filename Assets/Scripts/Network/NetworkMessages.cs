using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NetworkMessages
{
    const float MESSAGE_TIMEOUT = 30f;
    float latestCleanup = -1;
    
    class Message
    {
        public dynamic message;
        public float timestamp;
    }

    Dictionary<int, List<Message>> messages = new Dictionary<int, List<Message>>();

    public List<dynamic> PopMessages(int objectID)
    {
        if (!messages.ContainsKey(objectID))
        {
            messages[objectID] = new List<Message>();
        }
        List<dynamic> contents = messages[objectID].Select(x => x.message).ToList();
        messages[objectID].Clear();
        return contents;
    }

    public void SyncFromPacket(NetworkMessagePacket packet)
    {
        dynamic message = JsonConvert.DeserializeObject<dynamic>(NetworkUtils.Decompress(packet.data));
        if (!messages.ContainsKey(packet.objectID))
        {
            messages[packet.objectID] = new List<Message>();
        }
        messages[packet.objectID].Add(new Message() { message = message, timestamp = Time.deltaTime });
    }

    public static NetworkMessagePacket ToPacket(int objectID, object obj)
    {
        return new NetworkMessagePacket() { objectID = objectID, data = NetworkUtils.Compress(JsonConvert.SerializeObject(obj)) };
    }

    public void Update()
    {
        if (latestCleanup < 0)
        {
            latestCleanup = Time.deltaTime;
        }
        if (Time.deltaTime - latestCleanup > MESSAGE_TIMEOUT)
        {
            latestCleanup = Time.deltaTime;
            // clean up old events
            foreach (int objectID in messages.Keys)
            {
                for (int i = 0; i < messages[objectID].Count; i++)
                {
                    if (Time.deltaTime - messages[objectID][i].timestamp > MESSAGE_TIMEOUT)
                    {
                        messages[objectID].RemoveAt(i);
                        i--;
                    }
                }
            }
        }
    }
}

public class NetworkMessage
{
    public dynamic contents;
    public int peerId;
}

public class NetworkMessagePacket
{
    public int objectID { get; set; }
    public byte[] data { get; set; }
}