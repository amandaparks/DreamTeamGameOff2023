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

    public void OnStepFwd()
    {
        Debug.Log("Stepped forward");
    }
}
