using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager> {

    //Public fields
    public StringAudioClipDictionary musicClipDictionary;

    //Private fields
    Dictionary<string, AudioSource> musicLayerDictionary;
    Dictionary<string, Coroutine> musicCoroutineDictionary;


    // Use this for initialization
    void Awake() {
        //Instantiate dicts
        musicLayerDictionary = new Dictionary<string, AudioSource>();
        musicCoroutineDictionary = new Dictionary<string, Coroutine>();

        //Create gameobjects for each music player
        foreach (string str in musicClipDictionary.Keys)
        {
            AudioSource layer = new GameObject("str", typeof(AudioSource)).GetComponent<AudioSource>();
            layer.transform.parent = transform;
            layer.clip = musicClipDictionary[str];
            layer.playOnAwake = false;
            layer.Stop();
            musicLayerDictionary.Add(str, layer);
        }
    }

    // Update is called once per frame
    void Update() {

    }

    /// <summary>
    /// Check if a layer is playing
    /// </summary>
    /// <param name="layerName">Name of the layer</param>
    public bool IsLayerPlaying(string layerName)
    {
        return musicLayerDictionary[layerName].isPlaying;
    }

    /// <summary>
    /// Play a layer
    /// </summary>
    /// <param name="layerName">The name of the layer</param>
    public void PlayLayer(string layerName)
    {
        if (!IsLayerPlaying(layerName))
        {
            musicLayerDictionary[layerName].Play();
        }
    }

    /// <summary>
    /// Stop a layer
    /// </summary>
    /// <param name="layerName">The name of the layer</param>
    public void StopLayer(string layerName)
    {
        if (IsLayerPlaying(layerName))
        {
            musicLayerDictionary[layerName].Stop();
        }
    }

    /// <summary>
    /// Fade in an audio layer
    /// </summary>
    /// <param name="layerName">Layer to fade in</param>
    /// <param name="goalVolume">The goal volume</param>
    /// <param name="fadeLength">The length of the fade</param>
    public void FadeInLayer(string layerName, float goalVolume, float fadeLength)
    {
        if (musicCoroutineDictionary[layerName] != null)
            StopCoroutine(musicCoroutineDictionary[layerName]);

        StartCoroutine(LayerFadeIn(musicLayerDictionary[layerName], goalVolume, fadeLength));
    }

    IEnumerator LayerFadeIn(AudioSource source, float goalVol, float fadeLength)
    {
        float vol = source.volume;
        //Fade the vol
        while (vol <= goalVol)
        {
            vol -= Time.deltaTime / fadeLength;
            source.volume = vol;
            yield return null;
        }
        source.volume = goalVol;
    }

}
