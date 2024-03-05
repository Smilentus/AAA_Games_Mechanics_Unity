using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "RadioStationProfile", menuName = "RadioBroadcastingSystem/RadioStationProfile")]
public class RadioStationProfile : ScriptableObject
{
    [field: SerializeField]
    public string StationTitle { get; protected set; }


    [Tooltip("����������� ������� ������� (100% ������ ������)")]
    [Range(10, 1000)]
    [SerializeField]
    private float m_stationFrequence;
    public float StationFrequence => m_stationFrequence;

    [Tooltip("��������� ������� ������� ��� ������� (- � + � ��� �������)")]
    [Range(0, 10)]
    [SerializeField]
    private float m_stationFrequenceThreshold = 1f;
    public float StationFrequenceThresholdThreshold => m_stationFrequenceThreshold;

    // ???
    //[Tooltip("���������� ����������� '������� 100%' ������� � ��������� ��������� ������ (������������� �� ���������� �������� �������)")]
    //[Range(0, 1)]
    //[SerializeField]
    //private float m_stationClearFrequenceThreshold = 0.1f;


    [field: SerializeField]
    public List<RadioBroadcastingProgramProfile> BroadcastingPrograms { get; protected set; }
}