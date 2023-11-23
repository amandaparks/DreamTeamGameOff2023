using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DT_LevelManager : MonoBehaviour
{
    
    [Header("What is this scene?")]
    [SerializeField] private GameManager.GameScene _gameScene;
    [Header("Display a scroll on start?")] 
    [SerializeField] private bool _showScroll;
    [Header("Player's current level:")]
    [SerializeField] private GameManager.PlayerLevel _startLevel;
    [Header("Level after completing this scene:")]
    [SerializeField] private GameManager.PlayerLevel _endLevel;

    private DT_GameTextManager _gameTextManager;
    
    // Start is called before the first frame update
    void Awake()
    {
        GameManager.CurrentGameState = GameManager.GameState.Playing;
        GameManager.CurrentScene = _gameScene;
        GameManager.CurrentPlayerLevel = _startLevel;
        GameManager.EndLevelPlayerLevel = _endLevel;
        _gameTextManager = FindObjectOfType<DT_GameTextManager>();
    }
    void Start()
    {
        
        // Load text if expected
        if (!_showScroll) return;
        _gameTextManager.MakeTextSceneRequest(DT_SO_GameText.GameText.TextType.Scroll,_gameScene.ToString());
    }
}
