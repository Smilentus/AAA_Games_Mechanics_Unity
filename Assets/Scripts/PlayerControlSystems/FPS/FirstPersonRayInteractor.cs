using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;


public class FirstPersonRayInteractor : NetworkBehaviour
{
    [SerializeField]
    private Transform m_raycastingPoint;


    [SerializeField]
    private InputActionReference m_interactButton;


    [SerializeField]
    private float m_interactionDistance = 3f;


    [SerializeField]
    private bool m_isAllowedToInteractAtAwake = false;


    private bool isAllowedToInteract;
    public bool IsAllowedToInteract => isAllowedToInteract;


    private IInteractableObject selectedInteractable;


    private void Awake()
    {
        if (m_isAllowedToInteractAtAwake)
        {
            isAllowedToInteract = true;
        }

        m_interactButton.action.Enable();
    }


    private void Update()
    {
        if (!IsLocalPlayer) return;
        if (!isAllowedToInteract) return;

        RaycastInteractables();

        if (m_interactButton.action.WasPerformedThisFrame())
        {
            if (selectedInteractable != null)
            {
                Debug.Log($"Interact with object {selectedInteractable.GetType()}");
                selectedInteractable.InteractWithObject(OwnerClientId);
            }
        }
    }

    private void RaycastInteractables()
    {
        Ray interactionRay = new Ray(m_raycastingPoint.position, m_raycastingPoint.forward);

        if (Physics.Raycast(interactionRay, out RaycastHit hitInfo, m_interactionDistance))
        {
            if (hitInfo.collider != null)
            {
                IInteractableObject interactableObject = hitInfo.collider.GetComponent<IInteractableObject>();

                if (interactableObject != null)
                {
                    if (selectedInteractable != null)
                    {
                        if (selectedInteractable.Equals(interactableObject)) return;

                        OnSelectionEnded();
                    }

                    OnSelectionStarted(interactableObject);
                }
                else
                {
                    OnSelectionEnded();
                }
            }
            else
            {
                OnSelectionEnded();
            }
        }
        else
        {
            OnSelectionEnded();
        }
    }


    private void OnSelectionStarted(IInteractableObject _interactableObject)
    {
        selectedInteractable = _interactableObject;

        if (selectedInteractable == null) return;

        selectedInteractable.OnInteractableSelectionStarted();

        Debug.Log($"OnSelectionStarted");
    }

    private void OnSelectionEnded()
    {
        if (selectedInteractable == null) return;

        selectedInteractable.OnInteractableSelectionEnded();
        selectedInteractable = null;

        Debug.Log($"OnSelectionEnded");
    }
}