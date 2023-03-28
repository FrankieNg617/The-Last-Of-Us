using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

[System.Serializable]
public class ProfileData
{
    public string username;
    public int level;
    public int xp;

    public ProfileData()
    {
        this.username = "DEFAULT USERNAME";
        this.level = 0;
        this.xp = 0;
    }

    public ProfileData(string u, int l, int x)
    {
        this.username = u;
        this.level = l;
        this.xp = x;
    }
}

public class Launcher : MonoBehaviourPunCallbacks
{
    public InputField usernameField;
    public InputField roomnameField;
    public Slider maxPlayersSlider; 
    public Text maxPlayersValue;
    public static ProfileData myProfile = new ProfileData();

    public GameObject tabMain;
    public GameObject tabRooms;
    public GameObject tabCreate;

    public GameObject buttonRoom;

    private List<RoomInfo> roomList;


    public void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        myProfile = Data.LoadProfile();
        usernameField.text = myProfile.username;

        Connect();
    }

    //will be called after connected to the server
    public override void OnConnectedToMaster()
    {   
        Debug.Log("CONNECTED!");

        PhotonNetwork.JoinLobby();
        base.OnConnectedToMaster();
    }

    //will be called after joining a room
    public override void OnJoinedRoom()
    {
        StartGame();

        base.OnJoinedRoom();
    }

    //will be called if fail to join room
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Create();

        base.OnJoinRandomFailed(returnCode, message);
    }

    public void Connect()
    {
        Debug.Log("Trying to connect...");
        PhotonNetwork.GameVersion = "0.0.0";
        PhotonNetwork.ConnectUsingSettings();
    }

    public void Join()
    {   
        SetUpClientProfile();
        PhotonNetwork.JoinRandomRoom();
    }

    public void Create()
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = (byte) maxPlayersSlider.value;

        ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable(); 
        properties.Add("map", 0);
        options.CustomRoomProperties = properties; 

        PhotonNetwork.CreateRoom(roomnameField.text, options);
    }

    public void ChangeMap ()
    {
        // currentmap++;
        // if (currentmap >= maps.Length) currentmap = 0;
        // mapValue.text = "MAP: " + maps[currentmap].name.ToUpper();
    }

    public void ChangeMaxPlayersSlider (float t_value) 
    {
        maxPlayersValue.text = Mathf.RoundToInt(t_value).ToString();
    }

    public void TabCloseAll()
    {
        tabMain.SetActive(false);
        tabRooms.SetActive(false);
        tabCreate.SetActive(false);
    }

    public void TabOpenMain ()
    {
        TabCloseAll();
        tabMain.SetActive(true);
    }

    public void TabOpenRooms ()
    {
        TabCloseAll();
        tabRooms.SetActive(true);
    }

    public void TabOpenCreate ()
    {
        TabCloseAll();
        tabCreate.SetActive(true);
    
    }

    private void ClearRoomList ()
    {
        Transform content = tabRooms.transform.Find("Scroll View/Viewport/Content");
        foreach (Transform a in content) Destroy(a.gameObject);
    }

    public override void OnRoomListUpdate(List<RoomInfo> p_list)
    {
        roomList = p_list;
        ClearRoomList();

        Transform content = tabRooms.transform.Find("Scroll View/Viewport/Content");

        foreach (RoomInfo a in roomList)
        {
            GameObject newRoomButton = Instantiate(buttonRoom, content) as GameObject;

            newRoomButton.transform.Find("Name").GetComponent<Text>().text = a.Name;
            newRoomButton.transform.Find("Players").GetComponent<Text>().text = a.PlayerCount + " / " + a.MaxPlayers;

            // if (a.CustomProperties.ContainsKey("map"))
            //     newRoomButton.transform.Find("Map/Name").GetComponent<Text>().text = maps[(int)a.CustomProperties["map"]].name;
            // else
            //     newRoomButton.transform.Find("Map/Name").GetComponent<Text>().text = "-----";

            newRoomButton.GetComponent<Button>().onClick.AddListener(delegate { JoinRoom(newRoomButton.transform); });
        }

        base.OnRoomListUpdate(roomList);
    }

    public void JoinRoom (Transform p_button)
    {
        string t_roomName = p_button.Find("Name").GetComponent<Text>().text;
        PhotonNetwork.JoinRoom(t_roomName);  
    }

    public void SetUpClientProfile()
    {
        if(string.IsNullOrEmpty(usernameField.text))
        {
            myProfile.username = "User" + Random.Range(100, 1000);
        }
        else
        {
            myProfile.username = usernameField.text;
        }

        Data.SaveProfile(myProfile);
    }
     
    public void StartGame()
    {   
        if(string.IsNullOrEmpty(usernameField.text))
        {
            myProfile.username = "User" + Random.Range(100, 1000);
        }
        else
        {
            myProfile.username = usernameField.text;
        }

        //only the first player need to load the map
        if(PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            Data.SaveProfile(myProfile);
            PhotonNetwork.LoadLevel(1);
        }
    }
}
