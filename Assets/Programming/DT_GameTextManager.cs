using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class DT_GameTextManager : MonoBehaviour
{
    [Header("GAME TEXT MANAGER")]
    [Header(" -All text stored in GameTextAsset")]
    [Header(" -UI EventSystem lives here")]
    [Header(" -Scroll lives here")]
    [Header(" -Bubbles live on characters")]
    [Space(25)]
    [SerializeField] private DT_SO_GameText gameTextAsset;
    private DT_SO_GameText.GameText[] _gameText;
    [Tooltip("Prevents player from accidentally skipping dialogue")]
    public float cooldownTime = 0.5f;

    private TextMeshProUGUI _infoTextField;
    private TextMeshProUGUI _playerTextField;
    private TextMeshProUGUI _npcTextField;

    private GameObject _infoCanvasGameObject;
    private GameObject _playerCanvasGameObject;
    private GameObject _npcCanvasGameObject;

    private DT_SO_GameText.GameText _matchingEntry;
    private DT_SO_GameText.TextLines[] _matchingLines;
    private int _currentLineIndex = -1;
    private DT_SO_GameText.TextLines.Speaker _currentSpeaker;
    private GameObject _currentCanvasGameObject;
    private GameManager.GameScene _sceneToLoad;
    private DT_SceneLoader _sceneLoader;
    private DT_EnemyManager _enemyManager;
    
    private DT_InputManager _inputManager;
    private bool _canAdvance = true;
    private bool _isWorldMapRequest;
    private bool _isConfirmed;
   
    
    private void Awake()
    {
        // Find the input manager and scene loader
        _inputManager = FindObjectOfType<DT_InputManager>();
        _sceneLoader = FindObjectOfType<DT_SceneLoader>();
        _enemyManager = FindObjectOfType<DT_EnemyManager>();
        
        // Make sure game text asset has been assigned
        if (gameTextAsset == null)
        {
            Debug.LogError("Game Text Scriptable Object not assigned");
        }
        
        // Assign the data from the asset
        _gameText = gameTextAsset.gameText;
        
        // Find the relevant objects
        _infoCanvasGameObject = GameObject.FindGameObjectWithTag("InfoCanvas");
        _playerCanvasGameObject = GameObject.FindGameObjectWithTag("PlayerCanvas");
        _npcCanvasGameObject= GameObject.FindGameObjectWithTag("NPCCanvas");
        
        // Find canvases and text boxes and turn them all off for now

        if (_playerCanvasGameObject != null)
        {
            Debug.Log($"Player Canvas GO: {_playerCanvasGameObject.name}");
            _playerTextField = _playerCanvasGameObject.GetComponentInChildren<TextMeshProUGUI>();
            _playerCanvasGameObject.SetActive(false);
        }
        else { Debug.LogWarning("Just FYI, no player canvas in this scene."); }

        if (_npcCanvasGameObject != null)
        {
            Debug.Log($"NPC Canvas GO: {_npcCanvasGameObject.name}");
            _npcTextField = _npcCanvasGameObject.GetComponentInChildren<TextMeshProUGUI>();
            _npcCanvasGameObject.SetActive(false);
        }
        else { Debug.LogWarning("Just FYI, no npc canvas in this scene."); }
        
        if (_infoCanvasGameObject != null)
        {
            Debug.Log($"Info Canvas GO: {_infoCanvasGameObject.name}");
            _infoTextField = _infoCanvasGameObject.GetComponentInChildren<TextMeshProUGUI>();
            _infoCanvasGameObject.SetActive(false);
        }
        else { Debug.LogWarning("Just FYI, no info canvas in this scene."); }
    }

    // Other game objects can also use this method to trigger text to appear
    // method takes parameters : text type (info or dialogue), scene to be loaded after (if not null)
    public void MakeTextSceneRequest(DT_SO_GameText.GameText.TextType textType, GameManager.GameScene sceneToLoad)
    {
        Debug.Log($"REQUEST RECEIVED");
        
        // First, see if there is a matching entry:
        _matchingEntry = FindEntry(textType);
        _sceneToLoad = sceneToLoad;

        // If there is no matching text entry
        if (_matchingEntry == null)
        {
            // Load the scene if there is a request
            if (sceneToLoad != GameManager.GameScene.None)
            {
                StartCoroutine(_sceneLoader.LoadScene(_sceneToLoad));
            }
            else
            {
                // Otherwise, make sure player set to idle so they aren't stuck.
                Debug.Log("No text to run. No scene to load.");
                GameManager.CurrentPlayerState = GameManager.PlayerState.Idle;
            }
        }
        // If there is a matching text entry, run request
        else if (_matchingEntry != null)
        {
            // Run the text and then change scene if requested
            StartCoroutine(PerformRequest());
            
        }
    }

    public void WorldMapRequest(GameManager.GameScene scene)
    {
        Debug.Log($"WORLD MAP REQUEST RECEIVED");
        _isWorldMapRequest = true;
        _sceneToLoad = scene;
        
        StartCoroutine(RunWorldMapRequest());
    }

    private IEnumerator RunWorldMapRequest()
    {
        // Set the player to Talking
        GameManager.CurrentPlayerState = GameManager.PlayerState.Talking;
        Debug.Log("PLAYER STATE: TALKING");
        
        // Switch to the Talking Action Map
        _inputManager.SwitchActionMap("Talking");

        //Just seems a little too snappy so...
        yield return new WaitForSeconds(0.7f);
        
        
        
        // Display line
        NextLine();
    }

    private DT_SO_GameText.GameText FindEntry(DT_SO_GameText.GameText.TextType textType)
    {
        Debug.Log($"Looking for {textType} for {GameManager.CurrentScene}");
        
        //Search through game text and find the one with matching:
        //Text Type + Current Scene and Level Bool + Required Level
        var entry =
            Array.Find(_gameText, entry => 
                entry.textBoxType == textType && 
                entry.sceneName == GameManager.CurrentScene);
        
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
    private IEnumerator PerformRequest()
    {
        // turn off enemy spawner if there is one
        if (_enemyManager != null)
        {
            _enemyManager.gameObject.SetActive(false);
        }

        // Set the player to Talking
        GameManager.CurrentPlayerState = GameManager.PlayerState.Talking;
        Debug.Log("PLAYER STATE: TALKING");
        // Switch to the Talking Action Map
        _inputManager.SwitchActionMap("Talking");
        
        //Just seems a little too snappy so...
        yield return new WaitForSeconds(0.7f);
        
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

        switch (_isWorldMapRequest)
        {
            case true:
                // If it's from the world map, show confirmation
                if (!_isConfirmed)
                {
                    Confirmation();
                }
                else
                {
                    FinishUp();
                }
                break;
            case false:
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
                break;
        }

    }

    private void Confirmation()
    {
        
        
        //Populate text
        _infoTextField.text = "Enter the Dungeon";
        //Turn on canvas
        _infoCanvasGameObject.SetActive(true);
        // Remember new current speaker and new current canvas
        _currentSpeaker = DT_SO_GameText.TextLines.Speaker.Info;
        _currentCanvasGameObject = _infoCanvasGameObject;

        _isConfirmed = true;
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
        // If the speaker has changed (and a speech bubble is active), disable the old speaker's bubble
        if (_currentCanvasGameObject != null && !_currentSpeaker.Equals(newSpeaker))
        {
            _currentCanvasGameObject.SetActive(false);
        }

        // Choose text field depending on speaker
        switch (newSpeaker)
        {
            case DT_SO_GameText.TextLines.Speaker.Info:
            {
                if (_infoCanvasGameObject == null)
                {
                    Debug.LogError("Next speaker is Info but no Info text box exists.");
                    break;
                }
                // Remember new current speaker and new current canvas
                _currentSpeaker = DT_SO_GameText.TextLines.Speaker.Info;
                _currentCanvasGameObject = _infoCanvasGameObject;
                
                //Populate text
                _infoTextField.text = _matchingLines[_currentLineIndex].text;
                //Turn on canvas
                _infoCanvasGameObject.SetActive(true);
                break;
            }
            case DT_SO_GameText.TextLines.Speaker.Player:
            {
                if (_playerCanvasGameObject == null)
                {
                    Debug.LogError("Next speaker is Player but no Player text box exists.");
                    break;
                }
                _playerTextField.text = _matchingLines[_currentLineIndex].text;
                _playerCanvasGameObject.SetActive(true);
                _currentSpeaker = DT_SO_GameText.TextLines.Speaker.Player;
                _currentCanvasGameObject = _playerCanvasGameObject;
                break;
            }
            case DT_SO_GameText.TextLines.Speaker.NPC:
            {
                if (_npcCanvasGameObject == null)
                {
                    Debug.LogError("Next speaker is NPC but no NPC text box exists.");
                    break;
                }
                _npcTextField.text = _matchingLines[_currentLineIndex].text;
                _npcCanvasGameObject.SetActive(true);
                _currentSpeaker = DT_SO_GameText.TextLines.Speaker.NPC;
                _currentCanvasGameObject = _npcCanvasGameObject;
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
        _currentCanvasGameObject.SetActive(false);
        // reset everything we were tracking
        
        _currentLineIndex = -1;
        _matchingEntry = null;
        _matchingLines = null;
        _currentCanvasGameObject = null;
        
        // if there's a scene to load, do that
        if (_sceneToLoad != GameManager.GameScene.None)
        {
            StartCoroutine(_sceneLoader.LoadScene(_sceneToLoad));
            _sceneToLoad = GameManager.GameScene.None;
        }
        else
        {
            // otherwise, set player back to idle
            GameManager.CurrentPlayerState = GameManager.PlayerState.Idle;
            // and switch back to the gameplay action map
            _inputManager.SwitchActionMap("Gameplay");
            
            // and turn enemy spawner back on
            // turn off enemy spawner if there is one
            if (_enemyManager != null)
            {
                _enemyManager.gameObject.SetActive(true);
            }
        }
    }
    
}
