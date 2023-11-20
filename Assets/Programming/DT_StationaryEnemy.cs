using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DT_StationaryEnemy : MonoBehaviour
{
    [Header("== Enemy Behaviour ==")]
    [SerializeField] private Collider damageCollider;
    [SerializeField] private DamageType damageType;
    private enum DamageType
    {
        constant,
        interval
    }
    [Header("Interval Options")]
    [SerializeField] private ParticleSystem intervalParticles;
    [SerializeField] private AudioClip intervalSound;
    [HideInInspector] public AudioSource intervalAudioSource;
    //Can make this more complex later:
    [SerializeField] private float onDuration;
    [SerializeField] private float offDuration;
    
    [Header("== Damage Player Sound / Effect ==")]
    [SerializeField] private ParticleSystem damageParticles;
    [SerializeField] private AudioClip damageSound;
    [HideInInspector] public AudioSource damageAudioSource;

    [Header("== Attackable Enemy Settings ==")]
    [SerializeField] private bool playerCanAttack;
    [SerializeField] private float maxAttackableDistance;
    [SerializeField] private ParticleSystem destroyedParticles;
    [SerializeField] private AudioClip destroyedSound;
    [HideInInspector] public AudioSource destroyedAudioSource;

    [Header("== Always-On Sound / Effects ==")]
    [SerializeField] private AudioClip alwaysOnSound;
    [SerializeField] private ParticleSystem alwaysOnParticles;
    [HideInInspector] public AudioSource alwaysOnAudioSource;
    
    private GameObject _player;
    private bool _isEnemyActive;
    private Collider _damageCollider;
    private DT_PlayerDamage _playerDamage;

    /* NOTE:
    *  Stationary enemies are:
    *    - Geyser
    *    - Poison bramble
    */
    
    void Start()
    {
        // Find the player and scripts
        _player = GameObject.FindWithTag("Player");
        _playerDamage = _player.GetComponent<DT_PlayerDamage>();
        
        // Instantiate audio sources
        alwaysOnAudioSource = new GameObject().AddComponent<AudioSource>();
        damageAudioSource = new GameObject().AddComponent<AudioSource>();
        intervalAudioSource = new GameObject().AddComponent<AudioSource>();
        destroyedAudioSource = new GameObject().AddComponent<AudioSource>();

        // Subscribe to know when game is paused/unpaused
        GameManager.OnGameStateChanged += HandleGameStateChanged;

        // Start based on the current game state
        HandleGameStateChanged(GameManager.CurrentGameState);
        
        EnemyToggle(true);
    }
    
    private void OnDestroy()
    {
        // Unsubscribe when destroyed
        GameManager.OnGameStateChanged -= HandleGameStateChanged;
    }
    
    private void HandleGameStateChanged(GameManager.GameState newGameState)
    {
        switch (newGameState)
        {
            case GameManager.GameState.Paused:
                if (_isEnemyActive)
                {
                    EnemyToggle(false);
                }
                break;
            case GameManager.GameState.Playing:
                if (!_isEnemyActive)
                {
                    EnemyToggle(true);
                }
                break;
        }
    }

    void EnemyToggle(bool isEnemyActive)
    {
        // Start/stop enemy behaviour if true/false
        if (isEnemyActive)
        {
            AlwaysOn(true);
            _isEnemyActive = true;
            if (damageType != DamageType.interval) return;
            StartCoroutine(IntervalDamage());
        }
        else
        {
            AlwaysOn(false);
            _isEnemyActive = false;
            if (damageType != DamageType.interval) return;
            StopCoroutine(IntervalDamage());
        }
    }

    
    private IEnumerator IntervalDamage()
    {
        if (intervalSound != null)
            intervalAudioSource.clip = intervalSound;

        // while true runs forever
        while (true)
        {
            // Turn everything on
            intervalParticles.Play();
            if (intervalSound != null)
            {
                intervalAudioSource.Play();
            }
            damageCollider.enabled = true;
            damageCollider.isTrigger = true;
            
            // Wait for duration
            yield return new WaitForSeconds(onDuration);
            
            // Turn everything off
            intervalParticles.Stop();
            if (intervalSound != null)
            {
                intervalAudioSource.Stop();
            }
            damageCollider.enabled = false;
            damageCollider.isTrigger = false;
            
            // Wait for duration
            yield return new WaitForSeconds(offDuration);
        }
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

        if (damageType != DamageType.constant) return;
        
        damageCollider.enabled = toggle;
        damageCollider.isTrigger = toggle;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Cause damage to player
            _playerDamage.DamagePlayer();
            
            // Do effects and sound
            if (damageParticles != null)
            {damageParticles.Play();}
            if (damageSound == null) return;
            damageAudioSource.clip = damageSound;
            damageAudioSource.Play();
        }
    }
}
