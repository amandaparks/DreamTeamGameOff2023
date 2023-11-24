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
    //set up sound effect and volume slider
    [SerializeField] private AudioClip intervalSound;
    [Range(0f, 1f)] public float intervalVolume;
    [HideInInspector] public AudioSource intervalAudioSource;
    //Can make this more complex later:
    [SerializeField] private float onDuration;
    [SerializeField] private float offDuration;
    
    [Header("== Damage Player Sound / Effect ==")]
    [SerializeField] private ParticleSystem damageParticles;
    //set up sound effect and volume slider
    [SerializeField] private AudioClip damageSound;
    [Range(0f, 1f)] public float damageVolume;
    [HideInInspector] public AudioSource damageAudioSource;

    [Header("== Attackable Enemy Settings ==")]
    [SerializeField] private bool playerCanAttack;
    [SerializeField] private float maxAttackableDistance;
    [SerializeField] private ParticleSystem destroyedParticles;
    [SerializeField] private Animation destroyedAnimation;
    [SerializeField] private AudioClip destroyedSound;
    [SerializeField] private GameObject objectToEnable;
    [SerializeField] private GameObject objectToDisable;
    [Range(0f, 1f)] public float destroyedVolume;
    [HideInInspector] public AudioSource destroyedAudioSource;

    [Header("== Always-On Sound / Effects ==")]
    [SerializeField] private AudioClip alwaysOnSound;
    [SerializeField] private ParticleSystem alwaysOnParticles;
    [HideInInspector] public AudioSource alwaysOnAudioSource;
    
    private GameObject _player;
    private Collider _damageCollider;
    private DT_PlayerDamage _playerDamage;
    private bool _isEnemyActive;

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
        alwaysOnAudioSource = gameObject.AddComponent<AudioSource>();
        damageAudioSource = gameObject.AddComponent<AudioSource>();
        intervalAudioSource = gameObject.AddComponent<AudioSource>();
        destroyedAudioSource = gameObject.AddComponent<AudioSource>();
        
        if (intervalSound != null)
        {
            intervalAudioSource.clip = intervalSound;
            intervalAudioSource.volume = intervalVolume;
        }

        EnemyToggle(true);
    }
    
    public void OnPlayerAttack()
    {
        // If not attackable enemy, ignore
        if (!playerCanAttack) return;
        
        // Calculate distance from player
        float distance = Vector3.Distance(_player.transform.position, transform.position);

        // If player is close enough
        if (distance <= maxAttackableDistance)
        {
            
            if (destroyedAnimation != null)
            {
                destroyedAnimation.Play();
            }
            
            if (objectToEnable != null)
            {
                Debug.Log($"Enabling {objectToEnable}");
                objectToEnable.SetActive(true);
            }
            if (objectToDisable != null)
            {
                Debug.Log($"Disabling {objectToDisable}");
                objectToDisable.SetActive(false);
            }
            
            if (destroyedParticles != null)
            {
                destroyedParticles.Play();
            }
            if (destroyedSound != null)
            {
                destroyedAudioSource.Play();
            }
            
            damageCollider.enabled = false;
            damageCollider.isTrigger = false;

            if (objectToDisable == null)
            {
                gameObject.SetActive(false);
            }
        }
    }

  

    public void EnemyToggle(bool isEnemyActive)
    {
        // Start/stop enemy behaviour if true/false
        if (isEnemyActive)
        {
            AlwaysOn(true);
            if (damageType != DamageType.interval) return;
            _isEnemyActive = true;
            StartCoroutine(IntervalDamage());
        }
        else
        {
            AlwaysOn(false);
            if (damageType != DamageType.interval) return;
            Debug.Log("Stopping Coroutine Interval Damage");
            _isEnemyActive = false;
            TurnComponentsOff();
        }
    }

    
    private IEnumerator IntervalDamage()
    {
        

        // runs while enemy is allowed to be active
        while (_isEnemyActive)
        {
            TurnComponentsOn();
            
            // Wait for duration
            yield return new WaitForSeconds(onDuration);
            
            TurnComponentsOff();
            
            // Wait for duration
            yield return new WaitForSeconds(offDuration);
        }
    }

    private void TurnComponentsOn()
    {
        // Turn everything on
        intervalParticles.Play();
        if (intervalSound != null)
        {
            intervalAudioSource.Play();
        }
        damageCollider.enabled = true;
        damageCollider.isTrigger = true;
    }

    private void TurnComponentsOff()
    {
        // Turn everything off
        intervalParticles.Stop();
        if (intervalSound != null)
        {
            intervalAudioSource.Stop();
        }
        damageCollider.enabled = false;
        damageCollider.isTrigger = false;
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
            Debug.Log("particles and audio should be on.");
            
        }
        else //false
        {
            if (alwaysOnParticles != null)
            {alwaysOnParticles.Stop();}
            if (alwaysOnSound == null) return;
            alwaysOnAudioSource.Stop();
            Debug.Log("particles and audio should be stopped.");
        }

        if (damageType != DamageType.constant) return;
        Debug.Log("This should not show.");
        damageCollider.enabled = toggle;
        damageCollider.isTrigger = toggle;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Don't damage already damaged player
            if (GameManager.CurrentPlayerState == GameManager.PlayerState.Damaged) return;
            
            // Cause damage to player
            _playerDamage.DamagePlayer();
            
            // Do effects and sound
            if (damageParticles != null)
            {damageParticles.Play();}
            if (damageSound == null) return;
            damageAudioSource.clip = damageSound;
            damageAudioSource.volume = damageVolume;
            damageAudioSource.Play();
        }
    }
}
