using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class DT_PlayerActions : MonoBehaviour
{
    private GameManager _gameManager;
    private bool _isBardMode;
    private PlayerInput _playerInput;
    
    // Start is called before the first frame update
    void Start()
    {
        //Bard mode off by default
        _isBardMode = false;
        
        //Get player input component
        _playerInput = GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Crouch()
    {
        // Check we can do it
        if (!CanPerformAction("Crouch")) return;
        
        // If not already crouching
        if ( GameManager.CurrentPlayerState != GameManager.PlayerState.Crouching)
        {
            // Update Player State
            Debug.Log("PLAYER_STATE: CROUCHING");
            GameManager.CurrentPlayerState = GameManager.PlayerState.Crouching;
        }
        // If already crouching
        else if (GameManager.CurrentPlayerState == GameManager.PlayerState.Crouching)
        {
            // Update Player State
            Debug.Log("PLAYER_STATE: IDLE");
            GameManager.CurrentPlayerState = GameManager.PlayerState.Idle;
        }
    }
    public void Attack()
    {
        // Check we can do it
        if (!CanPerformAction("Attack")) return;
        // Update Player State
        Debug.Log("PLAYER_STATE: ATTACKING");
        GameManager.CurrentPlayerState = GameManager.PlayerState.Attacking;
        // Wait then go to Idle
        StartCoroutine(WaitAndReset());
    }
    public void Defend()
    {
        // Check we can do it
        if (!CanPerformAction("Defend")) return;
        // Update Player State
        Debug.Log("PLAYER_STATE: DEFENDING");
        GameManager.CurrentPlayerState = GameManager.PlayerState.Defending;
        // Wait then go to Idle
        StartCoroutine(WaitAndReset());
    }
    public void Magic()
    {
        // Check we can do it
        if (!CanPerformAction("Magic")) return;
        // Wait then go to Idle
        StartCoroutine(WaitAndReset());
    }

    private IEnumerator WaitAndReset()
    {
        yield return new WaitForSeconds(0.5f);
        // Update Player State
        Debug.Log("PLAYER_STATE: IDLE");
        GameManager.CurrentPlayerState = GameManager.PlayerState.Idle;
        yield return null;
    }

    public void Bard()
    {
        // Check we can do it
        if (!CanPerformAction("Bard")) return;
        
        // If not already in bard mode
        if (GameManager.CurrentPlayerState != GameManager.PlayerState.BardMode)
        {
            //Turn on Bard Mode
            _isBardMode = true;
            ToggleBardMode();
            // Update Player State
            Debug.Log("PLAYER_STATE: BARD MODE");
            GameManager.CurrentPlayerState = GameManager.PlayerState.BardMode;
        }
        // If already in bard mode
        else if (GameManager.CurrentPlayerState == GameManager.PlayerState.BardMode)
        {
            //Turn off Bard Mode
            _isBardMode = false;
            ToggleBardMode();
            // Update Player State
            Debug.Log("PLAYER_STATE: IDLE");
            GameManager.CurrentPlayerState = GameManager.PlayerState.Idle;
        }
    }

    private void ToggleBardMode()
    {
        // Enable/Disable Input Map for Bard Mode
        
        if (_isBardMode)
        {
            _playerInput.SwitchCurrentActionMap("Bard");
        }
        else if (!_isBardMode)
        {
            _playerInput.SwitchCurrentActionMap("Gameplay");
        }

    }

    private bool CanPerformAction(string actionType)
    {
        // Player can only do an action if game is not paused
        if (GameManager.CurrentGameState == GameManager.GameState.Paused)
        {
            Debug.Log($"{actionType} cancelled. INVALID: Game is paused.");
            return false;
        }

        // Player can do these actions under specific conditions
        switch (actionType)
        {
            case "Crouch":
                switch (GameManager.CurrentPlayerState)
                {
                    case GameManager.PlayerState.Crouching:
                    case GameManager.PlayerState.Idle:
                        return true;
                    default:
                        Debug.Log($"{actionType} cancelled. INVALID: Player must be crouching or idle.");
                        return false;
                }
            case "Bard":
                switch (GameManager.CurrentPlayerState)
                {
                    case GameManager.PlayerState.BardMode:
                    case GameManager.PlayerState.Idle:
                        return true;
                    default:
                         Debug.Log($"{actionType} cancelled. INVALID: Player must be in bard mode or idle.");
                         return false;
                }
        }

        // Player must be Idle to do other actions
        if (GameManager.CurrentPlayerState != GameManager.PlayerState.Idle)
        {
            Debug.Log($"{actionType} cancelled. INVALID: Player not Idle.");
            return false;
        }

        // Player can only do these if Idle
        switch (actionType)
        {
            case "Attack":
            {
                // Logic TBA
            }
                break;
            case "Defend":
            {
                // Logic TBA
            }
                break;
            case "Magic":
            {
                // Logic TBA
            }
                break;
            
            default:
                Debug.LogError($"Unknown action type: {actionType}");
                return false;
        }
        
        // If the above are okay, action can be performed
        return true;
    }
}
