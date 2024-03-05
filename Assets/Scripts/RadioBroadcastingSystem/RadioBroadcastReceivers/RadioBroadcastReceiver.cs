using System;
using UnityEngine;


public class RadioBroadcastReceiver : MonoBehaviour
{
    public event Action<bool> onReceiverEnableStatusChanged;


    [SerializeField]
    private AudioSource m_radioMainAudioSource;

    [SerializeField]
    private AudioSource m_whiteNoiseAudioSource;

    [SerializeField]
    private AudioSource m_toggleAudioSource;


    private RuntimeWorldRadioStation _syncedWorldRadioStation;


    [SerializeField]
    private float m_minAvailableFrequency;

    [SerializeField]
    private float m_maxAvailableFrequency;


    public float FrequencyRatio => (_radioFrequency - m_minAvailableFrequency) / (m_maxAvailableFrequency - m_minAvailableFrequency);


    private float _radioFrequency = 65;
    public float RadioFrequency => _radioFrequency;


    private float _volume = 1f;
    public float Volume
    {
        get => _volume;
        protected set
        {
            _volume = value;

            _volume = Mathf.Clamp01(_volume);
        }
    }


    private bool _isReceiverEnabled;
    public bool IsReceiverEnabled => _isReceiverEnabled;


    private void OnDestroy()
    {
        UnSubscribeFromRadioStation();
    }


    private void FixedUpdate()
    {
        ClampFrequency();

        if (!_isReceiverEnabled) return;

        RadioStationSetupData setupData = WorldRadioBroadcasting.Instance.TrySetUpRadioStationFromFrequency(_radioFrequency);

        //Debug.Log($"Signal Power {setupData.signalPowerNormalized.ToString("f4")} with radio station {(setupData.runtimeWorldRadioStation == null ? "Unknown" : setupData.runtimeWorldRadioStation.RadioStationProfile.StationTitle)}");
        
        ApplyVolumes(setupData);
        ReceiveBroadcasting(setupData);
    }


    private void ClampFrequency()
    {
        _radioFrequency = Mathf.Clamp(_radioFrequency, m_minAvailableFrequency, m_maxAvailableFrequency);
    }

    private void ApplyVolumes(RadioStationSetupData setupData)
    {
        m_whiteNoiseAudioSource.volume = Mathf.Clamp01((1 - setupData.signalPowerNormalized - 0.1f) * 0.75f * _volume);
        m_radioMainAudioSource.volume = Mathf.Clamp01(setupData.signalPowerNormalized * _volume);
    }

    private void ReceiveBroadcasting(RadioStationSetupData setupData)
    {
        if (!_isReceiverEnabled) return;

        if (setupData.runtimeWorldRadioStation != null)
        {
            if (_syncedWorldRadioStation != null)
            {
                if (_syncedWorldRadioStation.Equals(setupData.runtimeWorldRadioStation))
                {
                    ProcessCurrentBroadcastingProgram();
                }
                else
                {
                    UnSubscribeFromRadioStation();

                    _syncedWorldRadioStation = setupData.runtimeWorldRadioStation;

                    SubscribeToRadioStation();

                    ProcessCurrentBroadcastingProgram();
                }
            }
            else
            {
                _syncedWorldRadioStation = setupData.runtimeWorldRadioStation;

                SubscribeToRadioStation();

                ProcessCurrentBroadcastingProgram();
            }
        }
        else
        {
            UnSubscribeFromRadioStation();
            _syncedWorldRadioStation = null;
        }
    }

    private void ProcessCurrentBroadcastingProgram()
    {
        if (!_isReceiverEnabled) return;

        if (_syncedWorldRadioStation.CurrentBroadcastingProgramPart is MusicRadioBroadcastingProgramPart)
        {
            MusicRadioBroadcastingProgramPart programPart = _syncedWorldRadioStation.CurrentBroadcastingProgramPart as MusicRadioBroadcastingProgramPart;

            PlayableRadioMusicData playableRadioMusicData = programPart.GetPlayableDataByTotalLength(_syncedWorldRadioStation.ProgramPartBroadcastingSeconds);
            
            // Ёто место не нравитс€, надо бы другую проверку сюдык
            if (m_radioMainAudioSource.clip != null && m_radioMainAudioSource.clip.Equals(playableRadioMusicData.Clip)) return;

            m_radioMainAudioSource.Stop();

            m_radioMainAudioSource.clip = playableRadioMusicData.Clip;
            m_radioMainAudioSource.time = playableRadioMusicData.PassedTime;

            m_radioMainAudioSource.Play();
        }
        else
        {
            m_radioMainAudioSource.Stop();
            m_radioMainAudioSource.clip = null;
        }
    }


    public void IncreaseVolume(float volume)
    {
        Volume += volume;
    }

    public void DecreaseVolume(float volume)
    {
        Volume -= volume;
    }
    
    public void SetVolume(float volume)
    {
        Volume = volume;
    }
    

    public void IncreaseFrequency(float frequency)
    {
        _radioFrequency += frequency;
    }

    public void DecreaseFrequency(float frequency)
    {
        _radioFrequency -= frequency;
    }

    public void SetFrequency(float frequency)
    {
        _radioFrequency = frequency;
    }


    private void ClearReceiver()
    {
        _syncedWorldRadioStation = null;

        m_radioMainAudioSource.clip = null;
        m_radioMainAudioSource.Stop();
    }


    public void EnableReceiverDevice()
    {
        ClearReceiver();

        _isReceiverEnabled = true;

        m_toggleAudioSource.Play();

        m_whiteNoiseAudioSource.Play();

        onReceiverEnableStatusChanged?.Invoke(true);
    }

    public void DisableReceiverDevice()
    {
        _isReceiverEnabled = false;

        ClearReceiver();

        m_whiteNoiseAudioSource.Stop();

        m_toggleAudioSource.Play();

        onReceiverEnableStatusChanged?.Invoke(false);
    }

    public void ToggleReceiverDevice()
    {
        if (_isReceiverEnabled)
        {
            DisableReceiverDevice();
        }
        else
        {
            EnableReceiverDevice();
        }
    }


    private void SubscribeToRadioStation()
    {
        if (_syncedWorldRadioStation == null) return;

        _syncedWorldRadioStation.OnBroadcastingProgramPartChanged += ProcessCurrentBroadcastingProgram;
    }

    private void UnSubscribeFromRadioStation()
    {
        if (_syncedWorldRadioStation == null) return;

        _syncedWorldRadioStation.OnBroadcastingProgramPartChanged -= ProcessCurrentBroadcastingProgram;
    }
}