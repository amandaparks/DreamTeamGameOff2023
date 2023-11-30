using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DT_PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;

    private DT_SceneLoader _sceneLoader;

    
    // Start is called before the first frame update

    private void Awake()
    {

        _sceneLoader = FindObjectOfType<DT_SceneLoader>();

    }

    private void OnEnable()
    {
        // Set up listeners on pause menu buttons
        resumeButton.onClick.AddListener(delegate {ClickInput("resume");});
        restartButton.onClick.AddListener(delegate {ClickInput("restart");});
        quitButton.onClick.AddListener(delegate {ClickInput("quit");});
    }

    private void Start()
    {
        //Hide menu
        pauseMenu.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        // Remove listeners
        resumeButton.onClick.RemoveListener(delegate {ClickInput("resume");});
        restartButton.onClick.RemoveListener(delegate {ClickInput("restart");});
        quitButton.onClick.RemoveListener(delegate {ClickInput("quit");});
    }

    private void ClickInput(string button)
    {
        switch (button)
        {
            case "resume":
            {
                PauseUnpause();
            }
                break;
            case "restart":
            {
                StartCoroutine(_sceneLoader.LoadScene(GameManager.CurrentScene));
            }
                break;
            case "quit":
                {
                    //StartCoroutine(_sceneLoader.LoadScene(GameManager.GameScene.MainMenu));
                    Debug.Log("calling it QUITS");
                    Application.Quit();
                }
                break;
        }
    }

    public void PauseUnpause()
    {
        // If game is already paused
        if (GameManager.CurrentGameState == GameManager.GameState.Paused)
        {
            // Unpause the game
            pauseMenu.gameObject.SetActive(false);
            GameManager.CurrentGameState = GameManager.GameState.Playing;
        }
        // If game is not paused
        else if (GameManager.CurrentGameState != GameManager.GameState.Paused)
        {
            // Pause the game
            pauseMenu.gameObject.SetActive(true);
            GameManager.CurrentGameState = GameManager.GameState.Paused;
        }
        
    }
}
