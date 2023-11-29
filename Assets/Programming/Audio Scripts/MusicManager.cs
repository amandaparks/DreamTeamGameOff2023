using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    //sets up the first track to be played in this scene, editable in inspector
    public AudioClip defaultTrack;
    [Range(0f, 1f)] public float defaultTrackVol;

    //sets up variables which will be assigned later
    private AudioClip trackToPlay;
    private float trackToPlayVol;

    //sets number of seconds audio fades over, used in function FadeTrack()
    [Range(0f, 5f)] public float audioFadeTime;

    //create two AudioSource components so we can fade between the two
    private AudioSource track01, track02;
    private bool isPlayingTrack01;

    public static MusicManager instance;

    public Soundtracks[] tracks;

    // Start is called before the first frame update
    void Awake()
    {
        //if there is no other MusicManager instance
        if (instance == null) 
        {          
            //then make this the MusicManager instance and don't destroy when loading new scene
            instance = this;
            DontDestroyOnLoad(instance);                        
        }
        //else if there is already another MusicManager instance, then destroy this gameObject IMMEDIATELY (doesn't work otherwise)
        else if (instance != null && instance != this)
        {
            DestroyImmediate(gameObject);
            return;
        }

    }


    private void Start()
    {
        //creates AudioSource components for track01 and track02 and makes them loop
        track01 = gameObject.AddComponent<AudioSource>();
        track01.loop = true;
        track02 = gameObject.AddComponent<AudioSource>();
        track02.loop = true;
        isPlayingTrack01 = true;
        
        //plays music for first time
        SwapTrack(defaultTrack, 1);        
    }

    private void SwapTrack(AudioClip newClip, float newClipVol)
    {
        //StopAllCoroutines();
        StartCoroutine(FadeTrack(newClip, newClipVol));
        isPlayingTrack01 = !isPlayingTrack01;
    }

    //Coroutine which handles fading in and out between tracks
    //basically there are 2 audiosources active and whichever one is not currently playing will be used to load the new clip
    private IEnumerator FadeTrack(AudioClip clipToPlay, float targetVolume)
    {
        float time = 0f;
        //if track01 is playing
        if (isPlayingTrack01)
        {
            track02.clip = clipToPlay;
            track02.Play();            
            float playingVol = track01.volume;
            //fadey things - fade in track02 with the new music and fade out track01
            while (time < audioFadeTime)
            {
                time += Time.deltaTime;
                track02.volume = Mathf.Lerp(0, targetVolume, time / audioFadeTime);
                track01.volume = Mathf.Lerp(playingVol, 0, time / audioFadeTime);
                yield return null;
            }

            track01.Stop();
        }

        //else if track02 is playing
        else
        {
            track01.clip = clipToPlay;
            track01.Play();
            float playingVol = track02.volume;
            //fadey things - fade in track01 with the new music and fade out track02
            while (time < audioFadeTime)
            {
                time += Time.deltaTime;
                track01.volume = Mathf.Lerp(0, targetVolume, time / audioFadeTime);
                track02.volume = Mathf.Lerp(playingVol, 0, time / audioFadeTime);
                yield return null;
            }

            track02.Stop();
        }

    }


    //Changes the music based on scene and calls the SwapTrack function
    public void ChangeBGM (GameManager.GameScene newScene)
    {
        //searches through the arry to look for matching scene and finds the corresponding audio clip and audio volume
        foreach (var entry in tracks)
        {
            if(entry.sceneName == newScene) 
            {
                trackToPlay = entry.sceneMusic;
                trackToPlayVol = entry.volume;                
            }
        }
        //calls the SwapTrack function to fade into the next track at the volume set in the inspector element
        SwapTrack(trackToPlay, trackToPlayVol);
        Debug.Log("Changing BGM");
    }
}


//Pick which tracks go with which scenes in inspector
[System.Serializable]

public class Soundtracks
{
    //Corresponding scene
    public string trackScene;
    public GameManager.GameScene sceneName;
    //Audio clip and adjustable volume
    public AudioClip sceneMusic;
    [Range(0f, 1f)] public float volume;
    public AudioSource source;
}

