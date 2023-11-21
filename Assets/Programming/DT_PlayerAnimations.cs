using UnityEngine;

public class DT_PlayerAnimations : MonoBehaviour
{
    private GameManager _gameManager;
    private GameManager.PlayerState _currentState;
    [SerializeField] private Animator spriteAnimator;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    private static readonly int Damaged = Animator.StringToHash("Damaged");
    private static readonly int Crouch = Animator.StringToHash("Crouch");
    private static readonly int Climb = Animator.StringToHash("Climb");
    private static readonly int Step = Animator.StringToHash("Step");
    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int Bard = Animator.StringToHash("Bard");
    private static readonly int Magic = Animator.StringToHash("Magic");

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
        if (_currentState == GameManager.CurrentPlayerState) return;

        // Otherwise, change animation and update current state
        ChangeAnimation(GameManager.CurrentPlayerState);
        _currentState = GameManager.CurrentPlayerState;
    }

    private void ChangeAnimation(GameManager.PlayerState gameManagerPlayerState)
    {
        switch (GameManager.CurrentPlayerState)
        {
            case GameManager.PlayerState.Idle:
                spriteAnimator.SetTrigger(Idle);
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
                spriteAnimator.SetTrigger(Step);
                break;
            case GameManager.PlayerState.Climbing:
                spriteAnimator.SetTrigger(Climb);
                break;
            case GameManager.PlayerState.Crouching:
                spriteAnimator.SetTrigger(Crouch);
                break;
            case GameManager.PlayerState.Damaged:
                spriteAnimator.SetTrigger(Damaged);
                break;
            case GameManager.PlayerState.BardMode:
                spriteAnimator.SetTrigger(Bard);
                break;
            case GameManager.PlayerState.Attacking: 
            case GameManager.PlayerState.Defending:
            case GameManager.PlayerState.Magic:
                spriteAnimator.SetTrigger(Magic);
                break;
        }
    }
}
