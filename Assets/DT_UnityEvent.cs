using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DT_UnityEvent : MonoBehaviour
{
    public MyEvent Event;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


[Serializable]
public class MyEvent : UnityEvent {}