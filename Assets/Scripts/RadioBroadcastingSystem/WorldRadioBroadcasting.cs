using System.Collections.Generic;
using UnityEngine;


public class WorldRadioBroadcasting : MonoBehaviour
{
    [SerializeField]
    private List<RuntimeWorldRadioStation> m_runtimeWorldRadioStations = new List<RuntimeWorldRadioStation>();
    public List<RuntimeWorldRadioStation> RuntimeWorldRadioStations => m_runtimeWorldRadioStations;


    // Debug Area

    public AudioSource radioAudioSource;
    public AudioSource whiteNoiseAudioSource;

    public float FreqSettings = 0.05f;
    public float RadioFrequency = 65;

    private RuntimeWorldRadioStation syncedWorldRadioStation;


    private void Start()
    {
        whiteNoiseAudioSource.Play();
    }

    private void OnDestroy()
    {
        UnSubscribeFromRadioStation();
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            RadioFrequency -= FreqSettings;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            RadioFrequency += FreqSettings;
        }

        RadioStationSetupData setupData = TrySetUpRadioStationFromFrequency(RadioFrequency);

        //Debug.Log($"Signal Power {setupData.signalPowerNormalized.ToString("f4")} with radio station {(setupData.runtimeWorldRadioStation == null ? "Unknown" : setupData.runtimeWorldRadioStation.RadioStationProfile.StationTitle)}");

        whiteNoiseAudioSource.volume = (1 - setupData.signalPowerNormalized) * 0.5f;
        radioAudioSource.volume = setupData.signalPowerNormalized;

        if (setupData.runtimeWorldRadioStation != null)
        {
            if (syncedWorldRadioStation != null && syncedWorldRadioStation.Equals(setupData.runtimeWorldRadioStation)) return;

            syncedWorldRadioStation = setupData.runtimeWorldRadioStation;

            SubscribeToRadioStation();
            ParseCurrentBroadcastingProgram();
        }
        else
        {
            UnSubscribeFromRadioStation();
            syncedWorldRadioStation = null;
        }
    }

    private void ParseCurrentBroadcastingProgram()
    {
        BroadcastingProgramData broadcastingProgramData = syncedWorldRadioStation.GetCurrentBroadcastingProgram();

        // Тут различные парсеры радио-передач будут
        if (broadcastingProgramData.broadcastingProgramPart is MusicRadioBroadcastingProgramPart)
        {
            MusicRadioBroadcastingProgramPart programPart = broadcastingProgramData.broadcastingProgramPart as MusicRadioBroadcastingProgramPart;

            radioAudioSource.Stop();

            // Тут ещё будем получать временное отклонение, пока мы не слушали радио
            radioAudioSource.clip = programPart.MusicAudioClips[0];
            radioAudioSource.time = broadcastingProgramData.generalBroadcastingSeconds;

            radioAudioSource.Play();
        }
        else
        {
            radioAudioSource.Stop();
            radioAudioSource.clip = null;
        }
    }

    private void SubscribeToRadioStation()
    {
        if (syncedWorldRadioStation == null) return;

        syncedWorldRadioStation.OnBroadcastingProgramChanged += ParseCurrentBroadcastingProgram;
    }

    private void UnSubscribeFromRadioStation()
    {
        if (syncedWorldRadioStation == null) return;

        syncedWorldRadioStation.OnBroadcastingProgramChanged -= ParseCurrentBroadcastingProgram;
    }

    // End of Debug Area


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