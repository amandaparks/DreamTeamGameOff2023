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
    [Header("Gameplay Controls")]
    [SerializeField] private InputActionReference defend;
    [SerializeField] private InputActionReference crouch;
    [SerializeField] private InputActionReference stepBkd;
    [SerializeField] private InputActionReference climb;
    [SerializeField] private InputActionReference stepFwd;
    [SerializeField] private InputActionReference attack;
    [SerializeField] private InputActionReference magic;
    [SerializeField] private InputActionReference bardOn;
    [Header("Bard Controls")]
    [SerializeField] private InputActionReference key1;
    [SerializeField] private InputActionReference key2;
    [SerializeField] private InputActionReference key3;
    [SerializeField] private InputActionReference key4;
    [SerializeField] private InputActionReference key5;
    [SerializeField] private InputActionReference key6;
    [SerializeField] private InputActionReference key7;
    [SerializeField] private InputActionReference bardOff;

    [Header("Talking Controls")] 
    [SerializeField] private InputActionReference next;
    // There HAS to be another way to do the above but I'll figure it out later
    
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

    private bool IsGamePaused()
    {
        // return bool value of whether or not game is paused
        return GameManager.CurrentGameState == GameManager.GameState.Paused;
    }

    public void SwitchActionMap(string mapName)
    {
        _playerInput.SwitchCurrentActionMap(mapName);
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
    
    // KEYBOARD INPUT SECTION

    // Really annoyed that I can't use callback context in the methods here but
    // absolutely can't be bothered switching to C# events at this point
    
    public void OnDefend()
    {
        // Do nothing if game paused
        if (IsGamePaused()) return;
        _playerActions.Defend();
    }

    public void OnCrouch()
    {
        if (IsGamePaused()) return;
        _playerActions.Crouch();
    }

    public void OnStepBkd()
    {
        if (IsGamePaused()) return;
        _playerMovement.StepBkd();
    }

    public void OnClimb()
    {
        if (IsGamePaused()) return;
        _playerMovement.Climb();
    }

    public void OnStepFwd()
    {
        if (IsGamePaused()) return;
        _playerMovement.Climb();
    }

    public void OnAttack()
    {
        if (IsGamePaused()) return;
        _playerActions.Attack();
    }

    public void OnMagic()
    {
        if (IsGamePaused()) return;
        _playerActions.Magic();
    }

    public void OnBard()
    {
        if (IsGamePaused()) return;
        _playerActions.Bard();
    }

    public void On_1()
    {
        if (IsGamePaused()) return;
        _bardMode.PlayNote("1");
    }

    public void On_2()
    {
        if (IsGamePaused()) return;
        _bardMode.PlayNote("2");
    }

    public void On_3()
    {
        if (IsGamePaused()) return;
        _bardMode.PlayNote("3");
    }

    public void On_4()
    {
        if (IsGamePaused()) return;
        _bardMode.PlayNote("4");
    }

    public void On_5()
    {
        if (IsGamePaused()) return;
        _bardMode.PlayNote("5");
    }
    
    public void On_6()
    {
        if (IsGamePaused()) return;
        _bardMode.PlayNote("6");
    }
    
    public void On_7()
    {
        if (IsGamePaused()) return;
        _bardMode.PlayNote("7");
    }
    
    public void On_B()
    {
        if (IsGamePaused()) return;
        _bardMode.PlayNote("B");
    }
// TALKING MODE
    public void OnNext()
    {
        if (IsGamePaused()) return;
        _playerActions.Next();
    }

    // KEYBOARD UI BUTTON SELECT / DESELECT
    
    // Subscribing to all the events above legit can't believe I have to do this urrrrgh
    
        private void OnEnable()
    {
        // Subscribe to sending context to the method
        defend.action.started += context => SelectDeselect(button1, context);
        crouch.action.started += context => SelectDeselect(button2, context);
        stepBkd.action.started += context => SelectDeselect(button3, context);
        climb.action.started += context => SelectDeselect(button4, context);
        stepFwd.action.started += context => SelectDeselect(button5, context);
        attack.action.started += context => SelectDeselect(button6, context);
        magic.action.started += context => SelectDeselect(button7, context);
        bardOn.action.started += context => SelectDeselect(buttonB, context);
        
        key1.action.started += context => SelectDeselect(button1, context);
        key2.action.started += context => SelectDeselect(button2, context);
        key3.action.started += context => SelectDeselect(button3, context);
        key4.action.started += context => SelectDeselect(button4, context);
        key5.action.started += context => SelectDeselect(button5, context);
        key6.action.started += context => SelectDeselect(button6, context);
        key7.action.started += context => SelectDeselect(button7, context);
        bardOff.action.started += context => SelectDeselect(buttonB, context);
        
        next.action.started += context => SelectDeselect(button5, context);
        
        defend.action.canceled += context => SelectDeselect(button1, context);
        crouch.action.canceled += context => SelectDeselect(button2, context);
        stepBkd.action.canceled += context => SelectDeselect(button3, context);
        climb.action.canceled += context => SelectDeselect(button4, context);
        stepFwd.action.canceled += context => SelectDeselect(button5, context);
        attack.action.canceled += context => SelectDeselect(button6, context);
        magic.action.canceled += context => SelectDeselect(button7, context);
        bardOn.action.canceled += context => SelectDeselect(buttonB, context);
        
        key1.action.canceled += context => SelectDeselect(button1, context);
        key2.action.canceled += context => SelectDeselect(button2, context);
        key3.action.canceled += context => SelectDeselect(button3, context);
        key4.action.canceled += context => SelectDeselect(button4, context);
        key5.action.canceled += context => SelectDeselect(button5, context);
        key6.action.canceled += context => SelectDeselect(button6, context);
        key7.action.canceled += context => SelectDeselect(button7, context);
        bardOff.action.canceled += context => SelectDeselect(buttonB, context);
        
        next.action.canceled += context => SelectDeselect(button5, context);
    }

    private void OnDisable()
    {
        //Unsubscribe sending context to the method
        defend.action.started -= context => SelectDeselect(button1, context);
        crouch.action.started -= context => SelectDeselect(button2, context);
        stepBkd.action.started -= context => SelectDeselect(button3, context);
        climb.action.started -= context => SelectDeselect(button4, context);
        stepFwd.action.started -= context => SelectDeselect(button5, context);
        attack.action.started -= context => SelectDeselect(button6, context);
        magic.action.started -= context => SelectDeselect(button7, context);
        bardOn.action.started -= context => SelectDeselect(buttonB, context);
        
        key1.action.started -= context => SelectDeselect(button1, context);
        key2.action.started -= context => SelectDeselect(button2, context);
        key3.action.started -= context => SelectDeselect(button3, context);
        key4.action.started -= context => SelectDeselect(button4, context);
        key5.action.started -= context => SelectDeselect(button5, context);
        key6.action.started -= context => SelectDeselect(button6, context);
        key7.action.started -= context => SelectDeselect(button7, context);
        bardOff.action.started -= context => SelectDeselect(buttonB, context);
        
        next.action.started -= context => SelectDeselect(button5, context);
        
        defend.action.canceled -= context => SelectDeselect(button1, context);
        crouch.action.canceled -= context => SelectDeselect(button2, context);
        stepBkd.action.canceled -= context => SelectDeselect(button3, context);
        climb.action.canceled -= context => SelectDeselect(button4, context);
        stepFwd.action.canceled -= context => SelectDeselect(button5, context);
        attack.action.canceled -= context => SelectDeselect(button6, context);
        magic.action.canceled -= context => SelectDeselect(button7, context);
        bardOn.action.canceled -= context => SelectDeselect(buttonB, context);
        
        key1.action.canceled -= context => SelectDeselect(button1, context);
        key2.action.canceled -= context => SelectDeselect(button2, context);
        key3.action.canceled -= context => SelectDeselect(button3, context);
        key4.action.canceled -= context => SelectDeselect(button4, context);
        key5.action.canceled -= context => SelectDeselect(button5, context);
        key6.action.canceled -= context => SelectDeselect(button6, context);
        key7.action.canceled -= context => SelectDeselect(button7, context);
        bardOff.action.canceled -= context => SelectDeselect(buttonB, context);
        
        next.action.canceled -= context => SelectDeselect(button5, context);
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

}
