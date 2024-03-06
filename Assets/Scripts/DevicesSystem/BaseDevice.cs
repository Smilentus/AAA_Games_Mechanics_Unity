using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BaseDevice : MonoBehaviour
{
    [SerializeField]
    private DeviceProfile m_deviceProfile;
    public DeviceProfile DeviceProfile => m_deviceProfile;


    [SerializeField]
    private BaseHardwareMotherboard m_hardwareMotherboard;
    public BaseHardwareMotherboard HardwareMotherboard => m_hardwareMotherboard;


    private List<BaseDeviceApplication> _deviceApplications = new List<BaseDeviceApplication>();


    [ContextMenu("Debug_StartSystems")]
    public void StartSystems()
    {
        if (!m_hardwareMotherboard.IsMinimalVitalBlocksInstalled())
        {
            Debug.Log($"На материнской плате не установлены минимально необходимые для работы компоненты!");
        }
        else
        {
            Debug.Log($"Материнская плата содержит все необоходимые компоненты!");
        }
    }
}