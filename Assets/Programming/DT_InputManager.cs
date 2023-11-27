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
        
        if (_playerInput == null)
        {
            Debug.LogError("PlayerInput not found on " + gameObject.name);
        }
    }

    private void Start()
    {
        button1 = _gameplayUI.button1.GetComponent<Button>();
        button2 = _gameplayUI.button2.GetComponent<Button>();
        button3 = _gameplayUI.button3.GetComponent<Button>();
        button4 = _gameplayUI.button4.GetComponent<Button>();
        button5 = _gameplayUI.button5.GetComponent<Button>();
        button6 = _gameplayUI.button6.GetComponent<Button>();
        button7 = _gameplayUI.button7.GetComponent<Button>();
        buttonB = _gameplayUI.buttonB.GetComponent<Button>();
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
        Debug.Log($"Input map is now: {mapName}");
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
            case "Bard":
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
            case "1":
            {
                // Click button
                SelectDeselect(button1, context);
                break;
            }
            case "2":
            {
                // Click button
                SelectDeselect(button2, context);
                break;
            }
            case "3":
            {
                // Click button
                SelectDeselect(button3, context);
                break;
            }
            case "4":
            {
                // Click button
                SelectDeselect(button4, context);
                break;
            }
            case "5":
            {
                // Click button
                SelectDeselect(button5, context);
                break;
            }
            case "6":
            {
                // Click button
                SelectDeselect(button6, context);
                break;
            }
            case "7":
            {
                // Click button
                SelectDeselect(button7, context);
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

        // Check if we're in Bard Mode
        var isBardMode = GameManager.CurrentPlayerState == GameManager.PlayerState.BardMode;
        
        // Check if Player is talking
        var isTalking = GameManager.CurrentPlayerState == GameManager.PlayerState.Talking;
        
        switch (button)
        {
            case "Button1":
                if (isBardMode)
                {
                    _bardMode.PlayNote("1");
                }
                else
                {
                    _playerActions.Defend();
                }
                break;
            case "Button2":
                if (isBardMode)
                {
                    _bardMode.PlayNote("2");
                }
                else
                {
                    _playerActions.Crouch();
                }
                break;
            case "Button3":
                if (isBardMode)
                {
                    _bardMode.PlayNote("3");
                }
                else
                {
                    _playerMovement.StepBkd();
                }
                break;
            case "Button4":
                if (isBardMode)
                {
                    _bardMode.PlayNote("4");
                }
                else
                {
                    _playerMovement.Climb();
                }

                break;
            case "Button5":
                if (isBardMode)
                {
                    _bardMode.PlayNote("5");
                }
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
                if (isBardMode)
                {
                    _bardMode.PlayNote("6");
                }
                else
                {
                    _playerActions.Attack();
                }
                break;
            case "Button7":
                if (isBardMode)
                {
                    _bardMode.PlayNote("7");
                }
                else
                {
                    _playerActions.Magic();
                }
                break;
            case "ButtonB":
                if (isBardMode)
                {
                    _bardMode.PlayNote("B");
                }
                else
                {
                    _playerActions.Bard();
                }
                break;
            default:
                Debug.LogError("No action set.");
                break;
        }
    }
}
