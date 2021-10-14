using System;
using UnityEngine;
using UnityEngine.Audio;

[Serializable]
public class Sound
{
    public string name;

    public AudioClip clip;
    public AudioMixerGroup output;

    [Range(0f, 1f)]
    public float volume;

    public bool loop;
    public bool playOnAwake;

    [HideInInspector]
    public AudioSource source;
}
