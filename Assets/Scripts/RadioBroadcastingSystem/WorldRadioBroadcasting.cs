using System.Collections.Generic;
using UnityEngine;


public class WorldRadioBroadcasting : MonoBehaviour
{
    private static WorldRadioBroadcasting _instance;
    public static WorldRadioBroadcasting Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<WorldRadioBroadcasting>();
            }

            return _instance;
        }
    }


    [SerializeField]
    private List<RuntimeWorldRadioStation> m_runtimeWorldRadioStations = new List<RuntimeWorldRadioStation>();
    public List<RuntimeWorldRadioStation> RuntimeWorldRadioStations => m_runtimeWorldRadioStations;


    public RadioStationSetupData TrySetUpRadioStationFromFrequency(float settingsFrequency)
    {
        foreach (RuntimeWorldRadioStation runtimeWorldRadioStation in m_runtimeWorldRadioStations)
        {
            FrequencyComparatorData frequencyComparatorData = runtimeWorldRadioStation.CompareFrequency(settingsFrequency);

            if (frequencyComparatorData.isSyncWithStation)
            {
                return new RadioStationSetupData()
                {
                    runtimeWorldRadioStation = runtimeWorldRadioStation,
                    signalPowerNormalized = Mathf.Clamp01(1 - Mathf.Abs(frequencyComparatorData.frequencyRatio))
                };
            }
        }

        return new RadioStationSetupData()
        {
            runtimeWorldRadioStation = null,
            signalPowerNormalized = 0
        };
    }
}

[System.Serializable]
public struct RadioStationSetupData
{
    public RuntimeWorldRadioStation runtimeWorldRadioStation;
    public float signalPowerNormalized;
}