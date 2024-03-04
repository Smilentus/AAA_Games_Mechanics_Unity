using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "MusicRadioBroadcastingProgramPart", menuName = "RadioBroadcastingSystem/MusicRadioBroadcastingProgramPart")]
public class MusicRadioBroadcastingProgramPart : BaseRadioBroadcastingProgramPart
{
    [SerializeField]
    private List<AudioClip> m_musicAudioClips = new List<AudioClip>();
    public List<AudioClip> MusicAudioClips => m_musicAudioClips;


    private void OnValidate()
    {
        m_programPartTitle = "Музыкальная пауза";
    }

    private void Reset()
    {
        m_programPartTitle = "Музыкальная пауза";
    }


    public override float GetProgramPartSecondsLength()
    {
        float length = 0;

        foreach (AudioClip clip in m_musicAudioClips)
        {
            length += clip.length;
        }

        return length;
    }
}
