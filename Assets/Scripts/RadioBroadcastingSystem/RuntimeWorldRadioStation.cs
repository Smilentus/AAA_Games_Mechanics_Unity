using System;
using UnityEngine;


public class RuntimeWorldRadioStation : MonoBehaviour
{
    public event Action OnBroadcastingProgramChanged;


    [SerializeField]
    private RadioStationProfile m_radioStationProfile;
    public RadioStationProfile RadioStationProfile => m_radioStationProfile;


    private float generalBroadcastingSeconds;
    private float totalBroadcastingSeconds;


    private BaseRadioBroadcastingProgramPart currentBroadcastingProgramPart;
    private float programBroadcastingSeconds;


    private bool isBroadcasting;


    private void Start()
    {
        generalBroadcastingSeconds = 0;
        CalculateTotalBroadcastingLength();
        StartBroadcasting();
    }

    private void FixedUpdate()
    {
        if (!isBroadcasting) return;

        generalBroadcastingSeconds += Time.fixedDeltaTime;

        if (generalBroadcastingSeconds >= totalBroadcastingSeconds)
        {
            generalBroadcastingSeconds = 0;
            // debug
            OnBroadcastingProgramChanged?.Invoke();
        }
    }


    private void StartBroadcasting()
    {
        // debug
        isBroadcasting = true;
        currentBroadcastingProgramPart = m_radioStationProfile.BroadcastingPrograms[0].ProgramParts[0];
        OnBroadcastingProgramChanged?.Invoke();
    }

    public void CalculateTotalBroadcastingLength()
    {
        float totalSeconds = 0;

        foreach (RadioBroadcastingProgramProfile radioBroadcastingProgram in m_radioStationProfile.BroadcastingPrograms)
        {
            foreach (BaseRadioBroadcastingProgramPart programPart in radioBroadcastingProgram.ProgramParts)
            {
                totalSeconds += programPart.GetProgramPartSecondsLength();
            }
        }

        totalBroadcastingSeconds = totalSeconds;
    }


    public BroadcastingProgramData GetCurrentBroadcastingProgram()
    {
        return new BroadcastingProgramData()
        {
            broadcastingProgramPart = currentBroadcastingProgramPart,
            generalBroadcastingSeconds = generalBroadcastingSeconds,
            programBroadcastingSeconds = programBroadcastingSeconds
        };
    }

    public FrequencyComparatorData CompareFrequency(float settingsFrequency)
    {
        FrequencyComparatorData frequencyComparatorData = new FrequencyComparatorData();

        if (settingsFrequency >= m_radioStationProfile.StationFrequence - m_radioStationProfile.StationFrequenceThresholdThreshold &&
            settingsFrequency <= m_radioStationProfile.StationFrequence + m_radioStationProfile.StationFrequenceThresholdThreshold)
        {
            frequencyComparatorData.isSyncWithStation = true;

            float normalizedFrequency = NormalizeFrequency(
                minFreq: m_radioStationProfile.StationFrequence - m_radioStationProfile.StationFrequenceThresholdThreshold,
                maxFreq: m_radioStationProfile.StationFrequence + m_radioStationProfile.StationFrequenceThresholdThreshold,
                curFreq: settingsFrequency);

            frequencyComparatorData.frequencyRatio = normalizedFrequency;

            //Debug.Log($"frequencyComparatorData.frequencyRatio => {frequencyComparatorData.frequencyRatio}");
        }
        else
        {
            frequencyComparatorData.isSyncWithStation = false;
            frequencyComparatorData.frequencyRatio = 0;
        }

        return frequencyComparatorData;
    }

    private float NormalizeFrequency(float minFreq, float maxFreq, float curFreq)
    {
        return 2 * ((curFreq - minFreq) / (maxFreq - minFreq)) - 1;
    }
}

[System.Serializable]
public struct FrequencyComparatorData
{
    public bool isSyncWithStation;
    public float frequencyRatio;
}

[System.Serializable]
public struct BroadcastingProgramData
{
    public BaseRadioBroadcastingProgramPart broadcastingProgramPart;

    public float generalBroadcastingSeconds;
    public float programBroadcastingSeconds;
}