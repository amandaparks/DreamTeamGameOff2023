using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DT_PlayerActions : MonoBehaviour
{
    [Header("PLAYER ACTION SETTINGS")]
    [Header(" -Script controls crouching, magic etc.")]
    [Space]
    [SerializeField] private GameObject _iceCircle;
    [SerializeField] private GameObject _vineCircle;
    [SerializeField] private GameObject _lightCircle;
    
    
    [SerializeField] private float _magicTime = 1f;
    private GameManager _gameManager;
    private bool _isBardMode;
    private DT_InputManager _inputManager;
    private DT_GameTextManager _gameTextManager;
    private GameObject _bardEffects;
    private AudioManager _audioManager;
<<<<<<< HEAD
    private MusicManager _musicManager;


=======
    private GameObject _spell;
    
>>>>>>> ccf2254a12370669de382b97ad2f8c8d2d9e33f8
    // Start is called before the first frame update
    void Start()
    {
        
        
        //Find game text manager
        _gameTextManager = FindObjectOfType<DT_GameTextManager>();
        
        //Find input manager
        _inputManager = GetComponent<DT_InputManager>();
        
        // Find the Audio Manager
        _audioManager = FindObjectOfType<AudioManager>();

        // Find the Music Manager
        _musicManager = FindObjectOfType<MusicManager>();
        if (_musicManager == null)
        {
            Debug.LogError("No Music Manager Found");
        }


        //Find bard effects
        _bardEffects = GameObject.FindGameObjectWithTag("BardMode");
        //Bard mode off by default
        _isBardMode = false;
        _bardEffects.SetActive(false);

        //Turn off magic circles
        _iceCircle.SetActive(false);
        _vineCircle.SetActive(false);
        _lightCircle.SetActive(false);
        
        
    }

    public void Next()
    {
        // Check we can do it
        if (!CanPerformAction("Next")) return;
        // Trigger next line of text
        _gameTextManager.NextLine();
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
            _audioManager.PlayKalimba("2");
            GameManager.CurrentPlayerState = GameManager.PlayerState.Crouching;
        }
        // If already crouching
        else if (GameManager.CurrentPlayerState == GameManager.PlayerState.Crouching)
        {
            // Update Player State
            Debug.Log("PLAYER_STATE: IDLE");
            _audioManager.PlayKalimba("2");
            GameManager.CurrentPlayerState = GameManager.PlayerState.Idle;
        }
    }
    public void Attack()
    {
        // Check we can do it
        if (!CanPerformAction("Attack")) return;
        // Update Player State
        Debug.Log("PLAYER_STATE: ATTACKING");
        _audioManager.PlayKalimba("6");
        GameManager.CurrentPlayerState = GameManager.PlayerState.Attacking;
        
        // Assign spell
        _spell = _iceCircle;
        
        // Do magic then go to Idle
        StartCoroutine(DoMagic());
    }
    public void Defend()
    {
        // Check we can do it
        if (!CanPerformAction("Defend")) return;
        // Update Player State
        Debug.Log("PLAYER_STATE: DEFENDING");
        _audioManager.PlayKalimba("1");
        GameManager.CurrentPlayerState = GameManager.PlayerState.Defending;
        
        // Assign spell
        _spell = _vineCircle;
        
        // Do magic then go to Idle
        StartCoroutine(DoMagic());
    }
    public void Magic()
    {
        // Check we can do it
        if (!CanPerformAction("Magic")) return;
        // Update Player State
        Debug.Log("PLAYER_STATE: MAGIC");
        _audioManager.PlayKalimba("7");
        GameManager.CurrentPlayerState = GameManager.PlayerState.Magic;
        
        // Assign spell
        _spell = _lightCircle;
        
        // Do magic then go to Idle
        StartCoroutine(DoMagic());
    }

    private IEnumerator DoMagic()
    {
        _spell.SetActive(true);
        yield return new WaitForSeconds(_magicTime);
        _spell.SetActive(false);
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
            _bardEffects.SetActive(true);
            ToggleBardMode();
            // Update Player State
            Debug.Log("PLAYER_STATE: BARD MODE");
            _audioManager.PlayKalimba("B");
            GameManager.CurrentPlayerState = GameManager.PlayerState.BardMode;
            // Turn down BGM
            Debug.Log("Calling BardModeMusic");
            _musicManager.BardModeMusic();
        }
        // If already in bard mode
        else if (GameManager.CurrentPlayerState == GameManager.PlayerState.BardMode)
        {
            //Turn off Bard Mode
            _isBardMode = false;
            _bardEffects.SetActive(false);
            ToggleBardMode();
            // Update Player State
            Debug.Log("PLAYER_STATE: IDLE");
            _audioManager.PlayKalimba("B");
            GameManager.CurrentPlayerState = GameManager.PlayerState.Idle;
            // Turn BGM back up
            _musicManager.BardModeRestoreMusic();
        }
    }

    private void ToggleBardMode()
    {
        // Enable/Disable Input Map for Bard Mode
        
        if (_isBardMode)
        {
            _inputManager.SwitchActionMap("Bard");
        }
        else if (!_isBardMode)
        {
            _inputManager.SwitchActionMap("Gameplay");
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
                        Debug.Log($"FAIL: Player must be crouching or idle.");
                        _audioManager.PlayKalimba("fail");
                        return false;
                }
            case "Bard":
                switch (GameManager.CurrentPlayerState)
                {
                    case GameManager.PlayerState.BardMode:
                    case GameManager.PlayerState.Idle:
                        return true;
                    default:
                         Debug.Log($"FAIL: Player must be in bard mode or idle.");
                         _audioManager.PlayKalimba("fail");
                         return false;
                }
            case "Next":
                {
                    // return the result of the statement "current state = talking"
                    return GameManager.CurrentPlayerState == GameManager.PlayerState.Talking;
                }
        }

        // Player must be Idle to do other actions, if not idle = FAIL
        if (GameManager.CurrentPlayerState != GameManager.PlayerState.Idle)
        {
            Debug.Log($"FAIL: Player not Idle.");
            _audioManager.PlayKalimba("fail");
            return false;
        }

        // Space for extra conditions:
        switch (actionType)
        {
            case "Attack":
            {
                // NA
            }
                break;
            case "Defend":
            {
                // NA
            }
                break;
            case "Magic":
            {
                // NA
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
