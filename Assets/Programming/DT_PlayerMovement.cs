using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DT_PlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    public float climbSpeed;
    private Vector3 _startPos;
    private Vector3 _targetPos;
    
    // Start is called before the first frame update
    void Start()
    {
        _startPos = gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnStepBkd()
    {
        Debug.Log("Stepped backward");
    }
    public void OnStepFwd()
    {
        Debug.Log("Stepped forward");
    }
    public void OnClimb()
    {
        Debug.Log("Climbed");
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
