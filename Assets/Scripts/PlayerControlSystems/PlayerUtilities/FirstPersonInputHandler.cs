using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;


public class FirstPersonInputHandler : NetworkBehaviour
{
    [SerializeField]
    private FirstPersonController m_firstPersonController;


    [SerializeField]
    private float m_mouseSensitivityX = 0.125f;

    [SerializeField]
    private float m_mouseSensitivityY = 0.125f;

    [SerializeField]
    private float m_zoomSensitivity = 0.125f;


    [SerializeField]
    private InputActionReference m_movementAction;

    [SerializeField]
    private InputActionReference m_lookAction;

    [SerializeField]
    private InputActionReference m_crouchAction;

    [SerializeField]
    private InputActionReference m_zoomAction;


    public bool IsReadingInput { get; set; }


    private Vector2 LookVector => new Vector2(
        m_lookAction.action.ReadValue<Vector2>().x * m_mouseSensitivityX, 
        m_lookAction.action.ReadValue<Vector2>().y * m_mouseSensitivityY
    );

    private float ZoomInput => m_zoomAction.action.ReadValue<float>() * m_zoomSensitivity;


    private void Awake()
    {
        m_movementAction.action.Enable();
        m_lookAction.action.Enable();
        m_crouchAction.action.Enable();
        m_zoomAction.action.Enable();
    }


    private void Update()
    {
        if (!IsLocalPlayer) return;

        if (IsReadingInput)
        {
            m_firstPersonController.HandleMoveInput(m_movementAction.action.ReadValue<Vector2>());
            m_firstPersonController.HandleLookInput(LookVector);
            m_firstPersonController.HandleZoomInput(ZoomInput);
            m_firstPersonController.HandleCrouchInput(m_crouchAction.action.WasPerformedThisFrame());
        }
        else
        {
            m_firstPersonController.HandleMoveInput(Vector2.zero);
            m_firstPersonController.HandleLookInput(Vector2.zero);
        }

        m_firstPersonController.UpdateCrouch();
        m_firstPersonController.UpdateZoom();

        m_firstPersonController.AddMovement();
        m_firstPersonController.AddRotation();

        m_firstPersonController.ApplyHeadPosition();
        m_firstPersonController.ApplyMovements();
    }
}
