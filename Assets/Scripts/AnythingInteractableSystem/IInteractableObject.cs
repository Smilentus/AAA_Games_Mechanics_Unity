using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IInteractableObject
{ 
    public bool CanInteract { get; set; }


    public void InteractWithObject(ulong ownerClientId);


    public void OnSelectionStarted();
    public void OnSelectionEnded();
}