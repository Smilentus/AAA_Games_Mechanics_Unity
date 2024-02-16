using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class NetworkVehicleSeatObject : NetworkBehaviour, IInteractableObject
{
    [field: SerializeField]
    public VehicleSeatProfile AttachedVehicleSeatProfile;


    private NetworkVariable<bool> IsOccupied = new NetworkVariable<bool>();
    public ulong OccupiedOwnerID => NetworkObject.OwnerClientId;


    private NetworkVehicleObject attachedVehicleObject;
    private Transform placementPoint;


    public override void OnNetworkSpawn()
    {
        TryAttachVehicleObject();

        if (placementPoint != null)
        {
            this.transform.SetPositionAndRotation(placementPoint.position, placementPoint.rotation);
        }
    }

    public override void OnNetworkObjectParentChanged(NetworkObject parentNetworkObject)
    {
        TryAttachVehicleObject();

        if (placementPoint != null)
        {
            this.transform.SetPositionAndRotation(placementPoint.position, placementPoint.rotation);
        }
    }


    private void TryAttachVehicleObject()
    {
        if (attachedVehicleObject == null)
        {
            NetworkVehicleObject networkVehicleObject = GetComponentInParent<NetworkVehicleObject>();

            if (networkVehicleObject != null)
            {
                attachedVehicleObject = networkVehicleObject;
            }
        }
    }


    public void SetPlacementPoint(Transform _placementPoint)
    {
        placementPoint = _placementPoint;
    }


    public virtual void TryInteractWithSeat(ulong ownerClientId)
    {
        TryInteractWithSeatAtServerRpc(ownerClientId);
    }


    [ServerRpc(RequireOwnership = false)]
    public virtual void TryInteractWithSeatAtServerRpc(ulong ownerClientId)
    {
        if (IsOccupied.Value)
        {
            Debug.Log($"������ ��� ������");
            if (ownerClientId == NetworkObject.OwnerClientId)
            {
                Debug.Log($"����������� ������, �.�. ��� ������ ���, ��� � ��� ���������������");
                LeaveFromPlaceAtServerRpc(ownerClientId);
            }
            else
            {
                Debug.Log($"�� ������ ������ ������, ��� ������ ������ ���������!");
            }
        }
        else
        {
            Debug.Log($"������ ����� ����� �������!");
            OccupySeatAtServerRpc(ownerClientId);
        }
    }


    [ServerRpc(RequireOwnership = false)]
    public virtual void LeaveFromPlaceAtServerRpc(ulong ownerClientId)
    {
        NetworkObject.RemoveOwnership();

        IsOccupied.Value = false;

        OnSeatLeftByClientRpc();
    }


    [ServerRpc(RequireOwnership = false)]
    public void OccupySeatAtServerRpc(ulong ownerClientId)
    {
        NetworkObject.ChangeOwnership(ownerClientId);

        IsOccupied.Value = true;

        OnSeatOccupiedByClientRpc(ownerClientId);
    }


    [ClientRpc]
    public void OnSeatOccupiedByClientRpc(ulong ownerClientId)
    {
        
    }

    [ClientRpc]
    public void OnSeatLeftByClientRpc()
    {

    }

    public void InteractWithObject(ulong ownerClientId)
    {
        Debug.Log($"Interact with Seat {this.gameObject.name}", this.gameObject);
        TryInteractWithSeat(ownerClientId);
    }


    public void OnInteractableSelectionStarted()
    {
        
    }

    public void OnInteractableSelectionEnded()
    {

    }
}