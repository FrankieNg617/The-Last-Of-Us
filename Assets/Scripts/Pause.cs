using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Photon.Pun;
using UnityEngine;

public class Pause : MonoBehaviourPunCallbacks
{
    public GameObject mapcam;
    public static bool paused = false;
    private bool disconnecting = false;
    private Manager manager;

    private void Start()
    {
        manager = GameObject.Find("Manager").GetComponent<Manager>();;
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
        base.OnLeftRoom();
    }
    
    public void TogglePause()
    {
        if (disconnecting) return;

        paused = !paused;

        transform.GetChild(0).gameObject.SetActive(paused);
        Cursor.lockState = (paused) ? CursorLockMode.None : CursorLockMode.Confined;
        Cursor.visible = paused;
    }

    public void Quit()
    {
        disconnecting = true;
        manager.RemovePlayer();

        // activate map camera
        mapcam.SetActive(true);

        PhotonNetwork.LeaveRoom(); 
    }

}
