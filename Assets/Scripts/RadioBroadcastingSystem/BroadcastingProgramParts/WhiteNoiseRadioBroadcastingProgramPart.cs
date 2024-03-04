using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "WhiteNoiseRadioBroadcastingProgramPart", menuName = "RadioBroadcastingSystem/WhiteNoiseRadioBroadcastingProgramPart")]
public class WhiteNoiseRadioBroadcastingProgramPart : BaseRadioBroadcastingProgramPart
{
    [SerializeField]
    private AudioClip m_whiteNoiseClip;
    public AudioClip WhiteNoiseClip => m_whiteNoiseClip;


    public override float GetProgramPartSecondsLength()
    {
        return m_whiteNoiseClip.length;
    }
}