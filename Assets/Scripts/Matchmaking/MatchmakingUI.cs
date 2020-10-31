using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MatchmakingUI : MonoBehaviour
{
    string roomCode = "";
    bool isHosting = false;
    bool isClienting = false;
    public InputField clientRoomCode;
    public Text hostGameText;
    public GameObject hostMenu;
    public GameObject clientMenu;

    Coroutine getRoomsCoroutine;

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
                        Debug.Log($"[Room Entry] {room.name}: {room.room_code}, {room.users}/5");
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

    public void StartHost()
    {
        if (!isHosting)
        {
            isHosting = true;
            GameController.Instance.IsHost = true;
            clientMenu.SetActive(false);
            hostGameText.text = "Start Game";
            GameController.Instance.StartNetworkHost();
            StopCoroutine(getRoomsCoroutine);
        }
        else
        {
            GameController.Instance.host.SetScene("GameWorld");
        }
    }

    public void StartClient()
    {
        if (!isClienting)
        {
            hostMenu.GetComponent<Image>().color = Color.clear;
            hostMenu.GetComponent<Button>().interactable = false;
            GameController.Instance.StartNetworkClient(roomCode);
        }
    }
}

public class MatchmakingRoomEntry
{
    public string name;
    public string room_code;
    public int users;
}
