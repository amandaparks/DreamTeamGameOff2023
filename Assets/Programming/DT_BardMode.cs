using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DT_BardMode : MonoBehaviour
{
    private AudioManager _audioManager;
    private void Start()
    {
        _audioManager = FindObjectOfType<AudioManager>();
        if (_audioManager == null)
        {
            Debug.LogError("No Audio Manager Found");
        }
    }

    public void PlayNote(string keyNumber)
    {
        _audioManager.PlayKalimba(keyNumber);
    }
}
