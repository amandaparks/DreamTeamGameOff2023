using System.Collections;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class DT_PlayerMovement : MonoBehaviour
{
    public float stepDuration;
    public float stepArcHeight;
    public float climbDuration;
    public float maxStepHeight;
    public static TargetDirection targetDirection;
    public enum TargetDirection
    {
        Left,
        Right,
        Up,
        Down,
        Stop
    }
    private GameManager _gameManager;
    private Vector3 _levelStartPos;
    private Vector3 _targetPos;
    private bool isNoStonesBruh;
    private bool isNoLaddersBruh;
    private GameObject[] _steppingStones;
    private GameObject[] _ladders;
    private GameObject _nextStone;
    private GameObject _currentStone;
    private GameObject _ladderStartTarget;
    private GameObject _ladderEndTarget;
    void Start()
    {
        // Find the Game Manager and assign it to this variable
        _gameManager = GameManager.Instance;
        // Note position of the Player on level start and tell GameManager
        _levelStartPos = gameObject.transform.position;
        GameManager.PlayerStartPos = _levelStartPos;
        
        // Find all the stepping stones and ladders
        _steppingStones = GameObject.FindGameObjectsWithTag("StepStone");
        Debug.Log("Found "+_steppingStones.Length+" stone(s)!");
        _ladders = GameObject.FindGameObjectsWithTag("Ladder");
        Debug.Log("Found "+_ladders.Length+" ladder(s)!");
        
        // If there are no stones or ladders, set bools to true
        isNoStonesBruh = _steppingStones.Length == 0;
        isNoLaddersBruh = _ladders.Length == 0;
    }
    public void StepBkd()
    {
        Debug.Log("INPUT: STEP BKD");
        
        // Check we can do it
        if (!CanPerformAction("Step")) return;

        // Play sound but I can't get it to work because CS0120 error
        // AudioManager.PlayKalimba("3");
        
        // Move character
        targetDirection = TargetDirection.Left;
        StartCoroutine(Step());
    }
    public void StepFwd()
    {
        Debug.Log("INPUT: STEP FWD");
        
        // Check we can do it
        if (!CanPerformAction("Step")) return;

        // Play sound but I can't get it to work because CS0120 error
        // AudioManager.PlayKalimba("5");

        // Move character
        targetDirection = TargetDirection.Right;
        StartCoroutine(Step());
            
    }
    public void Climb()
    {
        Debug.Log("INPUT: CLIMB");
        
        // Check we can do it
        if (!CanPerformAction("Climb")) return;

        // Play sound but I can't get it to work because CS0120 error
        // AudioManager.PlayKalimba("4");

        // CLimb
        StartCoroutine(ClimbLadder());
    }
    private bool CanPerformAction(string actionType)
    {
        // Player can only move if game is not paused and they are already in Idle
        if (GameManager.CurrentGameState == GameManager.GameState.Paused)
        {
            Debug.Log($"{actionType} cancelled. INVALID: Game is paused.");
            return false;
        }

        if (GameManager.CurrentPlayerState != GameManager.PlayerState.Idle)
        {
            Debug.Log($"{actionType} cancelled. INVALID: Player not Idle.");
            return false;
        }

        // Other checks...
        switch (actionType)
        {
            case "Step":
                if (isNoStonesBruh)
                {
                    Debug.Log($"{actionType} cancelled. INVALID: No stones in the scene.");
                    return false;
                }
                break;

            case "Climb":
                if (isNoLaddersBruh)
                {
                    Debug.Log($"{actionType} cancelled. INVALID: No ladders in the scene.");
                    return false;
                }

                if (_currentStone == null)
                {
                    Debug.Log($"{actionType} cancelled. INVALID: Must be on a stone.");
                    return false;
                }

                if  (_currentStone.GetComponentInParent<DT_LadderGroup>() == null)
                {
                    Debug.Log($"{actionType} cancelled. INVALID: Stone must be part of a ladder group.");
                    return false;
                }
                break;

            default:
                Debug.LogError($"Unknown action type: {actionType}");
                return false;
        }
        
        return true;
    }
    private GameObject FindNextStone()
    {
        // Make sure starting value is null
        _nextStone = null;
        
        // Compare the distance from player of each stone to the previous stone
        // For the first stone, compare it to the biggest possible number (float.MaxValue)
        
        var closestDistance = float.MaxValue;
        
        foreach (GameObject stone in _steppingStones)
        {
            // Get distance b/w player and stone
            var distanceToStone = Vector3.Distance(transform.position, stone.transform.position);

            // Check if it's on the side we want
            bool isTargetDirection = (targetDirection == TargetDirection.Right && stone.transform.position.x > transform.position.x) ||
                                      (targetDirection == TargetDirection.Left && stone.transform.position.x < transform.position.x);

            // If it's on the side we want and closer than any previous stones
            if (isTargetDirection && distanceToStone < closestDistance)
            {
                //Set the new closest distance
                closestDistance = distanceToStone;
                
                //Set the stone as the target
                _nextStone = stone;
            }
        }
        
        
        // Update _nextStone
        
        if (_nextStone != null)
        {
            // Return the updated stone
            return _nextStone;
        }

        // If no stones were found
        
        Debug.Log("INVALID: No step target.");
        return null;
    }
    private void FindLadderNextTargets()
    {
        // Make sure starting values are null
        _nextStone = null;
        _ladderStartTarget = null;
        _ladderEndTarget = null;
        
        //If current stone is the base step stone
        if (_currentStone.GetComponentInParent<DT_LadderGroup>().baseStepStone == _currentStone)
        {
            //Assign direction and set the top step stone as the next stone
            targetDirection = TargetDirection.Up;
            _nextStone = _currentStone.GetComponentInParent<DT_LadderGroup>().topStepStone;
            //Assign the start and end points of the movement so sprite doesn't cut through ladder
            _ladderStartTarget = _currentStone.GetComponentInParent<DT_LadderGroup>().ladderBase;
            _ladderEndTarget = _currentStone.GetComponentInParent<DT_LadderGroup>().ladderTop;
            //Return next stone
            return;
        }
        //If current stone is the top step stone
        if (_currentStone.GetComponentInParent<DT_LadderGroup>().topStepStone == _currentStone)
        {
            //Assign direction and set the base step stone as the next stone
            targetDirection = TargetDirection.Down;
            _nextStone = _currentStone.GetComponentInParent<DT_LadderGroup>().baseStepStone;
            //Assign the start and end points of the movement so sprite doesn't cut through ladder
            _ladderStartTarget = _currentStone.GetComponentInParent<DT_LadderGroup>().ladderTop;
            _ladderEndTarget = _currentStone.GetComponentInParent<DT_LadderGroup>().ladderBase;
            //Return next stone
            return;
        }
        //If neither, log error.
        Debug.LogError("Cannot find ladder parts.");
    }
    private IEnumerator Step()
    {
        // First, find next stone
        _nextStone = FindNextStone();
        
        // If no stones found...
        if (_nextStone == null)
        {
            Debug.Log("INVALID: No more stones.");
        }
        // If next stone is too high/low...
        // Mathf.Abs always returns a positive number
        else if (Mathf.Abs(_nextStone.transform.position.y - transform.position.y) >= maxStepHeight)
        {
            Debug.Log("INVALID: Stone too far away.");
        }
        // Stone found...
        else
        {
            Debug.Log("PLAYER_STATE: STEPPING");
            GameManager.CurrentPlayerState = GameManager.PlayerState.Stepping;
            
            // Set start and end points of curve
            var startPoint = transform.position;
            var endPoint = _nextStone.transform.position;
            // Start counting
            float elapsedTime = 0f;

            while (elapsedTime < stepDuration && GameManager.CurrentPlayerState != GameManager.PlayerState.Damaged)
            {
                // Find where on curve player should be this frame
                float t = elapsedTime / stepDuration;
                Vector3 position = QuadraticBezier(startPoint, endPoint, stepArcHeight, t);

                // Set position
                transform.position = position;

                // Increment for next frame
                elapsedTime += Time.deltaTime;

                // Wait
                yield return null;
            }
            
            // If the player has been damaged
            if (GameManager.CurrentPlayerState == GameManager.PlayerState.Damaged)
            {
                // Reset current/next stones
                _currentStone = null;
                _nextStone = null;
            
                yield return null;
            }
        
            //Otherwise
            
            // Force player onto correct spot
            transform.position = _nextStone.transform.position;
            // Assign as current stone
            _currentStone = _nextStone;
            ResetToIdle();
            yield return null;
        }
    }
    private IEnumerator ClimbLadder()
    {
        //First find the next stone and Ladder Targets
        FindLadderNextTargets();
        
        //Set the target points
        var startPoint = _ladderStartTarget.transform.position;
        var endPoint = _ladderEndTarget.transform.position;
        
        // If anything is missing...
        if (_nextStone == null || _ladderStartTarget == null || _ladderEndTarget == null)
        {
            Debug.Log("INVALID: Missing target info.");
        }
        
        //Set state to climbing
        Debug.Log("PLAYER_STATE: CLIMBING");
        GameManager.CurrentPlayerState = GameManager.PlayerState.Climbing;

        // Start counting
        float elapsedTime = 0f;

        // Climb until there is no time left
        while (elapsedTime < climbDuration && GameManager.CurrentPlayerState != GameManager.PlayerState.Damaged)
        {
            // Move to where Player should be this frame
            transform.position = Vector3.Lerp(startPoint, endPoint, elapsedTime / climbDuration);

            // Increment for next frame
            elapsedTime += Time.deltaTime;

            // Wait
            yield return null;
        }
        
        // If the player has been damaged
        if (GameManager.CurrentPlayerState == GameManager.PlayerState.Damaged)
        {
            // Reset current/next stones
            _currentStone = null;
            _nextStone = null;
            
            yield return null;
        }
        
        //Otherwise

        // Force player on to correct spot
        transform.position = _nextStone.transform.position;
        // Assign the top/base stone as current stone
        _currentStone = _nextStone;
        ResetToIdle();
            
        yield return null;

    }
    public void ResetToIdle()
    {
        _nextStone = null;
        _ladderStartTarget = null;
        _ladderEndTarget = null;
        targetDirection = TargetDirection.Stop;
        
        // If player has already started talking, don't change the state
        if (GameManager.CurrentPlayerState == GameManager.PlayerState.Talking) return;
        
        // Otherwise, return to idle
        GameManager.CurrentPlayerState = GameManager.PlayerState.Idle;
        Debug.Log("PLAYER_STATE: IDLE");
    }
    private Vector3 QuadraticBezier(Vector3 p0, Vector3 p2, float height, float t)
    {
        // Quadratic Bezier curve calculation with added control point for height
        // Thanks to old mate chatGPT and https://www.gamedeveloper.com/business/how-to-work-with-bezier-curve-in-games-with-unity
        
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 p = uu * p0; // (1-t)^2 * P0
        p += 2 * u * t * (p2 + Vector3.up * height); // Use p0 + Vector3.up for height at start of curve
        p += tt * p2; // t^2 * P2

        return p;
    }
    
}
