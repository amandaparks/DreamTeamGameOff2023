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
    [Header("Gameplay UI Buttons")]
    [SerializeField] private Button button1, button2, button3, button4, button5, button6, button7, buttonB;

    private DT_BardMode _bardMode;
    private DT_PlayerActions _playerActions;
    private DT_PlayerMovement _playerMovement;
    private PlayerInput _playerInput;

    /*   This script handles GAMEPLAY and BARD and TALKING input
     *      1. Listens for which the UI buttons are being clicked
     *      2. Receives keyboard input
     *      3. Invokes the relevant methods on these scripts attached to the player:
     *          (or ignore if game is paused)
     *              - DT_PlayerActions
     *              - DT_PlayerMovement
     *              - DT_BardMode
     *      4. Mimics UI clicks based on keyboard input
     *      5. Holds method for switching action map
     */

    private void Awake()
    {
        // Assign components
        _bardMode = GetComponent<DT_BardMode>();
        _playerActions = GetComponent<DT_PlayerActions>();
        _playerMovement = GetComponent<DT_PlayerMovement>();
        _playerInput = GetComponent<PlayerInput>();
        
        if (_playerInput == null)
        {
            Debug.LogError("PlayerInput not found on " + gameObject.name);
        }
        
        // Subscribe to be notified when any input action is triggered
        _playerInput.onActionTriggered += OnActionTriggered;
        
        // Set up listeners to run methods when clicked
        button1.onClick.AddListener(delegate {ClickInput("Button1");});
        button2.onClick.AddListener(delegate {ClickInput("Button2");});
        button3.onClick.AddListener(delegate {ClickInput("Button3");});
        button4.onClick.AddListener(delegate {ClickInput("Button4");});
        button5.onClick.AddListener(delegate {ClickInput("Button5");});
        button6.onClick.AddListener(delegate {ClickInput("Button6");});
        button7.onClick.AddListener(delegate {ClickInput("Button7");});
        buttonB.onClick.AddListener(delegate {ClickInput("ButtonB");});
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from the onActionTriggered event when the script is destroyed
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
        // We'll use default controls for menus so can ignore any action input
        if (IsGamePaused()) return;

        switch (context.action.name)
        {
        // GAMEPLAY
            case "Defend":
            {
                // Do action
                if (context.performed)
                {
                    _playerActions.Defend();
                }

                // Make button appear clicked
                SelectDeselect(button1, context);
                break;
            }
            case "Crouch":
            {
                if (context.performed)
                {
                    _playerActions.Crouch();
                }

                SelectDeselect(button2, context);
                break;
            }
            case "StepBkd":
            {
                if (context.performed)
                {
                    _playerMovement.StepBkd();
                }

                SelectDeselect(button3, context);
                break;
            }
            case "Climb":
            {
                if (context.performed)
                {
                    _playerMovement.Climb();
                }

                SelectDeselect(button4, context);
                break;
            }
            case "StepFwd":
            {
                if (context.performed)
                {
                    _playerMovement.StepFwd();
                }

                SelectDeselect(button5, context);
                break;
            }
            case "Attack":
            {
                if (context.performed)
                {
                    _playerActions.Attack();
                }

                SelectDeselect(button6, context);
                break;
            }
            case "Magic":
            {
                if (context.performed)
                {
                    _playerActions.Magic();
                }

                SelectDeselect(button7, context);
                break;
            }
            case "Bard":
            {
                if (context.performed)
                {
                    _playerActions.Bard();
                }

                SelectDeselect(buttonB, context);
                break;
            }
        // TALKING
            case "Next":
            {
                if (context.performed)
                {
                    _playerActions.Next();
                }

                SelectDeselect(button5, context);
                break;
            }
        // BARD MODE
            case "1":
            {
                if (context.performed)
                {
                    _bardMode.PlayNote("1");
                }

                SelectDeselect(button1, context);
                break;
            }
            case "2":
            {
                if (context.performed)
                {
                    _bardMode.PlayNote("2");
                }

                SelectDeselect(button2, context);
                break;
            }
            case "3":
            {
                if (context.performed)
                {
                    _bardMode.PlayNote("3");
                }

                SelectDeselect(button3, context);
                break;
            }
            case "4":
            {
                if (context.performed)
                {
                    _bardMode.PlayNote("4");
                }

                SelectDeselect(button4, context);
                break;
            }
            case "5":
            {
                if (context.performed)
                {
                    _bardMode.PlayNote("5");
                }

                SelectDeselect(button5, context);
                break;
            }
            case "6":
            {
                if (context.performed)
                {
                    _bardMode.PlayNote("6");
                }

                SelectDeselect(button6, context);
                break;
            }
            case "7":
            {
                if (context.performed)
                {
                    _bardMode.PlayNote("7");
                }

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
                Debug.Log("Input started");
                //Execute the event for when a button is pressed aka submitted
                ExecuteEvents.Execute(button.gameObject, new BaseEventData(EventSystem.current),
                    ExecuteEvents.submitHandler);
                break;
            case InputActionPhase.Canceled:
                Debug.Log("Input canceled");
                //Execute the event for when a button is deselected
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
