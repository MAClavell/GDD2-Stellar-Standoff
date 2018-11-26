using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager> {

    //Public fields
    public StringAudioClipDictionary musicClipDictionary;
    public StringAudioClipDictionary soundClipDictionary;

    //Private fields
    Dictionary<string, AudioSource> musicLayerDictionary;
    Dictionary<string, Coroutine> musicCoroutineDictionary;
    AudioSource soundSource;

    // Use this for initialization
    void Awake() {
        //Instantiate dicts
        musicLayerDictionary = new Dictionary<string, AudioSource>();
        musicCoroutineDictionary = new Dictionary<string, Coroutine>();

        //Create gameobjects for each music player
        foreach (string str in musicClipDictionary.Keys)
        {
            AudioSource layer = new GameObject(str, typeof(AudioSource)).GetComponent<AudioSource>();
            layer.transform.parent = transform;
            layer.clip = musicClipDictionary[str];
            layer.playOnAwake = false;
            layer.Stop();
            musicLayerDictionary.Add(str, layer);
        }

        //Create soundeffect layer
        soundSource = new GameObject("SoundEffects", typeof(AudioSource)).GetComponent<AudioSource>();
        soundSource.transform.parent = transform;
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
    /// Play a one shot sound effect
    /// </summary>
    /// <param name="name">The name of the audio clip to play</param>
    /// <param name="volumeScale">The scale of the volume (0-1)</param>
    public void PlayOneShot(string name, float volumeScale)
    {
        //Error check
        if (!soundClipDictionary.ContainsKey(name))
        {
            Debug.LogError("Audio layer does not exist in dictionary");
            return;
        }

        soundSource.PlayOneShot(soundClipDictionary[name], volumeScale);
    }

    /// <summary>
    /// Play a layer
    /// </summary>
    /// <param name="layerName">The name of the layer</param>
    public void PlayLayer(string layerName)
    {
        //Error check
        if (!musicLayerDictionary.ContainsKey(layerName))
        {
            Debug.LogError("Audio layer does not exist in dictionary");
            return;
        }

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
        //Error check
        if (!musicLayerDictionary.ContainsKey(layerName))
        {
            Debug.LogError("Audio layer does not exist in dictionary");
            return;
        }

        if (IsLayerPlaying(layerName))
        {
            musicLayerDictionary[layerName].Stop();
        }
    }

    /// <summary>
    /// Set the volume of a layer
    /// </summary>
    /// <param name="layerName">The name of the layer</param>
    /// <param name="volume">The volume to set it to</param>
    public void SetLayerVolume(string layerName, float volume)
    {
        //Error check
        if (!musicLayerDictionary.ContainsKey(layerName))
        {
            Debug.LogError("Audio layer does not exist in dictionary");
            return;
        }

        musicLayerDictionary[layerName].volume = volume;
    }

    /// <summary>
    /// Fade an audio layer based on a given goal
    /// </summary>
    /// <param name="layerName">Layer to fade</param>
    /// <param name="goalVolume">The goal volume</param>
    /// <param name="fadeLength">The length of the fade</param>
    public void FadeLayer(string layerName, float goalVolume, float fadeLength)
    {
        //Error check
        if (!musicLayerDictionary.ContainsKey(layerName))
        {
            Debug.LogError("Audio layer does not exist in dictionary");
            return;
        }

        if (musicLayerDictionary[layerName].volume > goalVolume)
        {
            FadeOutLayer(layerName, goalVolume, fadeLength);
        }
        else FadeInLayer(layerName, goalVolume, fadeLength);
    }

    /// <summary>
    /// Fade in an audio layer
    /// </summary>
    /// <param name="layerName">Layer to fade in</param>
    /// <param name="goalVolume">The goal volume</param>
    /// <param name="fadeLength">The length of the fade</param>
    public void FadeInLayer(string layerName, float goalVolume, float fadeLength)
    {
        //Error check
        if (!musicLayerDictionary.ContainsKey(layerName))
        {
            Debug.LogError("Audio layer does not exist in dictionary");
            return;
        }

        if (musicCoroutineDictionary[layerName] != null)
            StopCoroutine(musicCoroutineDictionary[layerName]);

        StartCoroutine(LayerFadeIn(musicLayerDictionary[layerName], goalVolume, fadeLength));
    }

    /// <summary>
    /// Fade out an audio layer
    /// </summary>
    /// <param name="layerName">Layer to fade out</param>
    /// <param name="goalVolume">The goal volume</param>
    /// <param name="fadeLength">The length of the fade</param>
    public void FadeOutLayer(string layerName, float goalVolume, float fadeLength)
    {
        //Error check
        if (!musicLayerDictionary.ContainsKey(layerName))
        {
            Debug.LogError("Audio layer does not exist in dictionary");
            return;
        }

        if (musicCoroutineDictionary[layerName] != null)
            StopCoroutine(musicCoroutineDictionary[layerName]);

        StartCoroutine(LayerFadeOut(musicLayerDictionary[layerName], goalVolume, fadeLength));
    }

    /// <summary>
    /// Coroutine to fade in an audio layer
    /// </summary>
    /// <param name="source">Audio source to fade in</param>
    /// <param name="goalVol">Goal volume</param>
    /// <param name="fadeLength">The length of the fade</param>
    /// <returns></returns>
    IEnumerator LayerFadeIn(AudioSource source, float goalVol, float fadeLength)
    {
        float vol = source.volume;
        //Fade the vol
        while (vol < goalVol)
        {
            vol += Time.deltaTime / fadeLength;
            source.volume = vol;
            yield return null;
        }
        source.volume = goalVol;
    }

    /// <summary>
    /// Coroutine to fade in an audio layer
    /// </summary>
    /// <param name="source">Audio source to fade in</param>
    /// <param name="goalVol">Goal volume</param>
    /// <param name="fadeLength">The length of the fade</param>
    /// <returns></returns>
    IEnumerator LayerFadeOut(AudioSource source, float goalVol, float fadeLength)
    {
        float vol = source.volume;
        //Fade the vol
        while (vol > goalVol)
        {
            vol -= Time.deltaTime / fadeLength;
            source.volume = vol;
            yield return null;
        }
        source.volume = goalVol;
    }
}
