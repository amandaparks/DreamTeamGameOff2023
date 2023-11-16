using UnityEngine.Audio;
using System;
using System.Security.Cryptography;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Existing array
    public Sound[] sounds;
    
    [Header("KALIMBA NOTES: Method is PlayKalimba(''key'');")]
    // This is a slightly different one just for kalimba notes:
    public Notes[] kalimbaNotes;
    void Awake() {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
        }
        
        // We'll do the same with the kalimba notes:
        // For each note entry in our array of kalimba notes...
        foreach (Notes entry in kalimbaNotes)
        {
            // Assign elements to the entry's AudioSource (each entry has one but it's hidden in the inspector)
            
            // The source itself = create it and put it on this game object
            entry.source = gameObject.AddComponent<AudioSource>();
            
            // The clip the source will play = the note clip we assigned in the inspector
            entry.source.clip = entry.noteClip;
            
            // The volume to play at = the volume for that entry in the inspector
            entry.source.volume = entry.volume;
        }

    }

    public void Play (string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Play();
    }

    // Method for Kalimba Notes
    public void PlayKalimba(string keyNumber)
    {
        // The note we want to play = Look through all the kalimba notes until you get one that matches the button
        Notes noteToPlay = Array.Find(kalimbaNotes, notes => notes.buttonKey == keyNumber);
        // Play the corresponding audio source
        noteToPlay.source.Play();
    }
    
}

//This section sets what options you have to fill in for each note in the inspector:
[System.Serializable]
public class Notes
{
    //We'll use this variable when we send requests from other scripts
    public string buttonKey;
    //For reference
    public string kalimbaNote;
    //Audio clip
    public AudioClip noteClip;
    [Range(0f, 1f)] public float volume;
    [HideInInspector]
    public AudioSource source;
}
