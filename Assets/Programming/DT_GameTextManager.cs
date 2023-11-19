using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class DT_GameTextManager : MonoBehaviour
{
    [SerializeField] private DT_SO_GameText gameTextAsset;
    private DT_SO_GameText.GameText[] _gameText;

    private TextMeshProUGUI _infoTextField;
    private TextMeshProUGUI _playerTextField;
    private TextMeshProUGUI _npcTextField;

    private Canvas _infoCanvas;
    private Canvas _playerCanvas;
    private Canvas _npcCanvas;

    private DT_SO_GameText.GameText _matchingEntry;
    private DT_SO_GameText.TextLines[] _matchingLines;
    private int _currentLineIndex;
    private DT_SO_GameText.TextLines.Speaker _currentSpeaker;
    private Canvas _currentCanvas;
    private string _sceneToLoad;
    
    private DT_InputManager _inputManager;
    private bool _canAdvance = true;
    public float cooldownTime = 0.5f;
    
    private void Awake()
    {
        // Find the input manager
        _inputManager = FindObjectOfType<DT_InputManager>();
        
        // Make sure game text asset has been assigned
        if (gameTextAsset == null)
        {
            Debug.LogError("Game Text Scriptable Object not assigned");
        }
        
        // Assign the data from the asset
        _gameText = gameTextAsset.gameText;

        // Find the relevant canvases and text boxes
        _infoCanvas = GameObject.FindGameObjectWithTag("UI_Info").GetComponentInChildren<Canvas>();
        _playerCanvas = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Canvas>();
        _npcCanvas = GameObject.FindGameObjectWithTag("NPC").GetComponentInChildren<Canvas>();

        _infoTextField = _infoCanvas.GetComponentInChildren<TextMeshProUGUI>();
        _playerTextField = _playerCanvas.GetComponentInChildren<TextMeshProUGUI>();
        _npcTextField = _npcCanvas.GetComponentInChildren<TextMeshProUGUI>();

        Debug.Log($"Found {_infoTextField.gameObject.name} and {_playerTextField.gameObject.name} and {_npcTextField.gameObject.name}");
        
        
        // Make sure they're all turned off for now
        _infoCanvas.gameObject.SetActive(false);
        _playerCanvas.gameObject.SetActive(false);
        _npcCanvas.gameObject.SetActive(false);

    }

    private void Start()
    {
        _currentCanvas = null;
        
        // Run text if there is any
        MakeTextSceneRequest(DT_SO_GameText.GameText.TextType.Information, null);
    }

    // Other game objects can also use this method to trigger text to appear
    // method takes parameters : text type (info or dialogue), scene to be loaded after (if not null)
    public void MakeTextSceneRequest(DT_SO_GameText.GameText.TextType textType, string sceneToLoad)
    {
        Debug.Log($"REQUEST RECEIVED");
        
        // First, see if there is a matching entry:
        _matchingEntry = FindEntry(textType);
        _sceneToLoad = sceneToLoad;

        // If there isn't...
        if (_matchingEntry == null)
        {
            // Load the scene if there is a request
            if (sceneToLoad != null)
            {
                StartCoroutine(GameManager.LoadScene(_sceneToLoad));
            }
            // Otherwise, make sure player set to idle so they aren't stuck.
            Debug.Log("No text to run. No scene to load.");
            GameManager.CurrentPlayerState = GameManager.PlayerState.Idle;
        }
        // If there is...
        else if (_matchingEntry != null)
        {
            // Run the text and then change scene if requested
            PerformRequest();
        }
    }

    private DT_SO_GameText.GameText FindEntry(DT_SO_GameText.GameText.TextType textType)
    {
        Debug.Log($"Looking for {textType} for {GameManager.CurrentScene} and {GameManager.CurrentPlayerLevel}");
        
        //Search through game text and find the one with matching:
        //Text Type + Current Scene and Current Player Level
        var entry =
            Array.Find(_gameText, entry => 
                entry.textBoxType == textType && 
                entry.sceneName == GameManager.CurrentScene && 
                entry.playerLevel == GameManager.CurrentPlayerLevel);
        
        // If an entry is found, 
        if (entry != null)
        {
            // return that entry
            return entry;
        }
        
        // If it's not found
        Debug.Log("No text to run.");
        return null;
    }
    private void PerformRequest()
    {
        // Set the player to Talking
        GameManager.CurrentPlayerState = GameManager.PlayerState.Talking;
        Debug.Log("PLAYER STATE: TALKING");
        // Switch to the Talking Action Map
        _inputManager.SwitchActionMap("Talking");
        // Grab the set of lines for that entry
        _matchingLines = _matchingEntry.textLines;
        // Set the line index to -1 so NextLine loads at 0
        _currentLineIndex = -1;
        
        // Display the first line
        NextLine();
        // Remaining lines will be triggered by player input
    }
    
    public void NextLine()
    {
        // If too soon to press key, do nothing
        if (!_canAdvance) return;
        // Otherwise, accept input and initiate cooldown
        StartCoroutine(Cooldown());
        
        // If the most recent line wasn't the last one
        if (_currentLineIndex != _matchingLines.Length-1)
        {
            // Increment and display next line
            _currentLineIndex++;
            DisplayText();
        }
        else // Finish up
        {
            FinishUp();
        }
    }

    private IEnumerator Cooldown()
    {
        _canAdvance = false;
        yield return new WaitForSeconds(cooldownTime);
        _canAdvance = true;
    }

    private void DisplayText()
    {
        // Figure out who the speaker is
        DT_SO_GameText.TextLines.Speaker newSpeaker = _matchingLines[_currentLineIndex].speaker;
        // If the speaker has changed (and current canvas is not null), disable the old speaker's canvas
        if (_currentCanvas != null && !_currentSpeaker.Equals(newSpeaker))
        {
            _currentCanvas.gameObject.SetActive(false);
        }

        // Choose text field depending on speaker
        switch (newSpeaker)
        {
            case DT_SO_GameText.TextLines.Speaker.Info:
            {
                //Populate text
                _infoTextField.text = _matchingLines[_currentLineIndex].text;
                //Turn on canvas
                _infoCanvas.gameObject.SetActive(true);
                // Remember new current speaker and new current canvas
                _currentSpeaker = DT_SO_GameText.TextLines.Speaker.Info;
                _currentCanvas = _infoCanvas;
                break;
            }
            case DT_SO_GameText.TextLines.Speaker.Player:
            {
                _playerTextField.text = _matchingLines[_currentLineIndex].text;
                _playerCanvas.gameObject.SetActive(true);
                _currentSpeaker = DT_SO_GameText.TextLines.Speaker.Player;
                _currentCanvas = _playerCanvas;
                break;
            }
            case DT_SO_GameText.TextLines.Speaker.NPC:
            {
                _npcTextField.text = _matchingLines[_currentLineIndex].text;
                _npcCanvas.gameObject.SetActive(true);
                _currentSpeaker = DT_SO_GameText.TextLines.Speaker.NPC;
                _currentCanvas = _npcCanvas;
                break;
            }
            default:
                Debug.LogError("Speaker type missing");
                break;
        }
    }

    private void FinishUp()
    {
        // deactivate the current speaker's canvas
        _currentCanvas.gameObject.SetActive(false);
        // reset everything we were tracking
        _currentCanvas = null;
        _currentLineIndex = -1;
        _matchingEntry = null;
        _matchingLines = null;

        // if there's a scene to load, do that
        if (_sceneToLoad != null)
        {
            StartCoroutine(GameManager.LoadScene(_sceneToLoad));
            _sceneToLoad = null;
            return;
        }
        // otherwise, set player back to idle
        GameManager.CurrentPlayerState = GameManager.PlayerState.Idle;
        // and switch back to the gameplay action map
        _inputManager.SwitchActionMap("Gameplay");
    }
    
}
