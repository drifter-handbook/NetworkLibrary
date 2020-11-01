using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayers : MonoBehaviour, ISyncHost
{
    NetworkSyncToHost syncFromClients;

    public GameObject playerPrefab;

    GameObject hostPlayer;
    Dictionary<int, GameObject> clientPlayers = new Dictionary<int, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        syncFromClients = GetComponent<NetworkSyncToHost>();
        // create host
        hostPlayer = GameController.Instance.host.CreateNetworkObject("Player");
        hostPlayer.transform.position = Vector3.zero;
        // create other players
        foreach (int peerID in GameController.Instance.host.Peers)
        {
            clientPlayers[peerID] = GameController.Instance.host.CreateNetworkObject("Player");
            clientPlayers[peerID].transform.position = Vector3.right * clientPlayers.Count;
        }
    }

    // Update is called once per frame
    void Update()
    {
        int X = 0;
        if (Input.GetKey(KeyCode.D))
        {
            X++;
        }
        if (Input.GetKey(KeyCode.A))
        {
            X--;
        }
        int Y = 0;
        if (Input.GetKey(KeyCode.W))
        {
            Y++;
        }
        if (Input.GetKey(KeyCode.S))
        {
            Y--;
        }
        hostPlayer.GetComponent<PlayerMovement>().InputX = X;
        hostPlayer.GetComponent<PlayerMovement>().InputY = Y;
        foreach (int peerID in GameController.Instance.host.Peers)
        {
            if (syncFromClients["X", peerID] as object != null)
            {
                clientPlayers[peerID].GetComponent<PlayerMovement>().InputX = (int)syncFromClients["X", peerID];
                clientPlayers[peerID].GetComponent<PlayerMovement>().InputY = (int)syncFromClients["Y", peerID];
            }
        }
    }
}
