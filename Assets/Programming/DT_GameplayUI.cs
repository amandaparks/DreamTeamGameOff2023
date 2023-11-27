using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DT_GameplayUI : MonoBehaviour
{
    public GameObject buttonPause, button1, button2, button3, button4, button5, button6, button7, buttonB;

    private bool _isTalkControls;

    private void Start()
    {
        HideButtons();
        ShowButtons(GameManager.CurrentPlayerLevel);
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
        button1.SetActive(false);
        button2.SetActive(false);
        button3.SetActive(false);
        button4.SetActive(false);
        button6.SetActive(false);
        button7.SetActive(false);
        buttonB.SetActive(false);
    }

    private void ShowButtons(GameManager.PlayerLevel newPlayerLevel)
    {
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
}