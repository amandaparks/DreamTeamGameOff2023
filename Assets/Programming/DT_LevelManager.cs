using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DT_LevelManager : MonoBehaviour
{
    [Header("LEVEL SETTINGS")]
    [Space]
    [Header("Where is the Player?")]
    [SerializeField] private GameManager.GameScene _sceneName;
    [Header("Show instructions at the start?")] [Tooltip("You may wish to disable scrolls during testing.")]
    [SerializeField] private bool _showScroll;
    [Header("How many notes will the player ENTER with?")]
    [SerializeField] private GameManager.PlayerLevel _entersWith;
    [Header("How many notes will the player LEAVE with?")]
    [SerializeField] private GameManager.PlayerLevel _leavesWith;

    private DT_GameTextManager _gameTextManager;
    
    // Start is called before the first frame update
    void Awake()
    {
        GameManager.CurrentGameState = GameManager.GameState.Playing;
        GameManager.CurrentScene = _sceneName;
        GameManager.CurrentPlayerLevel = _entersWith;
        GameManager.EndLevelPlayerLevel = _leavesWith;
        _gameTextManager = FindObjectOfType<DT_GameTextManager>();
    }
    void Start()
    {
        // Tell game manager where the player is
        GameManager.PlayerStartPos = transform.position;
        Debug.Log($"Player starting position is {GameManager.PlayerStartPos}");
        
        // Load text if expected
        if (!_showScroll) return;
        _gameTextManager.MakeTextSceneRequest(DT_SO_GameText.GameText.TextType.Scroll,GameManager.GameScene.None);
    }
}
