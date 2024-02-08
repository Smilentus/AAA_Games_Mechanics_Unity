using System.Collections.Generic;
using UnityEngine;


public class VoiceWarehouseController : MonoBehaviour
{
    private static VoiceWarehouseController instance;
    public static VoiceWarehouseController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<VoiceWarehouseController>(true);
            }

            return instance;
        }
    }


    [SerializeField]
    protected VoiceClipsWarehouse m_voiceClipsWarehouse;
    public VoiceClipsWarehouse VoiceClipWarehouse => m_voiceClipsWarehouse;


    private Dictionary<string, BaseVoiceClipData> voiceClipDataDictionary = new Dictionary<string, BaseVoiceClipData>();
    public Dictionary<string, BaseVoiceClipData> VoiceClipDataDictionary => voiceClipDataDictionary;


    private void Awake()
    {
        InitializeVoiceWarehouse();
    }


    public void InitializeVoiceWarehouse()
    {
        foreach (BaseVoiceClipData baseVoiceClipData in m_voiceClipsWarehouse.voiceClipData)
        {
            voiceClipDataDictionary.Add(baseVoiceClipData.VoiceClipGUID, baseVoiceClipData);
        }
    }

    public BaseVoiceClipData GetVoiceClipData(string voiceClipGUID)
    {
        if (voiceClipDataDictionary.ContainsKey(voiceClipGUID))
        {
            return voiceClipDataDictionary[voiceClipGUID];
        }
        else
        {
            return null;
        }
    }
}