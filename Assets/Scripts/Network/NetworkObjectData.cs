using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using UnityEngine;

public class NetworkObjectData
{
    public Dictionary<int, Dictionary<string, object>> data = new Dictionary<int, Dictionary<string, object>>();

    public Dictionary<string, object> GetData(int objectID)
    {
        if (!data.ContainsKey(objectID))
        {
            data[objectID] = new Dictionary<string, object>();
        }
        return data[objectID];
    }
    public void DestroyData(int objectID)
    {
        data.Remove(objectID);
    }

    public void SyncFromPacket(NetworkObjectDataPacket packet)
    {
        Dictionary<int, Dictionary<string, object>> newData =
            JsonUtility.FromJson<Dictionary<int, Dictionary<string, object>>>(NetworkUtils.Decompress(packet.data));
        // sync keys
        foreach (int objectID in data.Keys)
        {
            foreach (string field in data[objectID].Keys)
            {
                if (newData.ContainsKey(objectID) && newData[objectID].ContainsKey(field))
                {
                    data[objectID][field] = newData[objectID][field];
                }
            }
        }
    }

    public NetworkObjectDataPacket ToPacket()
    {
        return new NetworkObjectDataPacket() { data = NetworkUtils.Compress(JsonUtility.ToJson(data)) };
    }
}

public class NetworkObjectDataPacket
{
    public byte[] data { get; set; }
}