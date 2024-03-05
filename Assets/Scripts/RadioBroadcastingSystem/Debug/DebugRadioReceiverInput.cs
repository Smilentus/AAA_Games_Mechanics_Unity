using UnityEngine;


public class DebugRadioReceiverInput : MonoBehaviour
{
    [SerializeField]
    private RadioBroadcastReceiver radioBroadcastReceiver;


    [SerializeField]
    private float FreqSettings = 0.001f;

    [SerializeField]
    private float VolumeSettings = 0.001f;

    [SerializeField]
    private float FreqShiftModifier = 10f;

    [SerializeField]
    private float VolumeShiftModifier = 5f;


    private void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            radioBroadcastReceiver.DecreaseFrequency(Input.GetKey(KeyCode.LeftShift) ? FreqSettings * FreqShiftModifier : FreqSettings);
        }
        if (Input.GetKey(KeyCode.D))
        {
            radioBroadcastReceiver.IncreaseFrequency(Input.GetKey(KeyCode.LeftShift) ? FreqSettings * FreqShiftModifier : FreqSettings);
        }

        if (Input.GetKey(KeyCode.W))
        {
            radioBroadcastReceiver.IncreaseVolume(Input.GetKey(KeyCode.LeftShift) ? VolumeSettings * VolumeShiftModifier : VolumeSettings);
        }
        if (Input.GetKey(KeyCode.S))
        {
            radioBroadcastReceiver.DecreaseVolume(Input.GetKey(KeyCode.LeftShift) ? VolumeSettings * VolumeShiftModifier : VolumeSettings);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            radioBroadcastReceiver.ToggleReceiverDevice();
        }
    }
}
