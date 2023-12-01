using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DT_EndSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject summitDragon;
    private DT_GameTextManager _gameTextManager;
    [SerializeField] private DT_KalimbaStones[] _stones;
    private bool _isEndOfGame;
    private bool _isCoroutineRunning;
    public float timeLimit = 20f;
    private bool isEndGameControls;
    private DT_GameplayUI _gameplayUI;
    private DT_PlayerActions _playerActions;

    private void Awake()
    {
        _gameTextManager = FindObjectOfType<DT_GameTextManager>();
        _gameplayUI = FindObjectOfType<DT_GameplayUI>();
        _playerActions = FindObjectOfType<DT_PlayerActions>();
        summitDragon.gameObject.SetActive(false);
    }

    private void Start()
    {
        // Turn off all the kalimba stones except the regular ones
        ResetStones();

        // If player is not at end of game, do nothing else here
        if (GameManager.CurrentPlayerLevel != GameManager.PlayerLevel.SevenNotes) return;

        _isEndOfGame = true;
        
        // Show first of the ending scrolls
        ShowFirstScroll();
    }

    
    private void ShowFirstScroll()
    {
        _gameTextManager.MakeTextSceneRequest(DT_SO_GameText.GameText.TextType.Scroll, GameManager.GameScene.None);
    }
    
    private void Update()
    {
        if (!_isEndOfGame) return;
        // If player has pressed magic...
        if (GameManager.CurrentPlayerState == GameManager.PlayerState.Magic)
        {
            if (!_isCoroutineRunning)
            {
                _isCoroutineRunning = true;
                StartCoroutine(ItsBardinTime());
            }
        }

        if (isEndGameControls) return;
        
        if (GameManager.CurrentPlayerState != GameManager.PlayerState.Talking)
        {
            _gameplayUI.EndGameControls();
            isEndGameControls = true;
        }
    }

    public void NotePlayed(string note)
    {
        Debug.Log("Note Played");
        if (_isCoroutineRunning)
        {
            Debug.Log("Checking note");
            foreach (var entry in _stones)
            {
                if (entry.note == note)
                {
                    entry.glowing.SetActive(false);
                    entry.activated.SetActive(true);
                    entry.isPlayed = true;
                    Debug.Log($"Note {entry.note} played");
                }
            }
        }
    }
    
    private void ResetStones()
    {
        foreach (var entry in _stones)
        {
            entry.stone.SetActive(true);
            entry.glowing.SetActive(false);
            entry.activated.SetActive(false);
        }
    }
    
    private IEnumerator ItsBardinTime()
    {
        // Turn on glowing stones
        foreach (var entry  in _stones)
        {  
            entry.stone.SetActive(false);
            entry.glowing.SetActive(true);
        }

        // do nothing until all stones are activated within 20 seconds
        float startTime = Time.time;
        
        while (!IsAllActivated() && Time.time - startTime < timeLimit)
        {
            
            yield return null;
        }
        
        // Check why it came out of the loop
        if (IsAllActivated())
        {
            //Force player out of bard mode
            _playerActions.Bard();
            // Make rainbow dragon appear
            summitDragon.SetActive(true);
            // Bask in its majesty
            yield return new WaitForSeconds(3f);
            // Chat
            Success();
        }
        else
        {
            // Timeout logic
            ResetStones();
            _isCoroutineRunning = false;
        }
    }

    private void Success()
    {
        // Set level to winner (this will trigger an additional scroll after conversation)
        GameManager.CurrentPlayerLevel = GameManager.PlayerLevel.Winner;
        
        // Show End Conversation
        _gameTextManager.MakeTextSceneRequest(DT_SO_GameText.GameText.TextType.SpeechBubbles, GameManager.GameScene.None);
        
    }

    private bool IsAllActivated()
    {
        foreach (var entry in _stones)
        {
            if (entry.isPlayed == false)
            {
                return false;
            }
        }

        // If we  make it through the loop without a false, then they are all true
        return true;
    }

}

[System.Serializable]

public class DT_KalimbaStones
{
    public string note;
    public GameObject stone;
    public GameObject glowing;
    public GameObject activated;
    [HideInInspector] public bool isPlayed;
}