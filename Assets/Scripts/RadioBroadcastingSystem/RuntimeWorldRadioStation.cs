using System;
using UnityEngine;


public class RuntimeWorldRadioStation : MonoBehaviour
{
    public event Action OnBroadcastingProgramChanged;
    public event Action OnBroadcastingProgramPartChanged;


    [SerializeField]
    private RadioStationProfile m_radioStationProfile;
    public RadioStationProfile RadioStationProfile => m_radioStationProfile;


    private float _totalBroadcastingSeconds;


    public RadioBroadcastingProgramProfile CurrentBroadcastingProgram => m_radioStationProfile.BroadcastingPrograms[_currentBroadcastingProgramIndex];
    public BaseRadioBroadcastingProgramPart CurrentBroadcastingProgramPart => CurrentBroadcastingProgram.ProgramParts[_currentBroadcastingProgramPartIndex];


    private int _currentBroadcastingProgramIndex;
    private int _currentBroadcastingProgramPartIndex;


    private float _programPartBroadcastingSeconds;
    public float ProgramPartBroadcastingSeconds => _programPartBroadcastingSeconds;


    private bool _isBroadcasting;


    private void Start()
    {
        CalculateTotalBroadcastingLength();
        StartBroadcasting();
    }

    private void FixedUpdate()
    {
        if (!_isBroadcasting) return;

        _programPartBroadcastingSeconds += Time.fixedDeltaTime;

        CheckAvailablePrograms();
    }


    private void StartBroadcasting()
    {
        ResetBroadcasting();

        _isBroadcasting = true;
    }

    private void ResetBroadcasting()
    {
        if (m_radioStationProfile == null || m_radioStationProfile.BroadcastingPrograms.Count == 0) return;

        _programPartBroadcastingSeconds = 0;

        _currentBroadcastingProgramIndex = 0;
        _currentBroadcastingProgramPartIndex = 0;

        OnBroadcastingProgramChanged?.Invoke();
        OnBroadcastingProgramPartChanged?.Invoke();
    }

    private void CheckAvailablePrograms()
    {
        if (_programPartBroadcastingSeconds >= CurrentBroadcastingProgramPart.GetProgramPartSecondsLength())
        {
            _programPartBroadcastingSeconds = CurrentBroadcastingProgramPart.GetProgramPartSecondsLength();

            if (IsNextProgramPartAvailable())
            {
                // Программа продолжается - запускаем следующую часть программы
                ApplyNextProgramPart();
            }
            else
            {
                if (IsNextProgramAvailable())
                {
                    // Программа закончилась - свапаем на следующую программу
                    ApplyNextProgram();
                }
                else
                {
                    // Программа была последней - запускаем круг радио-вещания по новой
                    ResetBroadcasting();
                }
            }
        }
    }

    private bool IsNextProgramPartAvailable()
    {
        return _currentBroadcastingProgramPartIndex + 1 < CurrentBroadcastingProgram.ProgramParts.Count;
    }
    
    private void ApplyNextProgramPart()
    {
        _currentBroadcastingProgramPartIndex++;
        _programPartBroadcastingSeconds = 0;

        OnBroadcastingProgramPartChanged?.Invoke();
    }


    private bool IsNextProgramAvailable()
    {
        return _currentBroadcastingProgramIndex + 1 < m_radioStationProfile.BroadcastingPrograms.Count;
    }
    
    private void ApplyNextProgram()
    {
        _currentBroadcastingProgramIndex++;

        _currentBroadcastingProgramPartIndex = 0;
        _programPartBroadcastingSeconds = 0;

        OnBroadcastingProgramChanged?.Invoke();
        OnBroadcastingProgramPartChanged?.Invoke();
    }


    public void CalculateTotalBroadcastingLength()
    {
        float totalSeconds = 0;

        foreach (RadioBroadcastingProgramProfile radioBroadcastingProgram in m_radioStationProfile.BroadcastingPrograms)
        {
            totalSeconds += radioBroadcastingProgram.ProgramLength;
        }

        _totalBroadcastingSeconds = totalSeconds;
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