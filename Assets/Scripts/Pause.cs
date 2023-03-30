using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Photon.Pun;
using UnityEngine;

public class Pause : MonoBehaviour
{
    public static bool paused = false;
    private bool disconnecting = false;
    private Manager manager;

    private void Start()
    {
        manager = GameObject.Find("Manager").GetComponent<Manager>();;
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
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(0);
    }

}
