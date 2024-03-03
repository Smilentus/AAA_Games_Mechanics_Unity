using System;
using UnityEngine;


public class TriggerableObject : MonoBehaviour
{
    public event Action<Collider> OnTriggerEnterEvent;
    public event Action<Collider> OnTriggerExitEvent;
    public event Action<Collider> OnTriggerStayEvent;


    protected virtual void OnTriggerEnter(Collider other)
    {
        OnTriggerEnterEvent?.Invoke(other);
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        OnTriggerExitEvent?.Invoke(other);
    }

    protected virtual void OnTriggerStay(Collider other)
    {
        OnTriggerStayEvent?.Invoke(other);
    }
}
