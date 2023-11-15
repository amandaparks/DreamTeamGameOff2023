using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DT_PointClickInput : MonoBehaviour
{
    //private GameObject _player;
    private GameManager _gameManager;

    // Start is called before the first frame update
    void Start()
    {
        // Find the game manager
        _gameManager = GameManager.Instance;
    }

    // Routing through here in case we want to add anything later
    public void ButtonClick(string input)
    {
        BroadcastInput(input);
        Debug.Log($"CLICK: {input}");
    }
    private void BroadcastInput(string input)
    {
        // Check if we're in Bard Mode (this reflects current Action Map)
        var isBardMode = _gameManager.playerState == GameManager.PlayerState.BardMode;

        switch (input.ToLower())
        {
            case "button_1":
            case "button 1":
            case "button1":
                if (isBardMode)
                {
                    BroadcastMessage("On_1");
                }
                else
                {
                    BroadcastMessage("OnDefend");
                }

                break;

            case "button_2":
            case "button 2":
            case "button2":
                if (isBardMode)
                {
                    BroadcastMessage("On_2");
                }
                else
                {
                    BroadcastMessage("OnCrouch");
                }

                break;

            case "button_3":
            case "button 3":
            case "button3":
                if (isBardMode)
                {
                    BroadcastMessage("On_3");
                }
                else
                {
                    BroadcastMessage("OnStepBkd");
                }

                break;

            case "button_4":
            case "button 4":
            case "button4":
                if (isBardMode)
                {
                    BroadcastMessage("On_4");
                }
                else
                {
                    BroadcastMessage("OnClimb");
                }

                break;

            case "button_5":
            case "button 5":
            case "button5":
                if (isBardMode)
                {
                    BroadcastMessage("On_5");
                }
                else
                {
                    BroadcastMessage("OnStepFwd");
                }

                break;

            case "button_6":
            case "button 6":
            case "button6":
                if (isBardMode)
                {
                    BroadcastMessage("On_6");
                }
                else
                {
                    BroadcastMessage("OnAttack");
                }

                break;

            case "button_7":
            case "button 7":
            case "button7":
                if (isBardMode)
                {
                    BroadcastMessage("On_7");
                }
                else
                {
                    BroadcastMessage("OnMagic");
                }

                break;

            case "button_b":
            case "button b":
            case "buttonb":
                //This is the same for both Action Maps
                BroadcastMessage("OnBard");
                break;

            default:
                Debug.LogError("No broadcast set.");
                break;
        }
    }
}
