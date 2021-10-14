// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.Audio;
// using System;

// public class AudioManager : MonoBehaviour
// {

//     private static AudioManager _instance;
//     public static AudioManager Instance
//     {
//         get
//         {
//             if (_instance == null)
//             {
//                 GameObject go = new GameObject("AudioManager");
//                 go.AddComponent<AudioManager>();
//             }

//             return _instance;
//         }
//     }

//     public Sound[] sounds;

//     private void Awake()
//     {
//         _instance = this;
//         foreach (Sound s in sounds)
//         {
//             s.source = gameObject.AddComponent<AudioSource>();
//             s.source.clip = s.clip;
//             s.source.outputAudioMixerGroup = s.output;

//             s.source.loop = s.loop;
//             s.source.volume = s.volume;
//             s.source.playOnAwake = s.playOnAwake;

//         }
//     }

//     public void PlaySound(string name)
//     {
//         Sound s = Array.Find(sounds, sound => sound.name == name);

//         if(s==null)
//         {
//             Debug.LogWarning("Sound: " + name + "not found!");
//             return;
//         }
        
//         if (s.source.isPlaying)
//             StopSound(name);
        
//         s.source.Play();
//     }
//     public void StopSound(string name)
//     {
//         Debug.Log($"Stopping {name}");
//         Sound s = Array.Find(sounds, sound => sound.name == name);
//         if (s == null)
//         {
//             Debug.LogWarning("Sound: " + name + "not found!");
//             return;
//         }
//         s.source.Stop();
//     }
// }
