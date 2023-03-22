using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class ProfileData
{
    public string username;
    public int level;
    public int xp;
}

public class Launcher : MonoBehaviourPunCallbacks
{
    public InputField usernameField;
    public static ProfileData myProfile = new ProfileData();


    public void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        Connect();
    }

    //will be called after connected to the server
    public override void OnConnectedToMaster()
    {   
        Debug.Log("CONNECTED!");

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
        PhotonNetwork.JoinRandomRoom();
    }

    public void Create()
    {
        PhotonNetwork.CreateRoom("");
    }

    public void StartGame()
    {   
        if(string.IsNullOrEmpty(usernameField.text))
        {
            myProfile.username = "CYY" + Random.Range(100, 1000);
        }
        else
        {
            myProfile.username = usernameField.text;
        }

        //only the first player need to load the map
        if(PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            PhotonNetwork.LoadLevel(1);
        }
    }
}
