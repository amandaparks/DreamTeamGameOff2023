using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DT_InputManager : MonoBehaviour
{
    [Header("INPUT MANAGER")]
    [Header(" -Buttons should already be assigned")]
    [Space]
    [SerializeField] private PlayerInput _playerInput;
    private GameManager _gameManager;
    private DT_BardMode _bardMode;
    private DT_PlayerActions _playerActions;
    private DT_PlayerMovement _playerMovement;
    private DT_PauseMenu _pauseMenu;
    private DT_GameplayUI _gameplayUI;
    private bool _isTalkControls;
    private Button buttonPause, button1, button2, button3, button4, button5, button6, button7, buttonB;
    private Button bardButton1, bardButton2, bardButton3, bardButton4, bardButton5, bardButton6, bardButton7, bardButtonB;
    
    /*   This script handles GAMEPLAY and BARD and TALKING input
     *      1. Listens for which the UI buttons are being clicked
     *      2. Receives keyboard input and..
     *      3. Mimics UI input
     *      4. Invokes the relevant methods on these scripts attached to the player:
     *          (or ignore if game is paused)
     *              - DT_PlayerActions
     *              - DT_PlayerMovement
     *              - DT_BardMode
     *      5. Holds method for switching action map
     */

    private void Awake()
    {
        // Assign components
        _gameplayUI = FindObjectOfType<DT_GameplayUI>();
        _gameManager = FindObjectOfType<GameManager>();
        _bardMode = GetComponent<DT_BardMode>();
        _playerActions = GetComponent<DT_PlayerActions>();
        _playerMovement = GetComponent<DT_PlayerMovement>();
        _pauseMenu = FindObjectOfType<DT_PauseMenu>();
        
        
        if (_playerInput == null)
        {
            Debug.LogError("PlayerInput not found on " + gameObject.name);
        }
    }

    private void Start()
    {
        // GamePlay Button Components 
        
        buttonPause = _gameplayUI._gPB;
        button1 = _gameplayUI._g1B;
        button2 = _gameplayUI._g2B;
        button3 = _gameplayUI._g3B;
        button4 = _gameplayUI._g4B;
        button5 = _gameplayUI._g5B;
        button6 = _gameplayUI._g6B;
        button7 = _gameplayUI._g7B;
        buttonB = _gameplayUI._gBB;
        
        // Bard Button Components 
        
        bardButton1 = _gameplayUI._b1B;
        bardButton2 = _gameplayUI._b2B;
        bardButton3 = _gameplayUI._b3B;
        bardButton4 = _gameplayUI._b4B;
        bardButton5 = _gameplayUI._b5B;
        bardButton6 = _gameplayUI._b6B;
        bardButton7 = _gameplayUI._b7B;
        bardButtonB = _gameplayUI._bBB;
    }

    private void OnEnable()
    {
        // Subscribe to be notified when any input action is triggered
        _playerInput.onActionTriggered += OnActionTriggered;
    }

    private void OnDisable()
    {
        // Unsubscribe from these events on disable
        _playerInput.onActionTriggered -= OnActionTriggered;
    }
    
    private bool IsGamePaused()
    {
        // return bool value of whether or not game is paused
        return GameManager.CurrentGameState == GameManager.GameState.Paused;
    }

    public void SwitchActionMap(string mapName)
    {
        _playerInput.SwitchCurrentActionMap(mapName);
    }
    
    // KEYBOARD INPUT SECTION 

    private void OnActionTriggered(InputAction.CallbackContext context)
    {
        if (context.action.name == "Pause")
        {   
            // Do Pause/Unpause
            SelectDeselect(buttonPause, context);
            Debug.Log("Pause Registered");
            return;
        }
        
        // Ignore other input actions if game paused
        if (IsGamePaused()) return;

        switch (context.action.name)
        {
        // GAMEPLAY
        case "Defend":
            {
                // Click button
                SelectDeselect(button1, context);
                break;
            }
            case "Crouch":
            {
                // Click button
                SelectDeselect(button2, context);
                break;
            }
            case "StepBkd":
            {
                // Click button
                SelectDeselect(button3, context);
                break;
            }
            case "Climb":
            {
                // Click button
                SelectDeselect(button4, context);
                break;
            }
            case "StepFwd":
            {
                // Click button
                SelectDeselect(button5, context);
                break;
            }
            case "Attack":
            {
                // Click button
                SelectDeselect(button6, context);
                break;
            }
            case "Magic":
            {
                // Click button
                SelectDeselect(button7, context);
                break;
            }
            case "BardOn":
            {
                // Click button
                SelectDeselect(buttonB, context);
                break;
            }
        // TALKING
            case "Next":
            {
                // Click button
                SelectDeselect(button5, context);
                break;
            }
        // BARD MODE
            case "BardOff":
            {
                // Click button
                SelectDeselect(bardButtonB, context);
                break;
            }
            case "1":
            {
                // Click button
                SelectDeselect(bardButton1, context);
                break;
            }
            case "2":
            {
                // Click button
                SelectDeselect(bardButton2, context);
                break;
            }
            case "3":
            {
                // Click button
                SelectDeselect(bardButton3, context);
                break;
            }
            case "4":
            {
                // Click button
                SelectDeselect(bardButton4, context);
                break;
            }
            case "5":
            {
                // Click button
                SelectDeselect(bardButton5, context);
                break;
            }
            case "6":
            {
                // Click button
                SelectDeselect(bardButton6, context);
                break;
            }
            case "7":
            {
                // Click button
                SelectDeselect(bardButton7, context);
                break;
            }

        }
    }

    private void SelectDeselect(Button button, InputAction.CallbackContext context)
    {
        // Make button appear selected depending on whether it's being pressed
        switch (context.phase)
        {
            case InputActionPhase.Started:
                //Make appear submitted
                ExecuteEvents.Execute(button.gameObject, new BaseEventData(EventSystem.current),
                    ExecuteEvents.submitHandler);
                break;
            case InputActionPhase.Canceled:
                //Make appear deselected
                ExecuteEvents.Execute(button.gameObject, new BaseEventData(EventSystem.current),
                    ExecuteEvents.deselectHandler);
                break;
        }
    }

    // CLICK INPUT SECTION
    public void ClickInput (string button)
    {
        if (button == "ButtonPause")
        {
            _pauseMenu.PauseUnpause();
            return;
        }
        
        // If game is paused, ignore action input
        if (IsGamePaused()) return;

        // Check if we're in Bard Mode - not being used now we have bard buttons
        // var isBardMode = GameManager.CurrentPlayerState == GameManager.PlayerState.BardMode;
        
        // Check if Player is talking
        var isTalking = GameManager.CurrentPlayerState == GameManager.PlayerState.Talking;
        
        switch (button)
        {
    // GAMEPLAY BUTTONS
            case "Button1":
                _playerActions.Defend();
                break;
            case "Button2":
                _playerActions.Crouch();
                break;
            case "Button3":
                _playerMovement.StepBkd();
                break;
            case "Button4":
                _playerMovement.Climb();
                break;
            case "Button5":
                if (isTalking) //IF TALKING
                {
                    _playerActions.Next();
                }
                else
                {
                    _playerMovement.StepFwd();
                }
                break;
            case "Button6":
                _playerActions.Attack();
                break;
            case "Button7":
                _playerActions.Magic();
                break;
            case "ButtonB": 
                _playerActions.Bard();
                break;
    // BARD BUTTONS
            case "BardButton1":
                _bardMode.PlayNote("1");
                break;
            case "BardButton2":
                _bardMode.PlayNote("2");
                break;
            case "BardButton3":
                _bardMode.PlayNote("3");
                break;
            case "BardButton4":
                _bardMode.PlayNote("4");
                break;
            case "BardButton5":
                _bardMode.PlayNote("5");
                break;
            case "BardButton6":
                _bardMode.PlayNote("6");
                break;
            case "BardButton7":
                _bardMode.PlayNote("7");
                break;
            case "BardButtonB":
                _bardMode.PlayNote("B");
                _playerActions.Bard();
                break;
            default:
                Debug.LogError("No action set.");
                break;
        }
    }
}
