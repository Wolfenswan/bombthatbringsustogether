using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class AudioManager : MonoBehaviour
{

    private static AudioManager _instance;
    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("AudioManager");
                go.AddComponent<AudioManager>();
            }

            return _instance;
        }
    }

    [SerializeField] bool _debugging;
    [SerializeField] AudioData _data;
    
    Dictionary<string, AudioSource> _allAudioSources = new Dictionary<string, AudioSource>();
    Dictionary<string, AudioSource> _musicTracks = new Dictionary<string, AudioSource>();
    Dictionary<string, AudioSource> _soundEffects = new Dictionary<string, AudioSource>();

    public List<AudioSource> ActiveAudioSources{get => _allAudioSources.Values.Where(asource => asource.isPlaying).ToList();}

    private void Awake()
    {
        _instance = this;
        _data.Music.ForEach(s => AddNewMusicTrack(s));
        _data.Sounds.ForEach(s => AddNewSoundEffect(s));
    }

    private AudioSource AddNewAudioSource(Sound s)
    {
        s.source = gameObject.AddComponent<AudioSource>();
        s.source.clip = s.clip;
        s.source.outputAudioMixerGroup = s.output;

        s.source.loop = s.loop;
        s.source.volume = s.volume;
        s.source.playOnAwake = s.playOnAwake;

        _allAudioSources[s.name] = s.source;
        return s.source;
    }

    private void AddNewMusicTrack(Sound s)
    {
        var asource = AddNewAudioSource(s);
        _musicTracks[s.name] = s.source;
    }

    private void AddNewSoundEffect(Sound s)
    {
        var asource = AddNewAudioSource(s);
        _soundEffects[s.name] = s.source;
    }

    public void StartMusicTrack(string name, bool canOverlap = false) 
    {   
        if (_musicTracks.TryGetValue(name, out AudioSource audioSource))
            Play(audioSource, canOverlap);
        else 
            Debug.LogWarning($"Could not find music track {name} in {_musicTracks}");
    }

    public void StopMusicTrack(string name) 
    {
        if (_musicTracks.TryGetValue(name, out AudioSource audioSource))
            Stop(audioSource);
        else 
            Debug.LogWarning($"Could not find music track {name} in {_musicTracks}");
    }
    
    public void PlaySoundEffect(string name, bool canOverlap = true) 
    {
        if (_soundEffects.TryGetValue(name, out AudioSource audioSource))
            Play(audioSource, canOverlap);
        else 
            Debug.LogWarning($"Could not find sound effect {name} in {_soundEffects}");
    }

    public void StopSoundEffect(string name) 
    {
        if (_soundEffects.TryGetValue(name, out AudioSource audioSource))
            Stop(audioSource);
        else 
            Debug.LogWarning($"Could not find sound effect {name} in {_soundEffects}");
    }

    void Play(AudioSource asource, bool canOverlap)
    {   
        if(_debugging) Debug.Log($"Playing {asource.clip}");   
        if (!canOverlap && asource.isPlaying)
            Stop(asource);
        asource.Play();
    }

    void Stop(AudioSource asource)
    {
        asource.Stop();
    }

    void StopAllActive() => ActiveAudioSources.ForEach(asource => Stop(asource));
}

