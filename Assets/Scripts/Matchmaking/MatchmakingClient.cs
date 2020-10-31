using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MatchmakingClient : MonoBehaviour
{
    [NonSerialized]
    public string JoinRoom = null;

    NetworkClient client => GameController.Instance.client;

    [NonSerialized]
    public List<MatchmakingRoomEntry> Rooms;

    void Start()
    {
        StartCoroutine(PollMatchmakingServer());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator PollMatchmakingServer()
    {
        string server = $"http://{GameController.Instance.MatchmakingServer.Address.ToString()}:{GameController.Instance.MatchmakingServer.Port}";
        UnityWebRequest www = null;
        // get rooms
        while (JoinRoom == null)
        {
            www = UnityWebRequest.Get($"{server}/rooms/{GameController.Instance.Username}/1");
            yield return www.SendWebRequest();
            List<MatchmakingRoomEntry> roomEntries = null;
            if (www.isNetworkError || www.isHttpError)
            {
                throw new UnityException(www.error);
            }
            else
            {
                roomEntries = JsonConvert.DeserializeObject<List<MatchmakingRoomEntry>>(www.downloadHandler.text);
                if (roomEntries != null)
                {
                    Rooms = roomEntries;
                    foreach (MatchmakingRoomEntry room in Rooms)
                    {
                        Debug.Log($"Room entry: {room.name}: {room.room_code}, {room.users}/5");
                    }
                }
            }
            // refresh every so often for new rooms
            yield return new WaitForSeconds(7.5f);
        }
        // join room
        www = UnityWebRequest.Post($"{server}/join/{JoinRoom}/{GameController.Instance.Username}", "");
        yield return www.SendWebRequest();
        MatchmakingJoinResponse joinResponse = null;
        if (www.isNetworkError || www.isHttpError)
        {
            // TODO: room has closed error or something, or host left
            throw new UnityException(www.error);
        }
        else
        {
            joinResponse = JsonConvert.DeserializeObject<MatchmakingJoinResponse>(www.downloadHandler.text);
            if (joinResponse != null)
            {
                // connect to new clients
                client.ConnectionKey = joinResponse.connection_key;
                client.netManager.NatPunchModule.SendNatIntroduceRequest(GameController.Instance.NatPunchServer, joinResponse.connect);
            }
        }
        // keep room open for next play
        while (client.GameStarted)
        {
            yield return www.SendWebRequest();
            MatchmakingRefreshResponse refreshResponse = null;
            if (www.isNetworkError || www.isHttpError)
            {
                // TODO: room closed
                throw new UnityException(www.error);
            }
            else
            {
                refreshResponse = JsonConvert.DeserializeObject<MatchmakingRefreshResponse>(www.downloadHandler.text);
                if (refreshResponse.expired)
                {
                    throw new UnityException("Lost spot in matchmaking server.");
                }
            }
            // refresh only enough to keep our spot
            yield return new WaitForSeconds(15f);
        }
    }
}

public class MatchmakingRoomEntry
{
    public string name;
    public string room_code;
    public int users;
}

public class MatchmakingJoinResponse
{
    public string user_id;
    public string connection_key;
    public string connect;
}