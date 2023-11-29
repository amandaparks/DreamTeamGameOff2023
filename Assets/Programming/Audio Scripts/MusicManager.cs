using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private AudioSource source;

    private static MusicManager instance;

    public Soundtracks[] tracks;

    // Start is called before the first frame update
    void Awake()
    {
        if(instance == null) 
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(gameObject);
        }
        
        source = GetComponent<AudioSource>();
        source.volume = 0f;

        StartCoroutine(Fade(source, 2f, 1f));        
    }

    public IEnumerator Fade(AudioSource source, float duration, float targetVolume)
    {

        float time = 0f;
        float startVol = source.volume;
        while (time < duration)
        {
            time += Time.deltaTime;
            source.volume = Mathf.Lerp(startVol, targetVolume, time / duration);
            yield return null;
        }

        yield break;

    }
    // Update is called once per frame
    void Update()
    {
        
    }
    //private void OnDestroy()
    //{
    //    StartCoroutine(Fade(true, source, 2f, 0f));
    //}
}


//Pick which tracks go with which scenes in inspector
[System.Serializable]

public class Soundtracks
{
    [SerializeField] private GameManager.GameScene _sceneName;
    public AudioClip sceneMusic;
    [Range(0f, 1f)] public float volume;
}

