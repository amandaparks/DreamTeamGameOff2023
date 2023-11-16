using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.InputSystem.UI;
using UnityEngine.EventSystems;

public class DT_PointClickInput : MonoBehaviour
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
    
    private GameManager _gameManager;
    private GameObject _player;
    

        /* THIS SCRIPT DOES TWO THINGS:
         * 1. It listens for mouse/touch input on UI Buttons and
         * invokes the same actions as the corresponding keyboard input
         * 2. It listens for keyboard input and makes the corresponding
         * UI buttons change colour/sprite as if they had been selected
        */
    
    // 1. MOUSE/TOUCH --> KEYBOARD

    // Add listeners to all the UI buttons
    void Start()
    {
        // Find the game manager and player
        _gameManager = GameManager.Instance;
        _player = GameObject.FindWithTag("Player");
        
        // Set up listeners to run methods when clicked
        button1.onClick.AddListener(delegate {MimicInput("Button1");});
        button2.onClick.AddListener(delegate {MimicInput("Button2");});
        button3.onClick.AddListener(delegate {MimicInput("Button3");});
        button4.onClick.AddListener(delegate {MimicInput("Button4");});
        button5.onClick.AddListener(delegate {MimicInput("Button5");});
        button6.onClick.AddListener(delegate {MimicInput("Button6");});
        button7.onClick.AddListener(delegate {MimicInput("Button7");});
        buttonB.onClick.AddListener(delegate {MimicInput("ButtonB");});
    }
    private void MimicInput (string button)
    {
        // First, check if we're in Bard Mode
        var isBardMode = _gameManager.playerState == GameManager.PlayerState.BardMode;
        
        switch (button)
        {
            case "Button1":
                if (isBardMode)
                {
                    _player.SendMessage("On_1");
                }
                else
                {
                    _player.SendMessage("OnDefend");
                }

                break;
            case "Button2":
                if (isBardMode)
                {
                    _player.SendMessage("On_2");
                }
                else
                {
                    _player.SendMessage("OnCrouch");
                }

                break;
            case "Button3":
                if (isBardMode)
                {
                    _player.SendMessage("On_3");
                }
                else
                {
                    _player.SendMessage("OnStepBkd");
                }

                break;
            case "Button4":
                if (isBardMode)
                {
                    _player.SendMessage("On_4");
                }
                else
                {
                    _player.SendMessage("OnClimb");
                }

                break;
            case "Button5":
                if (isBardMode)
                {
                    _player.SendMessage("On_5");
                }
                else
                {
                    _player.SendMessage("OnStepFwd");
                }
                break;
            case "Button6":
                if (isBardMode)
                {
                    _player.SendMessage("On_6");
                }
                else
                {
                    _player.SendMessage("OnAttack");
                }
                break;
            case "Button7":
                if (isBardMode)
                {
                    _player.SendMessage("On_7");
                }
                else
                {
                    _player.SendMessage("OnMagic");
                }
                break;
            case "ButtonB":
                //This is the same for both Action Maps
                _player.SendMessage("OnBard");
                break;
            default:
                Debug.LogError("No broadcast set.");
                break;
        }
    }


    // 2. KEYBOARD --> UI BUTTON SELECT

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
