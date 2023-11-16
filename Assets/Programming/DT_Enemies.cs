using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DT_Enemies : MonoBehaviour
{
    [Header("Enemy Type")] 
    [SerializeField] private EnemyType enemyType;
    
    [Header("Player Interaction")]
    [SerializeField] private bool playerCanAttack;
    [SerializeField] private bool playerCanDefend;
    [SerializeField] private bool crouchIsSafe;
    
    [Header("Damage Player Sound / Effect")]
    [SerializeField] private ParticleSystem playerCollisionParticles;
    [SerializeField] private AudioClip playerCollisionSound;
    
    [Header("Always On Sound / Effects / Damage")]
    [SerializeField] private AudioClip alwaysOnSound;
    [SerializeField] private ParticleSystem alwaysOnParticles;
    [SerializeField] private bool damageAlwaysOn;

    [Header("Stationary Enemy Settings")] 
    [SerializeField] private ParticleSystem intervalParticles;
    [SerializeField] private AudioClip intervalSound;
    //Can make this more complex later:
    [SerializeField] private float onDuration;
    [SerializeField] private float offDuration;
    
    [Header("Spawning Enemy Settings")]
    // These need to go in a spawner
    // [SerializeField] private GameObject objectToSpawn;
   // [SerializeField] private GameObject spawnZone;
   // [SerializeField] private float spawnRate;
   // [SerializeField] private float moveSpeed;
   // [SerializeField] private MovementType moveType;
    [SerializeField] private ParticleSystem otherObjectCollisionParticles;
    [SerializeField] private AudioClip otherObjectCollisionSound;
    [SerializeField] private GameObject[] otherGameObjects;
    
    [Header("Attackable / Defendable Enemy Settings")]
    [SerializeField] private ParticleSystem destroyedParticles;
    [SerializeField] private AudioClip destroyedSound;
    [SerializeField] private AnimationClip destroyedAnimation;
    [SerializeField] private GameObject closeCombatCollider;

    private GameManager _gameManager;
    private GameObject _player;
    private bool _isEnemiesReady;
    private enum MovementType 
    {
        Gravity,
        TowardPlayer
    }
    private enum EnemyType 
    {
        Stationary,
        Spawning
    }


    /* NOTE:
    *  Stationary enemies are:
    *    - Geyser
    *    - Poison bramble
    * 
    *  Spawning enemies are:
    *    - Falling boulders - must hide
    *    - Gusts of Wind - must hide
    *    - Falling icicles - can crouch
    *    - Flying spears - can defend
    *    
    */
    
    void Start()
    {
        // Find the game manager and player
        _gameManager = GameManager.Instance;
        _player = GameObject.FindWithTag("Player");
        
        // Set enemies ready to false
        _isEnemiesReady = false;
    }

    // Update is called once per frame
    void Update()
    {
        // If enemies are ready, do nothing
        if (_isEnemiesReady) return;
        // If game state is not Playing, do nothing
        if (_gameManager.gameState != GameManager.GameState.Playing) return;
        
        
        PrepEnemy();
        _isEnemiesReady = true;
    }

    void PrepEnemy()
    {
        switch (enemyType)
        {
            case EnemyType.Stationary:
                // If player can use attack must listen for.
                   // If can be attacked, need close combat collider
                    // Play sound/particles on successful attack and destroy self
                   // Player cannot defend or crouch against this enemy type

                // If player collision things aren't null, OnTriggerEnter must do them
                
                // If always on sound/particles aren't null, start playing them
                
                // If damage always on, make sure collision is on and OnTriggerEnter causes damage
                
                // If damage NOT always on, turn collision on and off with interval timing
                
                // If interval audio and particles aren't null, turn on and off with interval timing
                
                break;
            case EnemyType.Spawning:
                // If player can use defend or crouch must listen for
                    // If can be defend or crouch, turn off collider when player is doing it
                // Play sound/particles on successful defend and destroy self
                    // Player cannot attack this enemy type
                
                // If player collision things aren't null, OnTriggerEnter must do them
                
                // If always on sound/particles aren't null, start playing them
                
                // If damage always on, make sure collision is on and OnTriggerEnter causes damage
                
                // If other object collision stuff isn't null, OnTriggerEnter must do them
                
                break;
            default:
                Debug.LogError("Enemy Type Not Set");
                break;
        }
    }

    void PrepStationaryEnemy()
    {
        
    }



}
