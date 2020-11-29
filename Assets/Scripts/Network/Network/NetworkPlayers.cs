using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayers : MonoBehaviour, ISyncHost
{
    NetworkSyncToHost syncFromClients;

    public List<GameObject> spawnPoints;

    GameObject hostPlayer;
    Dictionary<int, GameObject> clientPlayers = new Dictionary<int, GameObject>();

    [NonSerialized]
    public Dictionary<int, GameObject> players = new Dictionary<int, GameObject>();

    public static NetworkPlayers Instance => GameObject.FindGameObjectWithTag("NetworkPlayers")?.GetComponent<NetworkPlayers>();

    // Start is called before the first frame update
    void Start()
    {
        syncFromClients = GetComponent<NetworkSyncToHost>();
        // create host
        int playerNum = 0;
        hostPlayer = CreatePlayer(-1, ref playerNum);
        // create other players
        foreach (int peerID in GameController.Instance.host.Peers)
        {
            clientPlayers[peerID] = CreatePlayer(peerID, ref playerNum);
        }
    }

    // Update is called once per frame
    void Update()
    {
        PlayerInputData input = GetInput();
        UpdateInput(hostPlayer, input);
        foreach (int peerID in GameController.Instance.host.Peers)
        {
            input = NetworkUtils.GetNetworkData<PlayerInputData>(syncFromClients["input", peerID]);
            if (input != null)
            {
                UpdateInput(clientPlayers[peerID], input);
            }
        }
    }

    GameObject CreatePlayer(int peerID, ref int i)
    {
        GameObject obj = GameController.Instance.host.CreateNetworkObject("Player",
            spawnPoints[i % spawnPoints.Count].transform.position, Quaternion.identity);
        i++;
        players[peerID] = obj;
        return obj;
    }

    public static void UpdateInput(GameObject player, PlayerInputData input)
    {
        if (player == null)
        {
            return;
        }
        player.GetComponent<PlayerMovement>().UpdateInput(input);
    }

    public static PlayerInputData GetInput()
    {
        PlayerInputData input = new PlayerInputData();
        // get player input

        //controller movement input
        if (Input.GetJoystickNames().Length > 0)
        {
            input.MoveX = Input.GetAxis("Horizontal");
            input.MoveY = Input.GetAxis("Vertical");
        }

        //keyboard movement input
        else
        {
            input.MoveX = 0;
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                input.MoveX--;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                input.MoveX++;
            }
            input.MoveY = 0;
            if (Input.GetKey(KeyCode.DownArrow))
            {
                // down key does nothing
                input.MoveY--;
            }
            if (Input.GetKey(KeyCode.UpArrow))
            {
                input.MoveY++;
            }
        }

        input.Jump = Input.GetKey(KeyCode.Space);
        input.Light = Input.GetKey(KeyCode.Q);
        input.Special = Input.GetKey(KeyCode.W);
        input.Grab = Input.GetKey(KeyCode.E);
        input.Guard = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        return input;
    }
}


[Serializable]
public class PlayerInputData : INetworkData, ICloneable
{
    public string Type { get; set; }
    public float MoveX;
    public float MoveY;
    public bool Jump;
    public bool Light;
    public bool Special;
    public bool Grab;
    public bool Guard;

    public object Clone()
    {
        return new PlayerInputData()
        {
            Type = Type,
            MoveX = MoveX,
            MoveY = MoveY,
            Jump = Jump,
            Light = Light,
            Special = Special,
            Grab = Grab,
            Guard = Guard
        };
    }

    public void CopyFrom(PlayerInputData data)
    {
        Type = data.Type;
        MoveX = data.MoveX;
        MoveY = data.MoveY;
        Jump = data.Jump;
        Light = data.Light;
        Special = data.Special;
        Grab = data.Grab;
        Guard = data.Guard;
    }
}