using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = System.Random;

public class DT_MovingEnemy : MonoBehaviour
{
    [Header("== Enemy Behaviour ==")] [SerializeField]
    private Collider damageCollider;

    [SerializeField] private MovementType moveType;

    private enum MovementType
    {
        fallStraightDown,
        screenRightToLeft,
        shootTowardPlayer
    }

    [HideInInspector] public float moveSpeed;
    [HideInInspector] public float maxLifeSpan = 5f;


    [Header("== Player Interaction ==")]
    //[SerializeField] private bool playerCanDefend;
    [SerializeField]
    private bool crouchIsSafe;

    [Header("== Damage Player Sound / Effect ==")] [SerializeField]
    private ParticleSystem damageParticles;

    [SerializeField] private AudioClip damageSound;
    [Range(0f, 1f)] public float damageVolume;
    [HideInInspector] public AudioSource damageAudioSource;

    [Header("== Always-On Sound / Effects ==")] [SerializeField]
    private AudioClip alwaysOnSound;
    [Range(0f, 1f)] public float alwaysOnVolume;

    [SerializeField] private ParticleSystem alwaysOnParticles;
    [HideInInspector] public AudioSource alwaysOnAudioSource;

    [Header("== Collide Environment Settings ==")] [SerializeField]
    private Animation collisionAnimation;

    [SerializeField] private ParticleSystem collisionParticles;
    [SerializeField] private AudioClip collisionSound;
    [Range(0f, 1f)] public float collisionVolume;
    [HideInInspector] public AudioSource collisionAudioSource;
    [SerializeField] private string environmentTag;
    [SerializeField] private bool dropOnCollide;

    [Header("== Defendable Enemy Settings ==")] [SerializeField]
    private bool playerCanDefend;

    [SerializeField] private ParticleSystem disabledParticles;
    [SerializeField] private AudioClip disabledSound;
    [Range(0f, 1f)] public float disabledVolume;
    [HideInInspector] public AudioSource disabledAudioSource;

    private GameObject _player;
    private bool _isEnemyActive;
    private bool _canCoroutineRun;
    private Collider _damageCollider;
    private DT_PlayerDamage _playerDamage;
    private GameManager.PlayerState _oldPlayerState = GameManager.PlayerState.Idle; // Initialise as idle
    private GameManager.GameState _previousGameState = GameManager.GameState.Playing; // Initialise as playing
    public float maxMoveDistance;

    // For use in calculating targets
    // Serialized for debugging
    [SerializeField] private Vector3 _startPoint;
    [SerializeField] private Vector3 _endPoint;
    [SerializeField] private Vector3 _playerPosition;

    [SerializeField] private Vector3 _enemyStartPosition;

    // [SerializeField] private float _distance;
    [SerializeField] private float _distanceFromStart;

    /* NOTE:
  *  Moving enemies are:
  *    - Falling rocks
  *    - Wind
  *    - Falling ice
  *    - Shooting arrows 
  */

    private void EnemyToggle(bool isEnemyActive)
    {
        // Start/stop enemy behaviour if true/false
        if (isEnemyActive)
        {
            AlwaysOn(true);
            // This bool resumes coroutine
            _canCoroutineRun = true;
        }
        else
        {
            AlwaysOn(false);
            // This bool pauses coroutine
            _canCoroutineRun = false;
        }
    }



    void OnEnable()
    {
        // Find the player and scripts
        _player = GameObject.FindWithTag("Player");
        _playerDamage = _player.GetComponent<DT_PlayerDamage>();

        // Instantiate audio sources
        alwaysOnAudioSource = gameObject.AddComponent<AudioSource>();
        damageAudioSource = gameObject.AddComponent<AudioSource>();
        collisionAudioSource = gameObject.AddComponent<AudioSource>();
        disabledAudioSource = gameObject.AddComponent<AudioSource>();

        // Assign targets depending on move type
        AssignTargets(moveType);

        // Turn on
        EnemyToggle(true);

        // Start moving
        _canCoroutineRun = true;
        StartCoroutine(Move());

        // Start self-destruct countdown
        Destroy(gameObject, maxLifeSpan);
    }

    private void AssignTargets(MovementType type)
    {
        _enemyStartPosition = transform.position;
        _startPoint = _enemyStartPosition;
        _playerPosition = _player.transform.position;
        // if max move distance hasn't been set, set it to 100
        if (maxMoveDistance == 0)
        {
            maxMoveDistance = 100f;
        }

        switch (type)
        {
            case MovementType.fallStraightDown:
                // End point is further down on the Y axis
                _endPoint = new Vector3(_enemyStartPosition.x, _enemyStartPosition.y - maxMoveDistance,
                    _enemyStartPosition.z);
                break;
            case MovementType.screenRightToLeft:
                // End point is further left on the X axis
                _endPoint = new Vector3(_enemyStartPosition.x - maxMoveDistance, _enemyStartPosition.y,
                    _enemyStartPosition.z);
                break;
            case MovementType.shootTowardPlayer:
                // Work out direction to player
                Vector3 direction = (_playerPosition - _startPoint).normalized;
                // Set end point somewhere past that
                _endPoint = _playerPosition + direction * maxMoveDistance;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void Update()
    {
        // If game state has changed
        if (_previousGameState != GameManager.CurrentGameState)
        {
            switch (GameManager.CurrentGameState)
            {
                case GameManager.GameState.Playing:
                    EnemyToggle(true);
                    break;
                case GameManager.GameState.Paused:
                    EnemyToggle(false);
                    break;
            }

            _previousGameState = GameManager.CurrentGameState;
        }

        // If player state has changed
        if (_oldPlayerState != GameManager.CurrentPlayerState)
        {
            if (_oldPlayerState == GameManager.PlayerState.BardMode &&
                GameManager.CurrentPlayerState != GameManager.PlayerState.BardMode)
            {
                EnemyToggle(true);
            }

            switch (GameManager.CurrentPlayerState)
            {
                case GameManager.PlayerState.Defending:
                    if (playerCanDefend)
                    {
                        OnPlayerDefend();
                    }

                    break;
                case GameManager.PlayerState.BardMode:
                    EnemyToggle(false);
                    break;
            }

            _oldPlayerState = GameManager.CurrentPlayerState;

        }
    }


    private IEnumerator Move()
    {
        if (!_canCoroutineRun) // Pause coroutine
        {
            yield return null;
        }

        // Start counting
        float elapsedTime = 0f;

        while (elapsedTime < (5f / moveSpeed))
        {
            if (!_canCoroutineRun) yield return null; // skip the rest of the loop

            // Calculate distance travelled (Math.Abs = positive value)
            _distanceFromStart = Math.Abs(Vector3.Distance(_startPoint, transform.position));

            // If current distance is equal to or greater than max distance, disable and drop
            if (_distanceFromStart >= maxMoveDistance)
            {
                StartCoroutine(DisableWaitDrop(0f));
                yield break;
            }

            // Move enemy in assigned direction
            transform.position = Vector3.Lerp(_startPoint, _endPoint, elapsedTime / (5f / moveSpeed));

            // If it's supposed to be shooting toward the player, make object face the player
            if (moveType == MovementType.shootTowardPlayer)
            {
                transform.LookAt(_player.transform.position);
            }

            // Increment for next frame
            elapsedTime += Time.deltaTime;

            // Wait for next frame
            yield return null;
        }

        yield return null;
    }

    private void AlwaysOn(bool toggle)
    {
        if (toggle) //true
        {
            if (alwaysOnParticles != null)
            {
                alwaysOnParticles.Play();
            }

            if (alwaysOnSound == null) return;
            alwaysOnAudioSource.clip = alwaysOnSound;
            alwaysOnAudioSource.volume = alwaysOnVolume;
            alwaysOnAudioSource.Play();

        }
        else //false
        {
            if (alwaysOnParticles != null)
            {
                alwaysOnParticles.Stop();
            }

            if (alwaysOnSound == null) return;
            alwaysOnAudioSource.Stop();
        }

        damageCollider.enabled = toggle;
        damageCollider.isTrigger = toggle;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Don't damage already damaged player, or player in Bard Mode, or player Crouching & Crouching is safe
            if (GameManager.CurrentPlayerState == GameManager.PlayerState.Damaged) return;
            if (GameManager.CurrentPlayerState == GameManager.PlayerState.BardMode) return;
            if (crouchIsSafe && GameManager.CurrentPlayerState == GameManager.PlayerState.Crouching) return;

            // Cause damage to player (OnPlayerCrouch handles crouching)
            _playerDamage.DamagePlayer();

            // Do effects and sound
            if (damageParticles != null)
            {
                damageParticles.Play();
            }

            if (damageSound == null) return;
            damageAudioSource.clip = damageSound;
            damageAudioSource.volume = damageVolume;
            damageAudioSource.Play();
        }

        if (string.IsNullOrEmpty(environmentTag)) return;
        if (other.CompareTag(environmentTag))
        {
            // Do effects and sound
            if (collisionParticles != null)
            {
                collisionParticles.Play();
            }

            if (collisionSound != null)
            {
                collisionAudioSource.clip = collisionSound;
                collisionAudioSource.Play();
            }

            if (collisionAnimation != null)
            {
                collisionAnimation.Play();
            }

            if (dropOnCollide)
            {
                StartCoroutine(DisableWaitDrop(0f));
            }
            else
            {
                // Just turn off collider
                damageCollider.enabled = false;
                damageCollider.isTrigger = false;
            }
        }
    }

    private void OnPlayerDefend()
    {
        // If not defendable enemy, ignore
        if (!playerCanDefend) return;

        if (disabledParticles != null)
        {
            disabledParticles.Play();
        }

        if (disabledSound != null)
        {
            disabledAudioSource.clip = disabledSound;
            disabledAudioSource.volume = disabledVolume;
            disabledAudioSource.Play();
        }

        StartCoroutine(DisableWaitDrop(0.5f));
    }

    private IEnumerator DisableWaitDrop(float seconds)
    {
        // Turn off collider
        damageCollider.enabled = false;
        damageCollider.isTrigger = false;

        // Stop running coroutine
        _canCoroutineRun = false;

        // Allow Unity to catch up
        yield return new WaitForSeconds(seconds);

        // Send straight down
        StartCoroutine(Fall());

    }

    private IEnumerator Fall()
    {
        _startPoint = transform.position;
        _endPoint = new Vector3(_startPoint.x, _startPoint.y - 30, _startPoint.z);

        // Set speed quite high
        moveSpeed = 1;
        
        // Start counting
        float elapsedTime = 0f;

        // 
        while (elapsedTime < (5f / moveSpeed))
        {
            
            // Move enemy in assigned direction
            transform.position = Vector3.Lerp(_startPoint, _endPoint, elapsedTime / (5f / moveSpeed));

            // Increment for next frame
            elapsedTime += Time.deltaTime;

            // Wait for next frame
            yield return null;
        }
    }
}


