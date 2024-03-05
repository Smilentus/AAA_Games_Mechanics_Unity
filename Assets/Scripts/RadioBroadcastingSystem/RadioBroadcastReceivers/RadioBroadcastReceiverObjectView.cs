using DG.Tweening;
using TMPro;
using UnityEngine;


// TODO: Разбить класс на подклассы
public class RadioBroadcastReceiverObjectView : MonoBehaviour
{
    [SerializeField]
    private RadioBroadcastReceiver m_radioBroadcastRecevier;


    [SerializeField]
    private Transform m_movableTuneLine;

    [SerializeField]
    private Vector3 m_movableAxis;

    [SerializeField]
    private Vector3 m_tuneLineMinPoint;

    [SerializeField]
    private Vector3 m_tuneLineMaxPoint;


    [SerializeField]
    private Transform m_rotatableFrequencyTuner;

    [SerializeField]
    private Vector3 m_frequencyTunerAngleAxis;

    [SerializeField]
    private float m_frequencyMinAngle;

    [SerializeField]
    private float m_frequencyMaxAngle;


    [SerializeField]
    private Transform m_rotatableVolumeTuner;

    [SerializeField]
    private Vector3 m_volumeTunerAngleAxis;

    [SerializeField]
    private float m_volumeMinAngle;

    [SerializeField]
    private float m_volumeMaxAngle;


    [SerializeField]
    private Transform m_toggleButton;

    [SerializeField]
    private Vector3 m_toggleButtonOnAngle;

    [SerializeField]
    private Vector3 m_toggleButtonOffAngle;


    [SerializeField]
    private TMP_Text m_settingsFrequencyTMP;


    private Quaternion volumeStartRotation;
    private Quaternion frequencyStartRotation;


    private Tweener smoothTuneLineTweener;
    private Tweener smoothVolumeTunerTweener;
    private Tweener smoothFreqTunerTweener;


    private void Start()
    {
        volumeStartRotation = m_rotatableVolumeTuner.localRotation;
        frequencyStartRotation = m_rotatableFrequencyTuner.localRotation;

        m_radioBroadcastRecevier.onReceiverEnableStatusChanged += OnReceiverStatusChanged;
        OnReceiverStatusChanged(m_radioBroadcastRecevier.IsReceiverEnabled);
    }

    private void OnReceiverStatusChanged(bool status)
    {
        if (status)
        {
            m_toggleButton.localEulerAngles = m_toggleButtonOnAngle;
        }
        else
        {
            m_toggleButton.localEulerAngles = m_toggleButtonOffAngle;
        }
    }

    private void FixedUpdate()
    {
        float normalizedAngle = m_volumeMinAngle + (m_volumeMaxAngle - m_volumeMinAngle) * m_radioBroadcastRecevier.Volume;

        if (smoothVolumeTunerTweener != null)
        {
            smoothVolumeTunerTweener.Kill();
        }

        smoothVolumeTunerTweener = m_rotatableVolumeTuner.DOLocalRotateQuaternion(volumeStartRotation * Quaternion.AngleAxis(normalizedAngle, m_volumeTunerAngleAxis), Time.fixedDeltaTime);

        float normalizedFrequencyAngle = m_frequencyMinAngle + (m_frequencyMaxAngle - m_frequencyMinAngle) * m_radioBroadcastRecevier.FrequencyRatio;

        if (smoothFreqTunerTweener != null)
        {
            smoothFreqTunerTweener.Kill();
        }

        smoothFreqTunerTweener = m_rotatableFrequencyTuner.DOLocalRotateQuaternion(frequencyStartRotation * Quaternion.AngleAxis(normalizedFrequencyAngle, m_frequencyTunerAngleAxis), Time.fixedDeltaTime);

        if (smoothTuneLineTweener != null)
        {
            smoothTuneLineTweener.Kill();
        }

        smoothTuneLineTweener = m_movableTuneLine.transform.DOLocalMove(m_tuneLineMinPoint + (m_tuneLineMaxPoint - m_tuneLineMinPoint) * m_radioBroadcastRecevier.FrequencyRatio, Time.fixedDeltaTime);

        m_settingsFrequencyTMP.text = m_radioBroadcastRecevier.RadioFrequency.ToString("f3");
    }
}