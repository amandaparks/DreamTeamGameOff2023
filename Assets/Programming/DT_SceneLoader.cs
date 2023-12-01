using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DT_SceneLoader : MonoBehaviour
{
    
    [SerializeField] private GameObject sceneFadeCanvas;
    [SerializeField] private float sceneFadeSpeed;
    private Graphic _blackPanel;
    private MusicManager _musicManager;

    private void Awake()
    {
        _blackPanel = sceneFadeCanvas.GetComponent<Graphic>();        
    }
    private void OnEnable()
    {
        // Subscribe to the sceneLoaded event when the script is enabled
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Start all new scenes on Idle and Playing
        GameManager.CurrentPlayerState = GameManager.PlayerState.Idle;
        GameManager.CurrentGameState = GameManager.GameState.Playing;
        
        // This method will be called every time a new scene is loaded
        Debug.Log("Scene loaded: " + scene.name);
    }

    private void Start()
    {
        // Start on black
        Color color = default;
        color.a = 1f;
        _blackPanel.color = color;
        StartCoroutine(FadeIn());
        _musicManager = FindObjectOfType<MusicManager>();
    }


    private void OnDisable()
    {
        // Unsubscribe from the sceneLoaded event when the script is disabled
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    
    // This is triggered by the start button on the MainMenu
    public void StartGame()
    {
        StartCoroutine(LoadScene(GameManager.GameScene.WorldMap));
    }

    // Any script can use this method to load scenes
    public IEnumerator LoadScene(GameManager.GameScene scene)
    {
        // If there's no scene to load, break
        if (scene == GameManager.GameScene.None) yield break;
        
        Debug.Log($"Loading {scene}");
        
        // Start Fade Out
        StartCoroutine(FadeOut());
        
        yield return new WaitForSeconds(0.5f);

        // If request is not coming from the world map, update player's level
        if (GameManager.CurrentScene != GameManager.GameScene.WorldMap)
        {
            switch (scene)
            {
                case GameManager.GameScene.MainMenu:
                    // If loading main menu, game has been restarted so set to newgame
                    GameManager.CurrentPlayerLevel = GameManager.PlayerLevel.NewGame;
                    break;
                
                default:
                    // In all other cases do what was set in game manager
                    GameManager.CurrentPlayerLevel = GameManager.EndLevelPlayerLevel;
                    break;
            }
        }

        // Then load the scene
        LoadTheScene(scene);
        
    }

    private void LoadTheScene(GameManager.GameScene scene)
    {        
        switch (scene)
        {
            case GameManager.GameScene.MainMenu:
                SceneManager.LoadScene("MainMenu");
                break;
            case GameManager.GameScene.WorldMap:
            case GameManager.GameScene.WorldMapRotated:
            case GameManager.GameScene.Summit:
                SceneManager.LoadScene("WorldMap");
                break;
            case GameManager.GameScene.Dungeon_1:
            case GameManager.GameScene.Dungeon_2:
            case GameManager.GameScene.Dungeon_3:
            case GameManager.GameScene.Dungeon_4:
            case GameManager.GameScene.Dungeon_5:
            case GameManager.GameScene.Dungeon_6:
                SceneManager.LoadScene(scene.ToString());
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(scene), scene, null);
        }
        Debug.Log($"change BGM to {scene}");
        _musicManager.ChangeBGM(scene);
    }

    private IEnumerator FadeOut()
    {
        Color color = _blackPanel.color;
        // While alpha channel less than 1
        while (color.a < 1)
        {
            color.a += Time.deltaTime * sceneFadeSpeed;
            _blackPanel.color = color;
            yield return null;
        }
    }
    
    private IEnumerator FadeIn()
    {
        Color color = _blackPanel.color;
        // While alpha channel more than 0
        while (color.a > 0)
        {
            color.a -= Time.deltaTime * sceneFadeSpeed;
            _blackPanel.color = color;
            yield return null;
        }
    }
}
