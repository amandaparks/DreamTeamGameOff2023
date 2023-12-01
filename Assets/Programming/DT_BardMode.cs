using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DT_BardMode : MonoBehaviour
{
    private AudioManager _audioManager;
    private DT_EndSceneManager _endSceneManager;
    private void Start()
    {
        _audioManager = FindObjectOfType<AudioManager>();
        if (_audioManager == null)
        {
            Debug.LogError("No Audio Manager Found");
        }

        if (GameManager.CurrentPlayerLevel == GameManager.PlayerLevel.SevenNotes)
        {
            _endSceneManager = FindObjectOfType<DT_EndSceneManager>();
        }

    }

    public void PlayNote(string keyNumber)
    {
        _audioManager.PlayKalimba(keyNumber);

        // If at end of game
        if (GameManager.CurrentPlayerLevel == GameManager.PlayerLevel.SevenNotes)
        {
            //Tell end scene manager what notes are being played
            _endSceneManager.NotePlayed(keyNumber);
        }
    }
}
