using UnityEngine;

public class DT_PlayerAnimations : MonoBehaviour
{
    private GameManager _gameManager;
    private GameManager.PlayerState _currentState;
    [SerializeField] private Animator spriteAnimator;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        // Find the Game Manager and assign it to this variable
        _gameManager = GameManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        // If player state hasn't changed, do nothing
        if (_currentState == _gameManager.playerState) return;

        // Otherwise, change animation and update current state
        ChangeAnimation(_gameManager.playerState);
        _currentState = _gameManager.playerState;
    }

    private void ChangeAnimation(GameManager.PlayerState gameManagerPlayerState)
    {
        switch (_gameManager.playerState)
        {
            case GameManager.PlayerState.Idle:
                spriteAnimator.SetTrigger("Idle");
                break;
            case GameManager.PlayerState.Stepping:
                //If player going right, don't flip sprite
                if (DT_PlayerMovement.targetDirection == DT_PlayerMovement.TargetDirection.Right)
                {
                    _spriteRenderer.flipX = false;
                }
                //If player going left, flip sprite
                else if (DT_PlayerMovement.targetDirection == DT_PlayerMovement.TargetDirection.Left)
                {
                    _spriteRenderer.flipX = true;
                }
                spriteAnimator.SetTrigger("Step");
                break;
            case GameManager.PlayerState.Climbing:
                spriteAnimator.SetTrigger("Climb");
                break;
            case GameManager.PlayerState.Crouching:
                spriteAnimator.SetTrigger("Crouch");
                break;
        }
    }
}
