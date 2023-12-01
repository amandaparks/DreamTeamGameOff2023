using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class DT_EnemyManager : MonoBehaviour
{
    [Header("MOVING ENEMY SPAWNER")] 
    [Header("Max has been set to 30 at once.")]
    [Space]
    [SerializeField] private DT_EnemyGroup[] enemiesToSpawn;
    private bool _canSpawnEnemies;
    private List<GameObject> _spawnedEnemies = new List<GameObject>();
    private int _maxListSize = 30;
    private GameManager.PlayerState _oldPlayerState;
    private bool _isSpawnerActive;
    
    private void OnEnable()
    {
        // Don't do anything if no enemies at all
        if (enemiesToSpawn.Length == 0) return;

        // Turn on spawner
        ToggleSpawner(true);
    }

    public IEnumerator SpawnEnemies(DT_EnemyGroup group)
    {
        // Don't do anything if no enemies assigned 
        if (group.enemy == null) yield return null;

        // Repeat forever while true
        while (_isSpawnerActive)
        {
            // Wait for time set (dividing 1 by rate means higher number = more spawning)
            yield return new WaitForSeconds(1 / group.spawnRate);

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
            GameObject newEnemy = Instantiate(group.enemy, point.transform.position, rotation);

            // Set it's move speed and max lifespan and max distance
            newEnemy.GetComponent<DT_MovingEnemy>().moveSpeed = group.moveSpeed;
            newEnemy.GetComponent<DT_MovingEnemy>().maxMoveDistance = group.maxMoveDistance;

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
    
    public void ToggleSpawner(bool toggle)
    {
        if (enemiesToSpawn.Length == 0) return;
        
        if (toggle) //true
        {
            // Start Spawning Enemies
            _isSpawnerActive = true;
            foreach (DT_EnemyGroup group in enemiesToSpawn)
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

}

[System.Serializable]
public class DT_EnemyGroup
{ 
    public GameObject enemy;
    public bool randomRotation;
    [Range(0.1f, 3.0f)] public float spawnRate;
    [Range(0.1f, 1.0f)] public float moveSpeed;
    [Tooltip("Use spawn point guides")] public float maxMoveDistance;
    public GameObject[] spawnPoints;
}