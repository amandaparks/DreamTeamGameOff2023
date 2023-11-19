using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Image = UnityEngine.UIElements.Image;

public class GameManager : MonoBehaviour
{
    // Static variable so any script can access this script by using GameManager.Instance
    public static GameManager Instance;
    public static GameState CurrentGameState
    {
        get { return _currentGameState;}
        set
        {
            _currentGameState = value;
            // Get the calling method's information
            System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace();
            System.Diagnostics.StackFrame stackFrame = stackTrace.GetFrame(1);
            System.Reflection.MethodBase methodBase = stackFrame.GetMethod();
            
            Debug.LogWarning($"CurrentGameState changed to: {value} by {methodBase.DeclaringType}.{methodBase.Name}");
        }
    }

    private static GameState _currentGameState;

    public static GameScene CurrentScene
    {
        get { return _CurrentScene;}
        set
        {
            _CurrentScene = value;
            // Get the calling method's information
            System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace();
            System.Diagnostics.StackFrame stackFrame = stackTrace.GetFrame(1);
            System.Reflection.MethodBase methodBase = stackFrame.GetMethod();
            
            Debug.LogWarning($"CurrentGameState changed to: {value} by {methodBase.DeclaringType}.{methodBase.Name}");
        }
    }

    private static GameScene _CurrentScene;
    public static PlayerState CurrentPlayerState;

    public static PlayerLevel CurrentPlayerLevel
    {
        get { return _currentPlayerLevel; }
        set
        {    _currentPlayerLevel = value;

                // Get the calling method's information
                System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace();
                System.Diagnostics.StackFrame stackFrame = stackTrace.GetFrame(1);
                System.Reflection.MethodBase methodBase = stackFrame.GetMethod();
            
                Debug.LogWarning($"CurrentGameState changed to: {value} by {methodBase.DeclaringType}.{methodBase.Name}");
        }
    }
    private static PlayerLevel _currentPlayerLevel;
    [HideInInspector] public Vector3 playerStartPos;
    private Graphic _blackPanel;
    [SerializeField] private float sceneFadeSpeed = 1.0f;
    public static PlayerLevel EndLevelPlayerLevel;

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
        MainMenu,
        WorldMap,
        Level_1,
        Level_2,
        Level_3,
        Level_4,
        Level_5,
        Level_6,
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
        _blackPanel = GetComponentInChildren<Graphic>();
        
    }

    // This is triggered by the start button on the MainMenu
    public void StartGame()
    {
        StartCoroutine(LoadScene("WorldMap"));
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // This method will be called every time a new scene is loaded
        Debug.Log("Scene loaded: " + scene.name);
        Debug.Log($"Player starting position is {playerStartPos}");
        
        // If there is no Level Manager, set these as default:
        if (!FindObjectOfType<DT_LevelManager>())
        {
            CurrentPlayerState = PlayerState.Idle;
            CurrentGameState = GameState.Playing;
        }
    }
    
    // Any script can use this method to load scenes
    public static IEnumerator LoadScene(string scene)
    {
        Debug.Log($"Loading {scene}");
        Instance.StartCoroutine(Instance.FadeOut());
        yield return new WaitForSeconds(0.5f);
        //Update the player's level
        CurrentPlayerLevel = EndLevelPlayerLevel;
        SceneManager.LoadScene(scene);
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
