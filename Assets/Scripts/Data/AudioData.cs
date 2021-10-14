using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName="AudioData", menuName="Data/Audio Data")]
public class AudioData : ScriptableObject
{   
    public List<Sound> Music = new List<Sound>();
    public List<Sound> Sounds = new List<Sound>();
}