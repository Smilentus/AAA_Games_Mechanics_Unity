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

    [Range(0, 1)]
    public float RadioVolume = 1f;
    
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
        if (Input.GetKeyDown(KeyCode.W))
        {
            RadioVolume += 0.05f;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            RadioVolume -= 0.05f;
        }

        RadioVolume = Mathf.Clamp01(RadioVolume);

        RadioStationSetupData setupData = TrySetUpRadioStationFromFrequency(RadioFrequency);

        //Debug.Log($"Signal Power {setupData.signalPowerNormalized.ToString("f4")} with radio station {(setupData.runtimeWorldRadioStation == null ? "Unknown" : setupData.runtimeWorldRadioStation.RadioStationProfile.StationTitle)}");

        whiteNoiseAudioSource.volume = (1 - setupData.signalPowerNormalized) * 0.5f;
        radioAudioSource.volume = setupData.signalPowerNormalized;

        if (setupData.runtimeWorldRadioStation != null)
        {
            if (syncedWorldRadioStation != null)
            {
                if (syncedWorldRadioStation.Equals(setupData.runtimeWorldRadioStation))
                {
                    ProcessCurrentBroadcastingProgram();
                }
                else
                {
                    UnSubscribeFromRadioStation();

                    syncedWorldRadioStation = setupData.runtimeWorldRadioStation;

                    SubscribeToRadioStation();

                    ParseCurrentBroadcastingProgram();
                }
            }
            else
            {
                syncedWorldRadioStation = setupData.runtimeWorldRadioStation;

                SubscribeToRadioStation();

                ParseCurrentBroadcastingProgram();
            }
        }
        else
        {
            UnSubscribeFromRadioStation();
            syncedWorldRadioStation = null;
        }
    }

    private void ParseCurrentBroadcastingProgram()
    {
        // Тут различные парсеры радио-передач будут
        if (syncedWorldRadioStation.CurrentBroadcastingProgramPart is MusicRadioBroadcastingProgramPart)
        {
            MusicRadioBroadcastingProgramPart programPart = syncedWorldRadioStation.CurrentBroadcastingProgramPart as MusicRadioBroadcastingProgramPart;

            radioAudioSource.Stop();

            PlayableRadioMusicData playableRadioMusicData = programPart.GetPlayableDataByTotalLength(syncedWorldRadioStation.ProgramPartBroadcastingSeconds);

            // Тут ещё будем получать временное отклонение, пока мы не слушали радио
            radioAudioSource.clip = playableRadioMusicData.Clip;
            radioAudioSource.time = playableRadioMusicData.PassedTime;

            radioAudioSource.Play();
        }
        else
        {
            radioAudioSource.Stop();
            radioAudioSource.clip = null;
        }
    }

    private void ProcessCurrentBroadcastingProgram()
    {
        if (syncedWorldRadioStation.CurrentBroadcastingProgramPart is MusicRadioBroadcastingProgramPart)
        {
            MusicRadioBroadcastingProgramPart programPart = syncedWorldRadioStation.CurrentBroadcastingProgramPart as MusicRadioBroadcastingProgramPart;

            PlayableRadioMusicData playableRadioMusicData = programPart.GetPlayableDataByTotalLength(syncedWorldRadioStation.ProgramPartBroadcastingSeconds);

            if (radioAudioSource.clip.Equals(playableRadioMusicData.Clip)) return;

            radioAudioSource.Stop();

            // Тут ещё будем получать временное отклонение, пока мы не слушали радио
            radioAudioSource.clip = playableRadioMusicData.Clip;
            radioAudioSource.time = playableRadioMusicData.PassedTime;

            radioAudioSource.Play();
        }
    }


    private void SubscribeToRadioStation()
    {
        if (syncedWorldRadioStation == null) return;

        syncedWorldRadioStation.OnBroadcastingProgramPartChanged += ParseCurrentBroadcastingProgram;
    }

    private void UnSubscribeFromRadioStation()
    {
        if (syncedWorldRadioStation == null) return;

        syncedWorldRadioStation.OnBroadcastingProgramPartChanged -= ParseCurrentBroadcastingProgram;
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