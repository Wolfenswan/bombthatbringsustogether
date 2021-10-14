using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Volume : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider slider;
    float audioVolume;
    // Start is called before the first frame update
    void Awake()
    {
        audioMixer.GetFloat("volume", out audioVolume);
        slider.value = audioVolume;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
