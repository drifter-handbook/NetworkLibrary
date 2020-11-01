using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

[DisallowMultipleComponent]
public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    public bool IsHost;

    public string Username = "";

    [NonSerialized]
    public NetworkClient client;
    [NonSerialized]
    public MatchmakingClient matchmakingClient;
    [NonSerialized]
    public NetworkHost host;
    [NonSerialized]
    public MatchmakingHost matchmakingHost;

    [NonSerialized]
    public IPEndPoint NatPunchServer = new IPEndPoint(IPAddress.Parse("75.134.27.221"), 6996);
    [NonSerialized]
    public IPEndPoint MatchmakingServer = new IPEndPoint(IPAddress.Parse("75.134.27.221"), 6997);

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
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void StartNetworkHost()
    {
        host = gameObject.AddComponent<NetworkHost>();
        NetworkSync sync = GetComponent<NetworkSync>() ?? gameObject.AddComponent<NetworkSync>();
        sync.NetworkType = "GameController";
        sync.ObjectID = 0;
        host.Initialize();
        matchmakingHost = GetComponent<MatchmakingHost>() ?? gameObject.AddComponent<MatchmakingHost>();
    }
    public void StartNetworkClient(string roomCode)
    {
        client = gameObject.AddComponent<NetworkClient>();
        NetworkSync sync = GetComponent<NetworkSync>() ?? gameObject.AddComponent<NetworkSync>();
        sync.NetworkType = "GameController";
        sync.ObjectID = 0;
        client.Initialize();
        matchmakingClient = GetComponent<MatchmakingClient>() ?? gameObject.AddComponent<MatchmakingClient>();
        matchmakingClient.JoinRoom = roomCode;
    }
    public void CleanupNetwork()
    {
        if (IsHost)
        {
            Destroy(host);
        }
        else
        {
            Destroy(client);
        }
        IsHost = false;
    }
}