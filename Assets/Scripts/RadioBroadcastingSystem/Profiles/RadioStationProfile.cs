using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "RadioStationProfile", menuName = "RadioBroadcastingSystem/RadioStationProfile")]
public class RadioStationProfile : ScriptableObject
{
    [field: SerializeField]
    public string StationTitle { get; protected set; }


    [Tooltip("Центральная частота станции (100% чистый сигнал)")]
    [Range(10, 1000)]
    [SerializeField]
    private float m_stationFrequence;
    public float StationFrequence => m_stationFrequence;

    [Tooltip("Доступный разброс частоты для вещания (- и + в обе стороны)")]
    [Range(0, 10)]
    [SerializeField]
    private float m_stationFrequenceThreshold = 1f;
    public float StationFrequenceThresholdThreshold => m_stationFrequenceThreshold;

    // ???
    //[Tooltip("Процентное соотношение 'чистого 100%' сигнала в доступном диапазоне частот (отсчитывается от доступного разброса частоты)")]
    //[Range(0, 1)]
    //[SerializeField]
    //private float m_stationClearFrequenceThreshold = 0.1f;


    [field: SerializeField]
    public List<RadioBroadcastingProgramProfile> BroadcastingPrograms { get; protected set; }
}