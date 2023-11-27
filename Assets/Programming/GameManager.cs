using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("GAME MANAGER")]
    [Header("Keeps track of player progress.")]
    
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
                //Debug.LogWarning($"PLAYER STATE CHANGED TO: {_currentPlayerState}");
            }
        }
    }

    // EVENT for Player Level
    public delegate void PlayerLevelChangeHandler(PlayerLevel newPlayerLevel);
    public static event PlayerLevelChangeHandler OnPlayerLevelChanged;

    private static PlayerLevel _currentPlayerLevel = PlayerLevel.NewGame; // Instantiate as New Game
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
}
