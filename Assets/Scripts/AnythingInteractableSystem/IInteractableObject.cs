using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractableObject
{
    public void InteractWithObject(ulong ownerClientId);


    public void OnInteractableSelectionStarted();
    public void OnInteractableSelectionEnded();
}