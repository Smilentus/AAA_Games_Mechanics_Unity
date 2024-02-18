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
    private Transform leavePoint;


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

    public void SetLeavePoint(Transform _leavePoint)
    {
        leavePoint = _leavePoint;
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
            Debug.Log($"Кресло уже занято");
            if (ownerClientId == NetworkObject.OwnerClientId)
            {
                Debug.Log($"Освобождаем кресло, т.к. оно занято тем, кто с ним взаимодействует {ownerClientId}");
                LeaveFromPlaceAtServerRpc(ownerClientId);
            }
            else
            {
                Debug.Log($"Не смогли занять кресло, оно занято другим человеком!");
            }
        }
        else
        {
            Debug.Log($"Кресло занял новый человек! {ownerClientId}");
            OccupySeatAtServerRpc(ownerClientId);
        }
    }


    [ServerRpc(RequireOwnership = false)]
    public virtual void LeaveFromPlaceAtServerRpc(ulong ownerClientId)
    {
        NetworkObject.RemoveOwnership();

        IsOccupied.Value = false;

        NetworkManager.ConnectedClients[ownerClientId].PlayerObject.transform.SetPositionAndRotation(leavePoint.position, Quaternion.identity);
        NetworkManager.ConnectedClients[ownerClientId].PlayerObject.TryRemoveParent();

        OnSeatLeftByClientRpc(ownerClientId);
    }


    [ServerRpc(RequireOwnership = false)]
    public void OccupySeatAtServerRpc(ulong ownerClientId)
    {
        NetworkObject.ChangeOwnership(ownerClientId);

        IsOccupied.Value = true;

        NetworkManager.ConnectedClients[ownerClientId].PlayerObject.transform.SetPositionAndRotation(placementPoint.position, Quaternion.identity);
        NetworkManager.ConnectedClients[ownerClientId].PlayerObject.TrySetParent(this.transform);

        OnSeatOccupiedByClientRpc(ownerClientId);
    }


    [ClientRpc]
    public void OnSeatOccupiedByClientRpc(ulong ownerClientId)
    {
        if (NetworkObject.OwnerClientId == ownerClientId)
        {
            if (NetworkManager.ConnectedClients[ownerClientId].PlayerObject.TryGetComponent<FirstPersonInputHandler>(out FirstPersonInputHandler playerController))
            {
                playerController.RestrictMovements = true;
            }
        }
    }


    [ClientRpc]
    public void OnSeatLeftByClientRpc(ulong ownerClientId)
    {
        if (NetworkObject.OwnerClientId == ownerClientId)
        {
            if (NetworkManager.ConnectedClients[ownerClientId].PlayerObject.TryGetComponent<FirstPersonInputHandler>(out FirstPersonInputHandler playerController))
            {
                playerController.RestrictMovements = false;
            }
        }
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