using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class DT_EnemyManager : MonoBehaviour
{
    [Header("Moving Enemies")]
    [SerializeField] private DT_EnemyGroup[] movingEnemies;

    [Header("Stationary Enemies")] 
    [SerializeField] private GameObject[] stationaryEnemies;

    private bool _canSpawnEnemies;
    
    private List<GameObject> _spawnedEnemies = new List<GameObject>();
    private int _maxListSize = 30;
    private GameManager.PlayerState _oldPlayerState;
    private bool _isSpawnerActive;
    private void Awake()
    {
        // Subscribe to know when game is paused/unpaused
        GameManager.OnGameStateChanged += HandleGameStateChanged;
        GameManager.OnPlayerStateChanged += HandlePlayerStateChanged;
    }

    private void Start()
    {
        // Don't do anything if no enemies at all
        if (stationaryEnemies.Length == 0 && movingEnemies.Length == 0) return;
        
        // Turn on spawner
        ToggleMovingEnemies(true);
    }

    public IEnumerator SpawnEnemies(DT_EnemyGroup group)
    {
        // Don't do anything if no enemies assigned 
        if (group.enemyToSpawn == null) yield return null;
        
        // Repeat forever while true
            while(_isSpawnerActive)
            {
                // Wait for time set (dividing 1 by rate means higher number = more spawning)
                yield return new WaitForSeconds(1/group.spawnRate);
                
                // Pick a random spawn point
                var element = UnityEngine.Random.Range(0, group.spawnPoints.Length);
                var point = group.spawnPoints[element];
                
                // Assign rotation
                Quaternion rotation = default;
                if (group.randomRotation)
                {
                    rotation = UnityEngine.Random.rotation;
                }
                else if (!group.randomRotation)
                {
                    rotation = Quaternion.identity;
                }

                // Spawn an enemy there
                GameObject newEnemy = Instantiate(group.enemyToSpawn, point.transform.position, rotation);
                
                // Set it's move speed and max lifespan
                newEnemy.GetComponent<DT_MovingEnemy>().moveSpeed = group.moveSpeed;
                newEnemy.GetComponent<DT_MovingEnemy>().maxLifeSpan = group.maxLifeSpan;
                
                // Add enemy to a list
                _spawnedEnemies.Add(newEnemy);
                
                // Keep list at less than max list size
                if (_spawnedEnemies.Count > _maxListSize)
                {
                    // See how many we need to remove
                    int elementsToRemove = _spawnedEnemies.Count - _maxListSize;

                    // Remove the specified number, starting with oldest
                    _spawnedEnemies.RemoveRange(0, elementsToRemove);
                }
            }
    }
    
    private void OnDestroy()
    {
        // Unsubscribe when destroyed
        GameManager.OnGameStateChanged -= HandleGameStateChanged;
        GameManager.OnPlayerStateChanged -= HandlePlayerStateChanged;
    }

    public void ToggleMovingEnemies(bool toggle)
    {
        if (movingEnemies.Length == 0) return; 
        Debug.Log($"Moving enemy toggle is {toggle}.");
        if (toggle) //true
        {
            // Start Spawning Enemies
            _isSpawnerActive = true;
            foreach (DT_EnemyGroup group in movingEnemies)
            {
                StartCoroutine(SpawnEnemies(group));
            }
        }
        else
        {
            // Stop Spawning Enemies
            _isSpawnerActive = false;
            
            // Destroy all existing enemies
            if (_spawnedEnemies == null) return;
            // For enemies in the list that have not already self destructed
            foreach (var movingEnemy in _spawnedEnemies)
            {
                if (movingEnemy != null)
                {
                    Destroy(movingEnemy.gameObject);
                }
            }
        }
    }

    public void ToggleStationaryEnemies(bool toggle)
    {
        if (stationaryEnemies.Length == 0) return; 
        Debug.Log($"Stationary enemy toggle is {toggle}.");
        if (toggle) //true
        {
            // Start all Stationary Enemies
            foreach (var enemy in stationaryEnemies)
            {
                enemy.GetComponent<DT_StationaryEnemy>().EnemyToggle(true);
            }
        }
        else
        {
            // Stop all Stationary Enemies
            foreach (var enemy in stationaryEnemies)
            {
                enemy.GetComponent<DT_StationaryEnemy>().EnemyToggle(false);
            }
        }
    }

    private void HandleGameStateChanged(GameManager.GameState newGameState)
    {
        switch (newGameState)
        {
            // If new game state is PAUSED
            case GameManager.GameState.Paused:
            {
                // Toggle enemies off
                ToggleMovingEnemies(false);
                ToggleStationaryEnemies(false);
                break;
            }
            case GameManager.GameState.Playing:
            {
                // Toggle enemies on
                ToggleMovingEnemies(true);
                ToggleStationaryEnemies(true);
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void HandlePlayerStateChanged(GameManager.PlayerState newPlayerState)
    {
        // If player was bard mode but isn't anymore
        if (_oldPlayerState == GameManager.PlayerState.BardMode && newPlayerState != GameManager.PlayerState.BardMode)
        {
            // Maybe change this later to enable/disable colliders?
            // Toggle enemies on
            ToggleMovingEnemies(true);
            ToggleStationaryEnemies(true);
        }

        switch (newPlayerState)
        {
            
            case GameManager.PlayerState.Attacking: // NOTE: Player cannot attack moving Enemies
            {
                if (stationaryEnemies == null) return;
                // Tell stationary enemies that player is attacking
                foreach (var stationaryEnemy in stationaryEnemies)
                {
                    stationaryEnemy.GetComponent<DT_StationaryEnemy>().OnPlayerAttack();
                }
            }
                break;
            case GameManager.PlayerState.BardMode:
            {
                // Toggle enemies off
                ToggleMovingEnemies(false);
                ToggleStationaryEnemies(false);
            }
                break;
            case GameManager.PlayerState.Damaged:
                // This behaviour is handled by DT_MovingEnemy and DT_Stationary_Enemy on each game object
                break;
            case GameManager.PlayerState.Talking:
                // This behaviour is handled by DT_TriggerHandler on LevelManager
                break;
            case GameManager.PlayerState.Crouching:
            // This check is done OnTriggerEnter
                break;
            case GameManager.PlayerState.Defending: 
                // This will be handled in the coroutine in moving enemies
                break;
            case GameManager.PlayerState.Idle:
            case GameManager.PlayerState.Stepping:
            case GameManager.PlayerState.Climbing:
            case GameManager.PlayerState.Magic:
                // No change
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        _oldPlayerState = newPlayerState;
    }

}

[System.Serializable]
public class DT_EnemyGroup
{ 
    public GameObject enemyToSpawn;
    public bool randomRotation;
    [Range(0.1f, 3.0f)] public float spawnRate;
    [Range(0.1f, 1.0f)] public float moveSpeed;
    [Tooltip("(in seconds)")] public float maxLifeSpan;
    public GameObject[] spawnPoints;
}