using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class LoadingScreen : MonoBehaviour
{
    public CanvasGroup loadingScreen;
    public CanvasGroup mainMenu;
    public TMP_Text mapText;
    public float loadDuration;

    private Launcher launcher;
    private bool fadeIn = false;

    private Transform roomButton;


    void Start()
    {
        launcher = GameObject.Find("Launcher").GetComponent<Launcher>();
        loadingScreen.alpha = 0;
    }

    private void Update()
    {
        if (fadeIn)
        {
            if (loadingScreen.alpha < 1)
            {
                loadingScreen.alpha += Time.deltaTime;
                mainMenu.alpha -= Time.deltaTime * 2f;
                if (loadingScreen.alpha >= 1)
                {
                    fadeIn = false;
                    StartCoroutine(WaitLoading());
                }
            }
        }
    }

    public void ShowScreen(string m_name, Transform button)
    {
        mapText.text = m_name;
        roomButton = button;
        fadeIn = true;
    }

    IEnumerator WaitLoading()
    {
        yield return new WaitForSeconds(loadDuration);

        if (PhotonNetwork.IsMasterClient)
        {
            launcher.StartGame();
        }
        else
        {
            launcher.JoinRoom(roomButton);
        }
        
    }
}
