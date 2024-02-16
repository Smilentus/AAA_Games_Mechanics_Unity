using System;
using Unity.Netcode;
using UnityEngine;


public class NetworkPossessableEntity : NetworkBehaviour, IPossessableEntity
{
    public event Action onPossessedEvent;
    public event Action onPhasedOutEvent;


    protected NetworkVariable<bool> isPossessed = new NetworkVariable<bool>();
    public bool IsPossessed => isPossessed.Value;


    public virtual void Possess()
    {
        if (IsPossessed) return;

        SyncPossessionOnServerRpc(true);
    }

    public virtual void PhaseOut()
    {
        if (!IsPossessed) return;

        SyncPossessionOnServerRpc(false);
    }


    [ServerRpc(RequireOwnership = false)]
    protected void SyncPossessionOnServerRpc(bool _isPossessed)
    {
        isPossessed.Value = _isPossessed;

        if (isPossessed.Value)
        {
            OnSyncPossesAtClientRpc();
        }
        else
        {
            OnSyncPhaseOutAtClientRpc();
        }
    }

    [ClientRpc]
    protected void OnSyncPossesAtClientRpc()
    {
        OnPossess();

        onPossessedEvent?.Invoke();
    }

    [ClientRpc]
    protected void OnSyncPhaseOutAtClientRpc()
    {
        OnPhaseOut();

        onPhasedOutEvent?.Invoke();
    }


    protected virtual void OnPossess() { }
    protected virtual void OnPhaseOut() { }
}
