using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

[DisallowMultipleComponent]
public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    public bool IsHost;

    [NonSerialized]
    public NetworkClient client;
    [NonSerialized]
    public NetworkHost host;

    [NonSerialized]
    public IPEndPoint NatPunchServer = new IPEndPoint(IPAddress.Parse("75.134.27.221"), 6996);

    public string RoomCode = "Pascal'sPennePasta";

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
        }
        else
        {
            client = gameObject.AddComponent<NetworkClient>();
            client.Initialize();
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