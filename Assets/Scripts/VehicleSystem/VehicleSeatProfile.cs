using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "VehicleSeatProfile", menuName = "VehicleSystem/VehicleSeatProfile")]
public class VehicleSeatProfile : ScriptableObject
{
    [field: SerializeField]
    public string SeatName { get; set; }

    [field: SerializeField]
    public GameObject SeatPrefab;
}
