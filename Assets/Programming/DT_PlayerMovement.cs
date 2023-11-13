using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DT_PlayerMovement : MonoBehaviour
{

    private GameManager _gameManager;

    public float moveDuration;
    public float jumpArcHeight;
    public float climbSpeed;
    private Vector3 _startPos;
    private Vector3 _targetPos;
    private bool isNoStonesBruh;
    private bool isNoLaddersBruh;

    private GameObject[] _steppingStones;
    private GameObject[] _ladders;

    private GameObject _nextStone;
    private GameObject _currentStone;

    private enum TargetDirection
    {
        Left,
        Right,
        Up,
        Down,
        Stop
    }

    private TargetDirection _targetDirection;

    // Start is called before the first frame update
    void Start()
    {
        // Find the Game Manager and assign it to this variable
        _gameManager = GameManager.Instance;
        
        // Find all the stepping stones and ladders
        _steppingStones = GameObject.FindGameObjectsWithTag("StepStone");
        Debug.Log("Found "+_steppingStones.Length+" stones!");
        _ladders = GameObject.FindGameObjectsWithTag("Ladder");
        Debug.Log("Found "+_ladders.Length+" ladders!");

        _startPos = gameObject.transform.position;
        
        // If there are no stones or ladders, set bools to true
        isNoStonesBruh = _steppingStones.Length == 0;
        isNoLaddersBruh = _ladders.Length == 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnStepBkd()
    {
        Debug.Log("INPUT: StepBkd");
        
        // Check we can do it
        if (!CanPerformAction("Step")) return;
        
        _targetDirection = TargetDirection.Left; 
        Debug.Log("Starting Step Coroutine");
        StartCoroutine(Step());
    }

    public void OnStepFwd()
    {
        Debug.Log("INPUT: StepFwd");
        
        // Check we can do it
        if (!CanPerformAction("Step")) return;
        
        _targetDirection = TargetDirection.Right;
        Debug.Log("Starting Step Coroutine");
        StartCoroutine(Step());
            
    }
    /*
    public void OnClimb()
    {
        Debug.Log("INPUT: Climb");
        
        // Check we can do it
        if (!CanPerformAction("Climb")) return;
        Debug.Log("Starting Climb Coroutine");
        StartCoroutine(Climb());
    }

    private IEnumerator Climb()
    {
    var theTranformShouldBe = _currentStone.transform.parent.GetComponent<DT_LadderGroup>().ladderBase.transform;
    
        //If current stone is tagged with Base
        //Go up
        if (_currentStone.CompareTag("LadderBase"))
        {
            _targetDirection = TargetDirection.Up;
        }
        //If current stone is tagged with Top
        //Go down
        else if (_currentStone.CompareTag("LadderTop"))
        {
            _targetDirection = TargetDirection.Down;
            _currentStone.transform.parent.gameObject
        }
        
        GameObject.FindGameObjectsWithTag("LadderBase").Where(tranform.parent == )
            
        GetComponentInParent()

        Debug.Log("PlayerState.Climbing");
        _gameManager.playerState = GameManager.PlayerState.Climbing;
            
        // Set start and end points of movement
        if (_targetDirection == TargetDirection.Up)
        {
            var startPoint = transform.position;
            var endPoint = _nextStone.transform.position;
        }
        else if (_targetDirection == TargetDirection.Down)
        {
            
        }

        
        // Start counting
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            // Find where on curve player should be this frame
            float t = elapsedTime / moveDuration;
            Vector3 position = QuadraticBezier(startPoint, endPoint, jumpArcHeight, t);

            // Set position
            transform.position = position;

            // Increment for next frame
            elapsedTime += Time.deltaTime;

            // Wait
            yield return null;
        }
            
        // Force player onto correct spot
        transform.position = _nextStone.transform.position;
        // Assign as current stone
        _currentStone = _nextStone;
        Debug.Log("Arrived at target!");
            
        ResetToIdle();
            
        yield return null;

        
    }
    */

    private bool CanPerformAction(string actionType)
    {
        // Player can only move if game is not paused and they are already in Idle
        if (_gameManager.gameState == GameManager.GameState.Paused)
        {
            Debug.Log($"{actionType} cancelled. Game is paused.");
            return false;
        }

        if (_gameManager.playerState != GameManager.PlayerState.Idle)
        {
            Debug.Log($"{actionType} cancelled. Player not Idle.");
            return false;
        }

        // Other checks...
        switch (actionType)
        {
            case "Step":
                if (isNoStonesBruh)
                {
                    Debug.Log($"{actionType} cancelled. No stones in the scene.");
                    return false;
                }
                break;

            case "Climb":
                if (isNoLaddersBruh)
                {
                    Debug.Log($"{actionType} cancelled. No ladders in the scene.");
                    return false;
                }
                if (_currentStone == null)
                {
                    Debug.Log($"{actionType} cancelled. Must be on a stone.");
                    return false;
                }
                break;
            
            default:
                Debug.LogError($"Unknown action type: {actionType}");
                return false;
        }
        
        return true;
    }

    public void ResetToIdle()
    {
        _nextStone = null;
        _targetDirection = TargetDirection.Stop;
        _gameManager.playerState = GameManager.PlayerState.Idle;
        Debug.Log("PARAMS RESET");
    }


   

    private IEnumerator Step()
    {
        // First, find next stone
        _nextStone = FindNextStone();
        
        // If no stones found...
        if (_nextStone == null)
        {
            Debug.Log("No more stones :(");
        }
        // Stone found...
        else
        {
            Debug.Log("PlayerState.Stepping");
            _gameManager.playerState = GameManager.PlayerState.Stepping;
            
            // Set start and end points of curve
            var startPoint = transform.position;
            var endPoint = _nextStone.transform.position;
            // Start counting
            float elapsedTime = 0f;

            while (elapsedTime < moveDuration)
            {
                // Find where on curve player should be this frame
                float t = elapsedTime / moveDuration;
                Vector3 position = QuadraticBezier(startPoint, endPoint, jumpArcHeight, t);

                // Set position
                transform.position = position;

                // Increment for next frame
                elapsedTime += Time.deltaTime;

                // Wait
                yield return null;
            }
            
            // Force player onto correct spot
            transform.position = _nextStone.transform.position;
            // Assign as current stone
            _currentStone = _nextStone;
            Debug.Log("Arrived at target!");
            
            ResetToIdle();
            
            yield return null;
        }
    }

    private GameObject FindNextStone()
    {
        Debug.Log("Finding next stone...");
        
        // Reset _nextStone
        _nextStone = null;
        
        Debug.Log("Checking each one...");
        
        // Compare the distance from player of each stone to the previous stone
        // For the first stone, compare it to the biggest possible number (float.MaxValue)
        
        var closestDistance = float.MaxValue;
        
        foreach (GameObject stone in _steppingStones)
        {
            // Get distance b/w player and stone
            var distanceToStone = Vector3.Distance(transform.position, stone.transform.position);

            // Check if it's on the side we want
            bool isTargetDirection = (_targetDirection == TargetDirection.Right && stone.transform.position.x > transform.position.x) ||
                                      (_targetDirection == TargetDirection.Left && stone.transform.position.x < transform.position.x);

            // If it's on the side we want and closer than any previous stones
            if (isTargetDirection && distanceToStone < closestDistance)
            {
                //Set the new closest distance
                closestDistance = distanceToStone;
                
                //Set the stone as the target
                _nextStone = stone;
            }
        }
        
        // If no stones were found

        if (_nextStone != null)
        {
            Debug.Log($"Closest stone is {_nextStone.name}");
            // Return the updated stone
            return _nextStone;
        }

        Debug.Log("No stones...");
        return null;
    }

    
// Quadratic Bezier curve calculation with added control point for height
// Thanks to old mate chatGPT and https://www.gamedeveloper.com/business/how-to-work-with-bezier-curve-in-games-with-unity
    private Vector3 QuadraticBezier(Vector3 p0, Vector3 p2, float height, float t)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 p = uu * p0; // (1-t)^2 * P0
        p += 2 * u * t * (p2 + Vector3.up * height); // Use p0 + Vector3.up for height at start of curve
        p += tt * p2; // t^2 * P2

        return p;
    }





    public void OnDuck()
    {
        Debug.Log("Ducked");
    }
    public void OnAttack()
    {
        Debug.Log("Attacked");
    }
    public void OnMagic()
    {
        Debug.Log("Magic!");
    }
    public void OnDefend()
    {
        Debug.Log("Defended");
    }

}
