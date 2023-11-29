using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DT_SmoothUI : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void Open()
    {
        // Animate
        _animator.SetTrigger("Open");
    }

    public void Close()
    {
        // Animate
        _animator.SetTrigger("Close");
        
    }

    // Method used by animator
    public void Deactivate()
    {
        // Turn off this game object
        gameObject.SetActive(false);
    }
}
