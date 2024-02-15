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

        OnPossess();

        onPossessedEvent?.Invoke();
    }

    public virtual void PhaseOut()
    {
        if (!IsPossessed) return;

        SyncPossessionOnServerRpc(false);

        OnPhaseOut();

        onPhasedOutEvent?.Invoke();
    }


    [ServerRpc]
    protected void SyncPossessionOnServerRpc(bool _isPossessed)
    {
        isPossessed.Value = _isPossessed;
    }


    protected virtual void OnPossess() { }
    protected virtual void OnPhaseOut() { }
}
