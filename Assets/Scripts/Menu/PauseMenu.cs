using System;
using System.Collections;
using System.Collections.Generic;
using Mirror.Examples.NetworkRoom;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    //TODO GETSET Instead of public
    public static bool isPaused = false;
    public GameObject pauseMenuUI;
    public PlayerControl player;

    [SerializeField] private TextMeshProUGUI _networkInfo;
    
    private NetworkRoomManagerExt _networkManager;

    private void Start()
    {
        _networkManager = GameObject.Find("NetworkRoomManager").GetComponent<NetworkRoomManagerExt>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    private void Pause()
    {
        pauseMenuUI.SetActive(true);
        _networkManager.UpdateStatus(_networkInfo);
        player.enabled = false;
        isPaused = true;
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        player.enabled = true;
        isPaused = false;
    }

    public void Quit()
    {
        _networkManager.HandleStopButtons();
        SceneManager.LoadScene("LobbySceneV2");
    }
}
