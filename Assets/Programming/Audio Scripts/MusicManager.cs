using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioClip defaultTrack;
    private AudioSource source;
    private AudioClip trackToPlay;
    private float trackToPlayVol;
    //sets number of seconds audio fades over, used in function FadeTrack()
    [Range(0f, 5f)] public float audioFadeTime;


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
            
            Debug.Log("first musicManager!");
            //then make this the MusicManager instance and don't destroy when loading new scene
            instance = this;
            DontDestroyOnLoad(instance);                        
        }
        //else if there is already a MusicManager instance, then destroy this gameObject
        else if (instance != null && instance != this)
        {
            DestroyImmediate(gameObject);
            Debug.Log("destroyed musicManager!");
            return;
        }

    }

    private void Start()
    {
        track01 = gameObject.AddComponent<AudioSource>();
        track02 = gameObject.AddComponent<AudioSource>();
        isPlayingTrack01 = true;
        
        SwapTrack(defaultTrack, 1);
        Debug.Log("MusicManager Start");
    }

    public void SwapTrack(AudioClip newClip, float newClipVol)
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

    // Update is called once per frame
    void Update()
    {

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
    //Audio clip
    public AudioClip sceneMusic;
    [Range(0f, 1f)] public float volume;
    public AudioSource source;
}

