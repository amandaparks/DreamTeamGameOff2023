using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DT_EnemyManager : MonoBehaviour
{
    [Header("Moving Enemies")] 
    [SerializeField] private DT_EnemyGroup[] enemyGroup;

    [Header("Stationary Enemies")] 
    [SerializeField] private GameObject[] stationaryEnemies;

    private bool _canSpawnEnemies;
    
    private List<GameObject> _spawnedEnemies = new List<GameObject>();
    private int _maxListSize;
    private GameManager.PlayerState _oldPlayerState;
    private void Awake()
    {
        // Subscribe to know when game is paused/unpaused
        GameManager.OnGameStateChanged += HandleGameStateChanged;
        GameManager.OnPlayerStateChanged += HandlePlayerStateChanged;
    }

    private void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    public IEnumerator SpawnEnemies()
    {
        foreach (DT_EnemyGroup group in enemyGroup)
        {
            // Repeat forever
            while(true)
            {
                // Wait for time set
                yield return new WaitForSeconds(group.spawnRate);
                
                // Pick a random spawn point
                var element = UnityEngine.Random.Range(0, group.spawnPoints.Length);
                var point = group.spawnPoints[element];
                
                // Spawn an enemy there
                GameObject newEnemy = Instantiate(group.enemyToSpawn, point.transform.position, Quaternion.identity);
                
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
    }

    public void UpdateEnemies()
    {
        // For each enemy in the list that is not empty, do a thing
        foreach (var enemy in _spawnedEnemies.Where(enemy => !enemy))
        {
            //do a thing
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
        if (toggle) //true
        {
            // Start Spawning Enemies
            StartCoroutine(SpawnEnemies());
                
            // Restart all Moving Enemies
            foreach (var movingEnemy in _spawnedEnemies)
            {
                movingEnemy.GetComponent<DT_MovingEnemy>().EnemyToggle(true);
            }
        }
        else
        {
            // Stop Spawning Enemies
            StopCoroutine(SpawnEnemies());
                
            // Stop all Moving Enemies
            foreach (var movingEnemy in _spawnedEnemies)
            {
                movingEnemy.GetComponent<DT_MovingEnemy>().EnemyToggle(false);
            }
        }
    }

    public void ToggleStationaryEnemies(bool toggle)
    {
        if (toggle) //true
        {
            // Start all Stationary Enemies
            foreach (var stationaryEnemy in stationaryEnemies)
            {
                stationaryEnemy.GetComponent<DT_StationaryEnemy>().EnemyToggle(true);
            }
        }
        else
        {
            // Stop all Stationary Enemies
            foreach (var stationaryEnemy in stationaryEnemies)
            {
                stationaryEnemy.GetComponent<DT_StationaryEnemy>().EnemyToggle(false);
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
        // If player was crouching but isn't anymore
        if (_oldPlayerState == GameManager.PlayerState.Crouching && newPlayerState != GameManager.PlayerState.Crouching)
        {
            //Tell moving enemies
            foreach (var movingEnemy in _spawnedEnemies)
            {
                movingEnemy.GetComponent<DT_MovingEnemy>().OnPlayerCrouch(false);
            }
        }
        
        switch (newPlayerState)
        {
            case GameManager.PlayerState.Idle:
                break;
            case GameManager.PlayerState.Talking:
            // Option to stop enemies during talking will be option on Level Manager Trigger Handler
                break;
            case GameManager.PlayerState.Stepping:
                break;
            case GameManager.PlayerState.Climbing:
                break;
            case GameManager.PlayerState.Crouching:
            {
                // Tell moving enemies that player is crouching
                foreach (var movingEnemy in _spawnedEnemies)
                {
                    movingEnemy.GetComponent<DT_MovingEnemy>().OnPlayerCrouch(true);
                }
            }
                break;
            case GameManager.PlayerState.Damaged:
            {
                // Enemies are set to not do damage while player damaged
            }
                break;
            case GameManager.PlayerState.Attacking:
            {
                // Attack all attackable Stationary enemies
                foreach (var enemy in stationaryEnemies)
                {
                    enemy.GetComponent<DT_StationaryEnemy>().OnPlayerAttack();
                }
            }
                break;
            case GameManager.PlayerState.Defending:
                break;
            case GameManager.PlayerState.BardMode:
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
    public GameObject[] spawnPoints;
    public float spawnRate;
}