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
    public static GameState CurrentGameState;
    public static GameScene CurrentScene;
    public static PlayerState CurrentPlayerState;
    public static PlayerLevel CurrentPlayerLevel;
    [HideInInspector] public Vector3 playerStartPos;
    private Graphic _blackPanel;
    [SerializeField] private float sceneFadeSpeed = 1.0f;

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
    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // This method will be called every time a new scene is loaded
        Debug.Log("Scene loaded: " + scene.name);
        CurrentPlayerState = PlayerState.Idle;
        CurrentGameState = GameState.Playing;
        Debug.Log($"Player starting position is {playerStartPos}");
        
    }

    public void StartGame()
    {
        StartCoroutine(LoadScene("WorldMap"));
    }

    public IEnumerator LoadScene(string scene)
    {
        Debug.Log($"Loading {scene}");
        StartCoroutine(FadeOut());
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(scene);
        StartCoroutine(FadeIn());
        
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
