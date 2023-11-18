using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DT_LevelManager : MonoBehaviour
{
    
    [Header("What is this scene?")]
    [SerializeField] private GameManager.GameScene _gameScene;
    [Header("What should the player state be on start?")]
    [SerializeField] private GameManager.PlayerState _playerState;
    [Header("What is the player's level?")]
    [SerializeField] private GameManager.PlayerLevel _playerLevel;
    //[Header("On completion, what will be the player's level?")]
    //[SerializeField] private GameManager.PlayerLevel _playerGetLevel;

    // Start is called before the first frame update
    void Awake()
    {
        GameManager.CurrentGameState = GameManager.GameState.Playing;
        GameManager.CurrentPlayerState = _playerState;
        GameManager.CurrentScene = _gameScene;
        GameManager.CurrentPlayerLevel = _playerLevel;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
