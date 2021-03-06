﻿using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MatchmakingUI : MonoBehaviour
{
    string roomCode = "";
    public InputField clientRoomCode;
    public GameObject hostButton;
    public GameObject startButton;
    public GameObject clientMenu;

    Coroutine getRoomsCoroutine;

    public static MatchmakingUI Instance { get; set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        getRoomsCoroutine = StartCoroutine(GetRoomsCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator GetRoomsCoroutine()
    {
        string server = $"http://{GameController.Instance.MatchmakingServer.Address.ToString()}:{GameController.Instance.MatchmakingServer.Port}";
        // get rooms
        while (true)
        {
            UnityWebRequest www = UnityWebRequest.Get($"{server}/rooms/{GameController.Instance.Username}/1");
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
                    foreach (MatchmakingRoomEntry room in roomEntries)
                    {
                        Debug.Log($"[Room Entry] {room.name}: {room.room_code}, {room.users}/8");
                    }
                }
            }
            // refresh every so often for new rooms
            yield return new WaitForSeconds(5f);
        }
    }

    public void SetRoomCode()
    {
        roomCode = clientRoomCode.text;
    }

    public void HostGame()
    {
        GameController.Instance.IsHost = true;
        GameController.Instance.StartNetworkHost();
        StopCoroutine(getRoomsCoroutine);
        hostButton.SetActive(false);
        startButton.SetActive(true);
        clientMenu.SetActive(false);
    }

    public void StartHost()
    {
        GameController.Instance.host.SetScene("GameWorld");
    }

    public void StartClient()
    {
        SetRoomCode();
        GameController.Instance.StartNetworkClient(roomCode);
        hostButton.SetActive(false);
        startButton.SetActive(false);
        clientMenu.SetActive(false);
    }

    public void ResetUI()
    {
        hostButton.SetActive(true);
        startButton.SetActive(false);
        clientMenu.SetActive(true);
    }
}

public class MatchmakingRoomEntry
{
    public string name;
    public string room_code;
    public int users;
}
