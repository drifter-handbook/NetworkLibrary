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
        // network
        if (IsHost)
        {
            host = gameObject.AddComponent<NetworkHost>();
            host.Initialize();
            matchmakingHost = gameObject.AddComponent<MatchmakingHost>();
        }
        else
        {
            client = gameObject.AddComponent<NetworkClient>();
            client.Initialize();
            matchmakingClient = gameObject.AddComponent<MatchmakingClient>();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}