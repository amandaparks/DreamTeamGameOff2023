using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Image = UnityEngine.UIElements.Image;

public class GameManager : MonoBehaviour
{
    [Header("GAME MANAGER")]
    [Header("Leave settings as is.")]
    [Space]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private GameObject sceneFadeCanvas;
    [SerializeField] private float sceneFadeSpeed = 1.0f;
    private Graphic _blackPanel;
    
    // Static variable so any script can access this script by using GameManager.Instance
    public static GameManager Instance;
    
    // EVENT for Game State
    public delegate void GameStateChangedHandler(GameState newGameState);
    public static event GameStateChangedHandler OnGameStateChanged;
    
    private static GameState _currentGameState;
    public static GameState CurrentGameState
    {
        get { return _currentGameState;}
        set
        {
            if (_currentGameState != value)
            {
                // Set the new state
                _currentGameState = value;

                // Trigger the event to notify subscribers about the state change
                OnGameStateChanged?.Invoke(_currentGameState);
            }
        }
    }

    // EVENT for Player State

    public delegate void PlayerStateChangeHandler(PlayerState newPlayerState);
    public static event PlayerStateChangeHandler OnPlayerStateChanged;
    
    private static PlayerState _currentPlayerState;

    public static PlayerState CurrentPlayerState
    {
        get { return _currentPlayerState; }
        set
        {
            if (_currentPlayerState != value)
            {
                _currentPlayerState = value;
                // Notify subscribers
                OnPlayerStateChanged?.Invoke(_currentPlayerState);
            }
        }
    }

    // EVENT for Player Level
    public delegate void PlayerLevelChangeHandler(PlayerLevel newPlayerLevel);
    public static event PlayerLevelChangeHandler OnPlayerLevelChanged;

    private static PlayerLevel _currentPlayerLevel;
    public static PlayerLevel CurrentPlayerLevel
    {
        get { return _currentPlayerLevel; }
        set
        {
            if (_currentPlayerLevel != value)
            {
                _currentPlayerLevel = value;
                // Notify subscribers
                OnPlayerLevelChanged?.Invoke(_currentPlayerLevel);
            }
        }
    }
    
    // PUBLIC STATIC Variables
    public static GameScene CurrentScene;
    public static PlayerLevel EndLevelPlayerLevel;
    public static Vector3 PlayerStartPos;
    
    
    

    public enum PlayerLevel
    {
        NewGame,
        OneNote,
        TwoNotes,
        ThreeNotes,
        FourNotes,
        FiveNotes,
        SixNotes,
        SevenNotes,
        Winner
    }
    

    public enum GameState
    {
        Playing,
        Paused
    }

    public enum GameScene
    {
        None,
        MainMenu,
        WorldMap,
        Dungeon_1,
        Dungeon_2,
        Dungeon_3,
        WorldMapRotated,
        Dungeon_4,
        Dungeon_5,
        Dungeon_6,
        Summit
    }

    public enum PlayerState
    {
        Idle,
        Talking,
        Stepping,
        Climbing,
        Crouching,
        Damaged,
        Attacking,
        Defending,
        Magic,
        BardMode
    }
    
    
    void Awake()
    {
        // We want one Game Manager to awake and persist for the rest of the game
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        // If the Game Manager awakens but another already exists, it should destroy itself
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        _blackPanel = sceneFadeCanvas.GetComponent<Graphic>();
        pauseMenu.gameObject.SetActive(false);
        // Set up listeners on pause menu buttons
        resumeButton.onClick.AddListener(delegate {ClickInput("resume");});
        quitButton.onClick.AddListener(delegate {ClickInput("quit");});
    }

    public void PauseUnpause()
    {
        // If game is already paused
        if (CurrentGameState == GameState.Paused)
        {
            // Unpause the game
            pauseMenu.gameObject.SetActive(false);
            CurrentGameState = GameState.Playing;
        }
        // If game is not paused
        else if (CurrentGameState != GameState.Paused)
        {
            // Pause the game
            pauseMenu.gameObject.SetActive(true);
            CurrentGameState = GameState.Paused;
        }
        
    }

    // This is triggered by the start button on the MainMenu
    public void StartGame()
    {
        StartCoroutine(LoadScene(GameScene.WorldMap));
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // This method will be called every time a new scene is loaded
        Debug.Log("Scene loaded: " + scene.name);
        
        PlayerStartPos = transform.position;
        Debug.Log($"Player starting position is {PlayerStartPos}");
        
        // If there is no Level Manager, set these as default:
        if (!FindObjectOfType<DT_LevelManager>())
        {
            CurrentPlayerState = PlayerState.Idle;
            CurrentGameState = GameState.Playing;
        }
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
            case "quit":
            {
                StartCoroutine(LoadScene(GameScene.MainMenu));
            }
                break;
        }
    }

    // Any script can use this method to load scenes
    public static IEnumerator LoadScene(GameScene scene)
    {
        // If there's no scene to load, break
        if (scene == GameScene.None) yield break;
        
        Debug.Log($"Loading {scene}");
        
        // Start Fade Out
        Instance.StartCoroutine(Instance.FadeOut());
        
        yield return new WaitForSeconds(0.5f);

        // Update the player's level
        switch (scene)
        {
            case GameScene.MainMenu:
                CurrentPlayerLevel = PlayerLevel.NewGame;
                break;
            default:
                CurrentPlayerLevel = EndLevelPlayerLevel;
                break;
        }
        
        // Then load the scene
        SceneManager.LoadScene(scene.ToString());
        
        // Start all new scenes on Idle
        CurrentPlayerState = PlayerState.Idle;
        
        // Fade back in
        Instance.StartCoroutine(Instance.FadeIn());
    }
    
    private IEnumerator FadeOut()
    {
        Color color = _blackPanel.color;
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
        while (color.a > 0)
        {
            color.a -= Time.deltaTime * sceneFadeSpeed;
            _blackPanel.color = color;
            yield return null;
        }
    }
    
    private void OnEnable()
    {
        // Subscribe to the sceneLoaded event when the script is enabled
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Unsubscribe from the sceneLoaded event when the script is disabled
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

}
