using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool isPaused = false;
    public GameObject pauseMenuUI;
    //public PlayerControl player;

    [SerializeField] private TextMeshProUGUI _networkInfo;
    

    private void Start()
    {
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
        //player.enabled = false;
        isPaused = true;
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        //player.enabled = true;
        isPaused = false;
    }

    public void Quit()
    {
        SceneManager.LoadScene("LobbySceneV2");
    }
}
