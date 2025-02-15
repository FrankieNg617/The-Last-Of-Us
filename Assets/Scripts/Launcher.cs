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

[System.Serializable]
public class MapData
{
    public string name;
    public int scene;
}

public class Launcher : MonoBehaviourPunCallbacks
{
    public InputField usernameField;
    public InputField roomnameField;
    public Text levelValue;
    public Text xpValue;
    public Text mapValue;
    public Text modeValue;
    public Slider maxPlayersSlider;
    public Slider killsGoalSlider;
    public Slider timesSlider;
    public Text maxPlayersValue;
    public Text killsGoalValue;
    public Text timesValue;
    public static ProfileData myProfile = new ProfileData();
    public static int killsGoal = 0;
    public static int times = 0;

    public GameObject tabMain;
    public GameObject tabRooms;
    public GameObject tabCreate;

    public GameObject buttonRoom;
    public GameObject gameTitle;

    public MapData[] maps;
    private int currentmap = 0;

    private List<RoomInfo> roomList = new List<RoomInfo>();

    private LoadingScreen loadingScreen;


    public void Awake()
    {
        loadingScreen = GameObject.Find("Loading Manager").GetComponent<LoadingScreen>();
        PhotonNetwork.AutomaticallySyncScene = true;

        myProfile = Data.LoadProfile();
        usernameField.text = myProfile.username;
        levelValue.text = $"LEVEL {myProfile.level}";
        xpValue.text = $"XP {myProfile.xp} / 60";

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
        loadingScreen.ShowScreen(maps[currentmap].name, null);

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
        options.MaxPlayers = (byte)maxPlayersSlider.value;

        options.CustomRoomPropertiesForLobby = new string[] { "map", "mode" };

        ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();
        properties.Add("map", currentmap);
        properties.Add("mode", (int)GameSettings.GameMode);
        options.CustomRoomProperties = properties;

        killsGoal = (int) killsGoalSlider.value;
        times = (int) timesSlider.value;

        PhotonNetwork.CreateRoom(roomnameField.text, options);
    }

    public void ChangeMap()
    {
        currentmap++;
        if (currentmap >= maps.Length) currentmap = 0;
        mapValue.text = "MAP: " + maps[currentmap].name.ToUpper();
    }

    public void ChangeMode()
    {
        int newMode = (int)GameSettings.GameMode + 1;
        if (newMode >= System.Enum.GetValues(typeof(GameMode)).Length) newMode = 0;
        GameSettings.GameMode = (GameMode)newMode;
        modeValue.text = "MODE: " + System.Enum.GetName(typeof(GameMode), newMode);
    }

    public void ChangeMaxPlayersSlider(float t_value)
    {
        maxPlayersValue.text = Mathf.RoundToInt(t_value).ToString();
    }

    public void ChangeKillsGoalSlider(float t_value)
    {
        killsGoalValue.text = Mathf.RoundToInt(t_value).ToString();
    }

    public void ChangeTimesSlider(float t_value)
    {
        timesValue.text = Mathf.RoundToInt(t_value).ToString();
    }

    public void TabCloseAll()
    {
        tabMain.SetActive(false);
        tabRooms.SetActive(false);
        tabCreate.SetActive(false);
    }

    public void TabOpenMain()
    {
        TabCloseAll();
        tabMain.SetActive(true);
        gameTitle.SetActive(true);
    }

    public void TabOpenRooms()
    {
        TabCloseAll();
        gameTitle.SetActive(false);
        tabRooms.SetActive(true);
    }

    public void TabOpenCreate()
    {
        TabCloseAll();
        gameTitle.SetActive(false);
        tabCreate.SetActive(true);

        roomnameField.text = "";

        currentmap = 0;
        mapValue.text = "MAP: " + maps[currentmap].name.ToUpper();

        GameSettings.GameMode = (GameMode)0;
        modeValue.text = "MODE: " + System.Enum.GetName(typeof(GameMode), (GameMode)0);

        maxPlayersSlider.value = maxPlayersSlider.maxValue;
        killsGoalSlider.value = killsGoalSlider.maxValue;
        timesSlider.value = timesSlider.maxValue;

        maxPlayersValue.text = Mathf.RoundToInt(maxPlayersSlider.value).ToString();
        killsGoalValue.text = Mathf.RoundToInt(killsGoalSlider.value).ToString();
        timesValue.text = Mathf.RoundToInt(timesSlider.value).ToString();
    }

    private void ClearRoomList()
    {
        Transform content = tabRooms.transform.Find("Scroll View/Viewport/Content");
        foreach (Transform a in content) Destroy(a.gameObject);
    }

    private void VerifyUsername()
    {
        if (string.IsNullOrEmpty(usernameField.text))
        {
            myProfile.username = "CYY" + Random.Range(100, 1000);
        }
        else
        {
            myProfile.username = usernameField.text;
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> p_list)
    {
        RoomListUpdate(p_list);
        ClearRoomList();

        Transform content = tabRooms.transform.Find("Scroll View/Viewport/Content");

        foreach (RoomInfo a in roomList)
        {
            GameObject newRoomButton = Instantiate(buttonRoom, content) as GameObject;

            newRoomButton.transform.Find("Name").GetComponent<Text>().text = a.Name;
            newRoomButton.transform.Find("Players").GetComponent<Text>().text = a.PlayerCount + " / " + a.MaxPlayers;

            if (a.CustomProperties.ContainsKey("map"))
                newRoomButton.transform.Find("Map/Name").GetComponent<Text>().text = maps[(int)a.CustomProperties["map"]].name;
            else
                newRoomButton.transform.Find("Map/Name").GetComponent<Text>().text = "-----";

            newRoomButton.GetComponent<Button>().onClick.AddListener(delegate { loadingScreen.ShowScreen(maps[(int)a.CustomProperties["map"]].name, newRoomButton.transform); });
        }

        base.OnRoomListUpdate(roomList);
    }

    private void RoomListUpdate(List<RoomInfo> p_list)
    {
        bool isExist;

        for (int i = 0; i < p_list.Count; i++)
        {
            isExist = false;

            for (int j = 0; j < roomList.Count; j++)
            {
                if (p_list[i].Name == roomList[j].Name)
                {
                    isExist = true;

                    if (p_list[i].RemovedFromList || p_list[i].PlayerCount == 0)
                    {
                        roomList.Remove(roomList[j]);
                    }
                    else
                    {
                        roomList[j] = p_list[i];
                    }

                    break;
                }
            }

            if (!isExist && p_list[i].PlayerCount != 0)
            {
                roomList.Add(p_list[i]);
            }
        }
    }

    public void JoinRoom(Transform p_button)
    {
        string t_roomName = p_button.Find("Name").GetComponent<Text>().text;

        VerifyUsername();
        Data.SaveProfile(myProfile);

        RoomInfo roomInfo = null;
        Transform buttonParent = p_button.parent;
        for (int i = 0; i < buttonParent.childCount; i++)
        {
            if (buttonParent.GetChild(i).Equals(p_button))
            {
                roomInfo = roomList[i];
                break;
            }
        }

        if (roomInfo != null)
        {
            LoadGameSettings(roomInfo);
            PhotonNetwork.JoinRoom(t_roomName);
        }
    }

    public void LoadGameSettings(RoomInfo roomInfo)
    {
        GameSettings.GameMode = (GameMode)roomInfo.CustomProperties["mode"];
    }

    public void SetUpClientProfile()
    {
        VerifyUsername();

        Data.SaveProfile(myProfile);
    }

    public void StartGame()
    {
        VerifyUsername();

        //only the first player need to load the map
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            Data.SaveProfile(myProfile);
            PhotonNetwork.LoadLevel(maps[currentmap].scene);
        }
    }
}
