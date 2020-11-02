using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayers : MonoBehaviour, ISyncHost, INetworkMessageReceiver
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            hostPlayer.GetComponent<SyncAnimatorHost>().SetTrigger("Red");
        }
        foreach (int peerID in GameController.Instance.host.Peers)
        {
            if (syncFromClients["X", peerID] as object != null)
            {
                clientPlayers[peerID].GetComponent<PlayerMovement>().InputX = int.Parse(syncFromClients["X", peerID].ToString());
                clientPlayers[peerID].GetComponent<PlayerMovement>().InputY = int.Parse(syncFromClients["Y", peerID].ToString());
            }
        }
    }

    public void ReceiveNetworkMessage(NetworkMessage message)
    {
        SyncPlayerActionMessage action = NetworkUtils.GetNetworkData<SyncPlayerActionMessage>(message.contents);
        if (action != null)
        {
            if (clientPlayers.ContainsKey(message.peerId))
            {
                clientPlayers[message.peerId].GetComponent<SyncAnimatorHost>().SetTrigger("Red");
            }
        }
    }
}
