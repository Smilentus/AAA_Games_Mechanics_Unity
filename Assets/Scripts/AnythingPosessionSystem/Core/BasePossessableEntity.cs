using System;
using UnityEngine;


public class BasePossessableEntity : MonoBehaviour, IPossessableEntity
{
    public event Action onPossessedEvent;
    public event Action onPhasedOutEvent;


    protected bool isPossessed;
    public bool IsPossessed => isPossessed;


    public virtual void Possess()
    {
        if (isPossessed) return;

        isPossessed = true;

        OnPossess();

        onPossessedEvent?.Invoke();
    }

    public virtual void PhaseOut()
    {
        if (!isPossessed) return;

        isPossessed = false;

        OnPhaseOut();

        onPhasedOutEvent?.Invoke();
    }


    protected virtual void OnPossess() { }
    protected virtual void OnPhaseOut() { }
}
