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

    public PlayableRadioMusicData GetPlayableDataByTotalLength(float passedSeconds)
    {
        float clipTime = 0;

        AudioClip outputClip = null;
        float outputTime = 0;

        foreach (AudioClip clip in m_musicAudioClips)
        {
            clipTime += clip.length;
            
            if (passedSeconds <= clipTime)
            {
                outputTime = passedSeconds - (clipTime - clip.length);
                outputClip = clip;
                break;
            }
        }

        return new PlayableRadioMusicData()
        {
            Clip = outputClip,
            PassedTime = outputTime
        };
    }
}

[System.Serializable]
public struct PlayableRadioMusicData
{
    public AudioClip Clip;
    public float PassedTime;
}