using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class NetworkVehicleObject : NetworkBehaviour
{
    [SerializeField]
    private List<VehicleSeatSettings> m_defaultVehicleSeats = new List<VehicleSeatSettings>();


    protected NetworkVariable<bool> IsInitialized = new NetworkVariable<bool>();


    public override void OnNetworkSpawn()
    {
        if (!IsInitialized.Value)
        {
            Debug.Log($"On Vehicle Spawned");
            InitializeVehicle();
        }
    }

    // Пока как-то так
    // Подумать как нормально синхронизировать эту часть
    private void InitializeVehicle()
    {
        if (IsServer)
        {
            IsInitialized.Value = true;

            foreach (VehicleSeatSettings vehicleSeatSettings in m_defaultVehicleSeats)
            {
                GameObject vehicleSeatObject = Instantiate(vehicleSeatSettings.VehicleSeatProfile.SeatPrefab);

                if (vehicleSeatObject.TryGetComponent(out NetworkVehicleSeatObject networkVehicleSeatObject))
                {
                    networkVehicleSeatObject.SetPlacementPoint(vehicleSeatSettings.VehicleSeatPlacementPoint);
                }

                if (vehicleSeatObject.TryGetComponent(out NetworkObject networkSeatObject))
                {
                    networkSeatObject.Spawn();
                    networkSeatObject.TrySetParent(this.gameObject, false);
                }
            }
        }

        OnInitializeVehicle();
    }
    public virtual void OnInitializeVehicle() { }
}


[System.Serializable]
public class VehicleSeatSettings
{
    public Transform VehicleSeatPlacementPoint;

    public VehicleSeatProfile VehicleSeatProfile;
}