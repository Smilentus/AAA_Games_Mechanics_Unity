using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class MediaAudioClipData
{
    public string ClipTitle { get; protected set; }
    
    public string ClipDescription { get; protected set; }

    public AudioClip AudioClipReference { get; protected set; }
}