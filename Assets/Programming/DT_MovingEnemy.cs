using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DT_MovingEnemy : MonoBehaviour
{
    [Header("== Enemy Behaviour ==")]
    [SerializeField] private Collider damageCollider;
    [SerializeField] private MovementType moveType;
    private enum MovementType
    {
        fallStraightDown,
        screenLeftToRight,
        shootTowardPlayer
    }
    [HideInInspector] public float moveSpeed;
    [HideInInspector] public float maxLifeSpan;
    
    
    [Header("== Player Interaction ==")]
    //[SerializeField] private bool playerCanDefend;
    [SerializeField] private bool crouchIsSafe;
    
    [Header("== Damage Player Sound / Effect ==")]
    [SerializeField] private ParticleSystem damageParticles;
    [SerializeField] private AudioClip damageSound;
    [HideInInspector] public AudioSource damageAudioSource;
    
    [Header("== Always-On Sound / Effects ==")]
    [SerializeField] private AudioClip alwaysOnSound;
    [SerializeField] private ParticleSystem alwaysOnParticles;
    [HideInInspector] public AudioSource alwaysOnAudioSource;
    
    [Header("== Collide Environment Settings ==")]
    [SerializeField] private ParticleSystem collisionParticles;
    [SerializeField] private AudioClip collisionSound;
    [HideInInspector] public AudioSource collisionAudioSource;
    [SerializeField] private string environmentTag;
    
    [Header("== Defendable Enemy Settings ==")]
    [SerializeField] private bool playerCanDefend;
    [SerializeField] private ParticleSystem disabledParticles;
    [SerializeField] private AudioClip disabledSound;
    [HideInInspector] public AudioSource disabledAudioSource;
    
    private GameObject _player;
    private bool _isEnemyActive;
    private bool _canCoroutineRun;
    private Collider _damageCollider;
    private DT_PlayerDamage _playerDamage;
    private float _minMoveDistance = 100f; // hard coding to save time

    // For use in calculating targets
    [SerializeField] private Vector3 _startPoint;
    [SerializeField] private Vector3 _endPoint;
    [SerializeField] private Vector3 _playerPosition;
    [SerializeField] private Vector3 _enemyStartPosition;
    [SerializeField] private float _distance;
    
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
        Destroy(gameObject,maxLifeSpan);
    }

    private void AssignTargets(MovementType type)
    {
        _playerPosition = _player.transform.position;
        _enemyStartPosition = transform.position;
        
        switch (type)
        {
            case MovementType.fallStraightDown:
                _startPoint = _enemyStartPosition;
                // End point is further down on the Y axis
                _endPoint = new Vector3(_enemyStartPosition.x, _enemyStartPosition.y - _minMoveDistance, _enemyStartPosition.z);
                // Distance
                _distance = Vector3.Distance(_startPoint, _endPoint);
                break;
            case MovementType.screenLeftToRight:
                _startPoint = transform.position;
                _startPoint = _enemyStartPosition;
                // End point is further left on the X axis
                _endPoint = new Vector3(_enemyStartPosition.x - _minMoveDistance, _enemyStartPosition.y, _enemyStartPosition.z);
                // Distance
                _distance = Vector3.Distance(_startPoint, _endPoint);
                break;
            case MovementType.shootTowardPlayer:
                _startPoint = transform.position;
                var playerPosition = _player.transform.position;
                // Work out direction to player
                Vector3 direction = (playerPosition - _startPoint).normalized;
                // Set end point somewhere past that
                _endPoint = playerPosition + direction * _minMoveDistance;
                break;
            default:
                throw new ArgumentOutOfRangeException();
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

        while (elapsedTime < (5f/moveSpeed))
        {
            if (_canCoroutineRun)
            {
                // Move enemy in assigned direction
                transform.position = Vector3.Lerp(_startPoint, _endPoint, elapsedTime / (5f/moveSpeed));

                // Increment for next frame
                elapsedTime += Time.deltaTime;

                // Wait for next frame
                yield return null;
            }
        }
        
        yield return null;
    }

    private void AlwaysOn(bool toggle)
    {
        if (toggle) //true
        {   
            if (alwaysOnParticles != null)
            {alwaysOnParticles.Play();}
            if (alwaysOnSound == null) return;
            alwaysOnAudioSource.clip = alwaysOnSound;
            alwaysOnAudioSource.Play();
            
        }
        else //false
        {
            if (alwaysOnParticles != null)
            {alwaysOnParticles.Stop();}
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
            // Don't damage already damaged player, or player in Bard Mode
            if (GameManager.CurrentPlayerState == GameManager.PlayerState.Damaged) return;
            if (GameManager.CurrentPlayerState == GameManager.PlayerState.BardMode) return;
            
            // Cause damage to player (OnPlayerCrouch handles crouching)
            _playerDamage.DamagePlayer();
            
            // Do effects and sound
            if (damageParticles != null)
            {damageParticles.Play();}
            if (damageSound == null) return;
            damageAudioSource.clip = damageSound;
            damageAudioSource.Play();
            
            // Destroy yoself
            Destroy(gameObject);
        }

        if (other.CompareTag(environmentTag))
        {
            // Do effects and sound
            if (collisionParticles != null)
            {collisionParticles.Play();}
            if (collisionSound == null) return;
            collisionAudioSource.clip = damageSound;
            collisionAudioSource.Play();
            
            // Destroy yoself
            Destroy(gameObject);
        }
    }
    
    public void OnPlayerDefend()
    {
        // If not defendable enemy, ignore
        if (!playerCanDefend) return;
        
        if (disabledParticles != null)
        {disabledParticles.Play();}
        if (disabledSound != null)
        {
            disabledAudioSource.Play();
        }
        damageCollider.enabled = false;
        damageCollider.isTrigger = false;
            
        // Destroy yoself
        Destroy(gameObject);
    }

    public void OnPlayerCrouch(bool isCrouching)
    {
        // Ignore if player isn't protected by crouching
        if (!crouchIsSafe) return;
        
        // Disable damage collider while player is crouching
        if (isCrouching)
        {
            damageCollider.enabled = false;
            damageCollider.isTrigger = false;
        }
        else
        {
            damageCollider.enabled = true;
            damageCollider.isTrigger = true;
        }
    }

}
