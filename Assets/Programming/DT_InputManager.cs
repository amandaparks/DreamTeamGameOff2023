using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
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
    private bool _isTalkControls;
    [SerializeField] private Button buttonPause, button1, button2, button3, button4, button5, button6, button7, buttonB;

    

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
     *      6. Hides kalimba keys that Player doesn't have yet
     */

    private void Awake()
    {
        // Assign components
        _gameManager = FindObjectOfType<GameManager>();
        _bardMode = GetComponent<DT_BardMode>();
        _playerActions = GetComponent<DT_PlayerActions>();
        _playerMovement = GetComponent<DT_PlayerMovement>();
        
        if (_playerInput == null)
        {
            Debug.LogError("PlayerInput not found on " + gameObject.name);
        }
        
        // Subscribe to be notified when any input action is triggered
        _playerInput.onActionTriggered += OnActionTriggered;
        
        // Subscribe to be notified when Player Level changes
        GameManager.OnPlayerLevelChanged += ShowButtons;
        // Subscribe to be notified when Player Status changes
        GameManager.OnPlayerStateChanged += CheckButtons;
        
        // Set up listeners to run methods when clicked
        buttonPause.onClick.AddListener(delegate {ClickInput("ButtonPause");});
        button1.onClick.AddListener(delegate {ClickInput("Button1");});
        button2.onClick.AddListener(delegate {ClickInput("Button2");});
        button3.onClick.AddListener(delegate {ClickInput("Button3");});
        button4.onClick.AddListener(delegate {ClickInput("Button4");});
        button5.onClick.AddListener(delegate {ClickInput("Button5");});
        button6.onClick.AddListener(delegate {ClickInput("Button6");});
        button7.onClick.AddListener(delegate {ClickInput("Button7");});
        buttonB.onClick.AddListener(delegate {ClickInput("ButtonB");});
        
        HideButtons();
        ShowButtons(GameManager.CurrentPlayerLevel);
    }


    private void CheckButtons(GameManager.PlayerState newPlayerState)
    {
        // If player not currently talking
        if (!_isTalkControls)
        {
            // But new status is talking
            if (newPlayerState == GameManager.PlayerState.Talking)
            {
                    // Hide buttons (except Next and Pause)
                    HideButtons();
                    _isTalkControls = true;
            }
        }
        // If player is currently talking
        else if (_isTalkControls)
        {
            // And new status is not talking
            if (newPlayerState != GameManager.PlayerState.Talking)
            {
                // Show controls for player's current level
                ShowButtons(GameManager.CurrentPlayerLevel);
                _isTalkControls = false;
            }
        }
    }

    private void HideButtons()
    {
        // Hide everything except for step fwd (button 5) and pause
        button1.gameObject.SetActive(false);
        button2.gameObject.SetActive(false);
        button3.gameObject.SetActive(false);
        button4.gameObject.SetActive(false);
        button6.gameObject.SetActive(false);
        button7.gameObject.SetActive(false);
        buttonB.gameObject.SetActive(false);
    }

    private void ShowButtons(GameManager.PlayerLevel newPlayerLevel)
    {
        StartCoroutine(DelayedShowButtons(newPlayerLevel));
    }

    private IEnumerator DelayedShowButtons(GameManager.PlayerLevel newPlayerLevel)
    {
        yield return null; // Wait for a frame
        
        switch (newPlayerLevel)
        {
            case GameManager.PlayerLevel.NewGame:
                button5.gameObject.SetActive(true); // step fwd
                break;
            case GameManager.PlayerLevel.OneNote:
                button5.gameObject.SetActive(true); // step fwd
                buttonB.gameObject.SetActive(true); // bard mode
                break;
            case GameManager.PlayerLevel.TwoNotes:
                button5.gameObject.SetActive(true); // step fwd
                buttonB.gameObject.SetActive(true); // bard mode
                button4.gameObject.SetActive(true); // climb
                break;
            case GameManager.PlayerLevel.ThreeNotes:
                button5.gameObject.SetActive(true); // step fwd
                buttonB.gameObject.SetActive(true); // bard mode
                button4.gameObject.SetActive(true); // climb
                button3.gameObject.SetActive(true); // step bkd
                break;
            case GameManager.PlayerLevel.FourNotes:
                button5.gameObject.SetActive(true); // step fwd
                buttonB.gameObject.SetActive(true); // bard mode
                button4.gameObject.SetActive(true); // climb
                button3.gameObject.SetActive(true); // step bkd
                button2.gameObject.SetActive(true); // crouch
                break;
            case GameManager.PlayerLevel.FiveNotes:
                button5.gameObject.SetActive(true); // step fwd
                buttonB.gameObject.SetActive(true); // bard mode
                button4.gameObject.SetActive(true); // climb
                button3.gameObject.SetActive(true); // step bkd
                button2.gameObject.SetActive(true); // crouch
                button6.gameObject.SetActive(true); // attack
                break;
            case GameManager.PlayerLevel.SixNotes:
                button5.gameObject.SetActive(true); // step fwd
                buttonB.gameObject.SetActive(true); // bard mode
                button4.gameObject.SetActive(true); // climb
                button3.gameObject.SetActive(true); // step bkd
                button2.gameObject.SetActive(true); // crouch
                button6.gameObject.SetActive(true); // attack
                button1.gameObject.SetActive(true); // defend
                break;
            case GameManager.PlayerLevel.SevenNotes:
            case GameManager.PlayerLevel.Winner:
                button5.gameObject.SetActive(true); // step fwd
                buttonB.gameObject.SetActive(true); // bard mode
                button4.gameObject.SetActive(true); // climb
                button3.gameObject.SetActive(true); // step bkd
                button2.gameObject.SetActive(true); // crouch
                button6.gameObject.SetActive(true); // attack
                button1.gameObject.SetActive(true); // defend
                button7.gameObject.SetActive(true); // magic
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from these events on destroy
        _playerInput.onActionTriggered -= OnActionTriggered;
        GameManager.OnPlayerLevelChanged -= ShowButtons;
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
        // We'll use default UI input controls for menus so can ignore any action input
        if (IsGamePaused()) return;

        switch (context.action.name)
        {
        // GAMEPLAY
        case "Pause":
            {   
                // Click button
                SelectDeselect(buttonPause, context);
                break;
            }
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
    private void ClickInput (string button)
    {
        // If game is paused, ignore input
        if (IsGamePaused()) return;

        // Check if we're in Bard Mode
        var isBardMode = GameManager.CurrentPlayerState == GameManager.PlayerState.BardMode;
        
        // Check if Player is talking
        var isTalking = GameManager.CurrentPlayerState == GameManager.PlayerState.Talking;
        
        switch (button)
        {
            case "ButtonPause":
                _gameManager.PauseUnpause();
                break;
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
