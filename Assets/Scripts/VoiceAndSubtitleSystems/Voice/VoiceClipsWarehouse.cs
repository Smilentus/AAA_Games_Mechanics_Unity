using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "VoiceClipsWarehouse", menuName = "VoiceSystem/VoiceClipsWarehouse")]
public class VoiceClipsWarehouse : ScriptableObject
{
    [SerializeField]
    private List<BaseVoiceClipData> m_voiceClipData = new List<BaseVoiceClipData>();
    public List<BaseVoiceClipData> voiceClipData => m_voiceClipData;
}