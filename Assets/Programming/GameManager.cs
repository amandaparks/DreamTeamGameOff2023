using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Static variable so any script can access this script by using GameManager.Instance
    public static GameManager Instance;
    [HideInInspector] public GameState gameState;
    // [HideInInspector] public CurrentScene currentScene;
    [HideInInspector] public PlayerState playerState;
    [HideInInspector] public Vector3 playerStartPos;
    
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

    
    public enum GameState
    {
        Playing,
        Paused
    }

    public enum CurrentScene
    {
        WorldMap,
        Level1,
        Level2,
        Level3,
        Level4,
        Level5,
        Level6,
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
        Defending
    }

    // Start is called before the first frame update
    void Start()
    {
        playerState = PlayerState.Idle;
        gameState = GameState.Playing;
        Debug.Log($"Player starting position is {playerStartPos}");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
