using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class DT_GameplayUI : MonoBehaviour
{
    public GameObject buttonPause, button1, button2, button3, button4, button5, button6, button7, buttonB;
    
    public GameObject bardButton1, bardButton2, bardButton3, bardButton4, bardButton5, bardButton6, bardButton7, bardButtonB;
    
    private bool _isTalkControls;
    private bool _isBardControls;
    
    public Button _gPB, _g1B, _g2B, _g3B, _g4B, _g5B, _g6B, _g7B, _gBB;

    public Button _b1B, _b2B, _b3B, _b4B, _b5B, _b6B, _b7B, _bBB;

    private void Awake()
    {
        FindButtons();
        
    }
    

    private void Start()
    {
        
        HideButtons();
        ShowButtons(GameManager.CurrentPlayerLevel);
    }

    private void FindButtons()
    { 
        // Gameplay Button Components
        
        _gPB = buttonPause.GetComponent<Button>(); 
        _g1B = button1.GetComponent<Button>();
        _g2B = button2.GetComponent<Button>();
        _g3B = button3.GetComponent<Button>();
        _g4B = button4.GetComponent<Button>();
        _g5B = button5.GetComponent<Button>();
        _g6B = button6.GetComponent<Button>();
        _g7B = button7.GetComponent<Button>();
        _gBB = buttonB.GetComponent<Button>();
        
        // Bard Button Components 
        
        _b1B = bardButton1.GetComponent<Button>();
        _b2B = bardButton2.GetComponent<Button>();
        _b3B = bardButton3.GetComponent<Button>();
        _b4B = bardButton4.GetComponent<Button>();
        _b5B = bardButton5.GetComponent<Button>();
        _b6B = bardButton6.GetComponent<Button>();
        _b7B = bardButton7.GetComponent<Button>();
        _bBB = bardButtonB.GetComponent<Button>();
    }

    private void HideButtons()
    {
        // Hide everything except for step fwd (button 5) and pause
        button1.SetActive(false);
        button2.SetActive(false);
        button3.SetActive(false);
        button4.SetActive(false);
        button6.SetActive(false);
        button7.SetActive(false);
        buttonB.SetActive(false);
        
        bardButton1.SetActive(false);
        bardButton2.SetActive(false);
        bardButton3.SetActive(false);
        bardButton4.SetActive(false);
        bardButton5.SetActive(false);
        bardButton6.SetActive(false);
        bardButton7.SetActive(false);
        bardButtonB.SetActive(false);
    }

    private void HideBardButtons()
    {
        Debug.Log("Hiding BARD buttons");
        bardButton1.SetActive(false);
        bardButton2.SetActive(false);
        bardButton3.SetActive(false);
        bardButton4.SetActive(false);
        bardButton5.SetActive(false);
        bardButton6.SetActive(false);
        bardButton7.SetActive(false);
        bardButtonB.SetActive(false);
    }

    private void OnEnable()
    {
        // Subscribe to be notified when Player Level changes
        GameManager.OnPlayerLevelChanged += ShowButtons;
        // Subscribe to be notified when Player Status changes
        GameManager.OnPlayerStateChanged += CheckButtons;
    }

    private void OnDisable()
    {
        GameManager.OnPlayerLevelChanged -= ShowButtons;
        GameManager.OnPlayerStateChanged -= CheckButtons;
    }

    private void CheckButtons(GameManager.PlayerState newPlayerState)
    {
        // If player not currently talking
        if (!_isTalkControls)
        {
            // But new status is talking
            if (newPlayerState == GameManager.PlayerState.Talking)
            {
                // Talk controls
                TalkControls("on");
                _isTalkControls = true;
            }
        }
        // If player is currently talking
        else if (_isTalkControls)
        {
            // And new status is not talking
            if (newPlayerState != GameManager.PlayerState.Talking)
            {
                TalkControls("off");
                _isTalkControls = false;
            }
        }
        
        // If player is not currently Bard Mode
        if (!_isBardControls)
        {
            // But new status is bard mode
            {
                if (newPlayerState == GameManager.PlayerState.BardMode)
                {
                    _isBardControls = true;
                    // Hide everything
                    HideButtons();
                    // Bard controls on
                    BardControls();
                }
            }
        }
        // If player is currently Bard Mode
        else if (_isBardControls)
        {
            // And new status is not bard mode
            {
                if (newPlayerState != GameManager.PlayerState.BardMode)
                {
                    Debug.Log("Deactivating BARD controls");
                    _isBardControls = false;
                    // Hide all bard buttons
                    HideBardButtons();
                    // Enable buttons for current level
                    ShowButtons(GameManager.CurrentPlayerLevel);
                    
                }
            }
        }
    }

    private void BardControls()
    {
        switch (GameManager.CurrentPlayerLevel)
        {
            case GameManager.PlayerLevel.NewGame:
                bardButton5.SetActive(true); // E
                bardButtonB.SetActive(true); // bard mode
                break;
            case GameManager.PlayerLevel.OneNote:
                bardButton5.SetActive(true); // E
                bardButtonB.SetActive(true); // bard mode
                break;
            case GameManager.PlayerLevel.TwoNotes:
                bardButton5.SetActive(true); // E
                bardButtonB.SetActive(true); // bard mode
                bardButton4.SetActive(true); // C
                break;
            case GameManager.PlayerLevel.ThreeNotes:
                bardButton5.SetActive(true); // E
                bardButtonB.SetActive(true); // bard mode
                bardButton4.SetActive(true); // C
                bardButton3.SetActive(true); // D
                break;
            case GameManager.PlayerLevel.FourNotes:
                bardButton5.SetActive(true); // E
                bardButtonB.SetActive(true); // bard mode
                bardButton4.SetActive(true); // C
                bardButton3.SetActive(true); // D
                bardButton2.SetActive(true); // F
                break;
            case GameManager.PlayerLevel.FiveNotes:
                bardButton5.SetActive(true); // E
                bardButtonB.SetActive(true); // bard mode
                bardButton4.SetActive(true); // C
                bardButton3.SetActive(true); // D
                bardButton2.SetActive(true); // F
                bardButton6.SetActive(true); // G
                break;
            case GameManager.PlayerLevel.SixNotes:
                bardButton5.SetActive(true); // E
                bardButtonB.SetActive(true); // bard mode
                bardButton4.SetActive(true); // C
                bardButton3.SetActive(true); // D
                bardButton2.SetActive(true); // F
                bardButton6.SetActive(true); // G
                bardButton1.SetActive(true); // A
                break;
            case GameManager.PlayerLevel.SevenNotes:
            case GameManager.PlayerLevel.Winner:
                bardButton5.SetActive(true); // E
                bardButtonB.SetActive(true); // bard mode
                bardButton4.SetActive(true); // C
                bardButton3.SetActive(true); // D
                bardButton2.SetActive(true); // F
                bardButton6.SetActive(true); // G
                bardButton1.SetActive(true); // A
                bardButton7.SetActive(true); // B
                break;
        }
    }


    private void ShowButtons(GameManager.PlayerLevel newPlayerLevel)
    {
        Debug.Log("ENTERING SHOWBUTTONS()");
        switch (newPlayerLevel)
        {
            case GameManager.PlayerLevel.NewGame:
                button5.SetActive(true); // step fwd
                break;
            case GameManager.PlayerLevel.OneNote:
                button5.SetActive(true); // step fwd
                buttonB.SetActive(true); // bard mode
                break;
            case GameManager.PlayerLevel.TwoNotes:
                button5.SetActive(true); // step fwd
                buttonB.SetActive(true); // bard mode
                button4.SetActive(true); // climb
                break;
            case GameManager.PlayerLevel.ThreeNotes:
                button5.SetActive(true); // step fwd
                buttonB.SetActive(true); // bard mode
                button4.SetActive(true); // climb
                button3.SetActive(true); // step bkd
                break;
            case GameManager.PlayerLevel.FourNotes:
                button5.SetActive(true); // step fwd
                buttonB.SetActive(true); // bard mode
                button4.SetActive(true); // climb
                button3.SetActive(true); // step bkd
                button2.SetActive(true); // crouch
                break;
            case GameManager.PlayerLevel.FiveNotes:
                button5.SetActive(true); // step fwd
                buttonB.SetActive(true); // bard mode
                button4.SetActive(true); // climb
                button3.SetActive(true); // step bkd
                button2.SetActive(true); // crouch
                button6.SetActive(true); // attack
                break;
            case GameManager.PlayerLevel.SixNotes:
                button5.SetActive(true); // step fwd
                buttonB.SetActive(true); // bard mode
                button4.SetActive(true); // climb
                button3.SetActive(true); // step bkd
                button2.SetActive(true); // crouch
                button6.SetActive(true); // attack
                button1.SetActive(true); // defend
                break;
            case GameManager.PlayerLevel.SevenNotes:
            case GameManager.PlayerLevel.Winner:
                button5.SetActive(true); // step fwd
                buttonB.SetActive(true); // bard mode
                button4.SetActive(true); // climb
                button3.SetActive(true); // step bkd
                button2.SetActive(true); // crouch
                button6.SetActive(true); // attack
                button1.SetActive(true); // defend
                button7.SetActive(true); // magic
                break;
        }
    }

    private void TalkControls(string state)
    {
        // Talk controls only need affect gameplay buttons
        if (state == "on")
        {
            switch (GameManager.CurrentPlayerLevel)
            {
                case GameManager.PlayerLevel.NewGame:
                    _g1B.interactable = true; // step fwd / Next
                    _gBB.interactable = false; // bard mode
                    break;
                case GameManager.PlayerLevel.OneNote:
                    _g1B.interactable = true; // step fwd / Next
                    _gBB.interactable = false; // bard mode
                    break;
                case GameManager.PlayerLevel.TwoNotes:
                    _g1B.interactable = true; // step fwd / Next
                    _gBB.interactable = false; // bard mode
                    _g4B.interactable = false; // climb
                    break;
                case GameManager.PlayerLevel.ThreeNotes:
                    _g1B.interactable = true; // step fwd / Next
                    _gBB.interactable = false; // bard mode
                    _g4B.interactable = false; // climb
                    _g3B.interactable = false; // step bkd
                    break;
                case GameManager.PlayerLevel.FourNotes:
                    _g1B.interactable = true; // step fwd / Next
                    _gBB.interactable = false; // bard mode
                    _g4B.interactable = false; // climb
                    _g3B.interactable = false; // step bkd
                    _g2B.interactable = false; // crouch
                    break;
                case GameManager.PlayerLevel.FiveNotes:
                    _g1B.interactable = true; // step fwd / Next
                    _gBB.interactable = false; // bard mode
                    _g4B.interactable = false; // climb
                    _g3B.interactable = false; // step bkd
                    _g2B.interactable = false; // crouch
                    _g6B.interactable = false; // attack
                    break;
                case GameManager.PlayerLevel.SixNotes:
                    _g1B.interactable = true; // step fwd / Next
                    _gBB.interactable = false; // bard mode
                    _g4B.interactable = false; // climb
                    _g3B.interactable = false; // step bkd
                    _g2B.interactable = false; // crouch
                    _g6B.interactable = false; // attack
                    _g1B.interactable = false; // defend
                    break;
                case GameManager.PlayerLevel.SevenNotes:
                case GameManager.PlayerLevel.Winner:
                    _g1B.interactable = true; // step fwd / Next
                    _gBB.interactable = false; // bard mode
                    _g4B.interactable = false; // climb
                    _g3B.interactable = false; // step bkd
                    _g2B.interactable = false; // crouch
                    _g6B.interactable = false; // attack
                    _g1B.interactable = false; // defend
                    _g7B.interactable = false; // magic
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        else if (state == "off")
        {
            switch (GameManager.CurrentPlayerLevel)
            {
                case GameManager.PlayerLevel.NewGame:
                    _g1B.interactable = true; // step fwd / Next
                    _gBB.interactable = true; // bard mode
                    break;
                case GameManager.PlayerLevel.OneNote:
                    _g1B.interactable = true; // step fwd / Next
                    _gBB.interactable = true; // bard mode
                    break;
                case GameManager.PlayerLevel.TwoNotes:
                    _g1B.interactable = true; // step fwd / Next
                    _gBB.interactable = true; // bard mode
                    _g4B.interactable = true; // climb
                    break;
                case GameManager.PlayerLevel.ThreeNotes:
                    _g1B.interactable = true; // step fwd / Next
                    _gBB.interactable = true; // bard mode
                    _g4B.interactable = true; // climb
                    _g3B.interactable = true; // step bkd
                    break;
                case GameManager.PlayerLevel.FourNotes:
                    _g1B.interactable = true; // step fwd / Next
                    _gBB.interactable = true; // bard mode
                    _g4B.interactable = true; // climb
                    _g3B.interactable = true; // step bkd
                    _g2B.interactable = true; // crouch
                    break;
                case GameManager.PlayerLevel.FiveNotes:
                    _g1B.interactable = true; // step fwd / Next
                    _gBB.interactable = true; // bard mode
                    _g4B.interactable = true; // climb
                    _g3B.interactable = true; // step bkd
                    _g2B.interactable = true; // crouch
                    _g6B.interactable = true; // attack
                    break;
                case GameManager.PlayerLevel.SixNotes:
                    _g1B.interactable = true; // step fwd / Next
                    _gBB.interactable = true; // bard mode
                    _g4B.interactable = true; // climb
                    _g3B.interactable = true; // step bkd
                    _g2B.interactable = true; // crouch
                    _g6B.interactable = true; // attack
                    _g1B.interactable = true; // defend
                    break;
                case GameManager.PlayerLevel.SevenNotes:
                case GameManager.PlayerLevel.Winner:
                    _g1B.interactable = true; // step fwd / Next
                    _gBB.interactable = true; // bard mode
                    _g4B.interactable = true; // climb
                    _g3B.interactable = true; // step bkd
                    _g2B.interactable = true; // crouch
                    _g6B.interactable = true; // attack
                    _g1B.interactable = true; // defend
                    _g7B.interactable = true; // magic
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}

