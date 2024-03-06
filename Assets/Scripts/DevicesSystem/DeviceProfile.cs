using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DeviceProfile", menuName = "DeviceSystem/New DeviceProfile")]
public class DeviceProfile : ScriptableObject
{
    [field: SerializeField]
    public string DeviceGUID { get; protected set; }


    [field: SerializeField]
    public string DeviceName { get; protected set; }


    [TextArea(5, 10)]
    [SerializeField]
    private string m_deviceDescription;
    public string DeviceDescription => m_deviceDescription;
}