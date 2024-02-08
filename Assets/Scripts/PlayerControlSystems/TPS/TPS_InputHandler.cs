using UnityEngine;
using UnityEngine.InputSystem;


public class TPS_InputHandler : MonoBehaviour
{
    [SerializeField]
    private InputActionReference m_movementAction;
    public InputActionReference MovementAction => m_movementAction;

    [SerializeField]
    private InputActionReference m_lookAction;
    public InputActionReference LookAction => m_lookAction;

    [SerializeField]
    private InputActionReference m_crouchAction;
    public InputActionReference CrouchAction => m_crouchAction;

    [SerializeField]
    private InputActionReference m_proneAction;
    public InputActionReference ProneAction => m_proneAction;

    [SerializeField]
    private InputActionReference m_sprintAction;
    public InputActionReference SprintAction => m_sprintAction;

    [SerializeField]
    private InputActionReference m_walkAction;
    public InputActionReference WalkAction => m_walkAction;

    [SerializeField]
    private InputActionReference m_jumpAction;
    public InputActionReference JumpAction => m_jumpAction;

    [SerializeField]
    private InputActionReference m_strafingAction;
    public InputActionReference StrafingAction => m_strafingAction;


    public Vector2 MovementInput => m_movementAction.action.ReadValue<Vector2>();
    public Vector2 LookInput => m_lookAction.action.ReadValue<Vector2>();


    public bool IsWalking => m_walkAction.action.IsPressed();
    public bool IsStrafing => m_strafingAction.action.IsPressed();
    public bool IsSprinting => m_sprintAction.action.IsPressed();
    public bool IsCrouching => m_crouchAction.action.IsPressed();
    public bool IsProning => m_proneAction.action.IsPressed();
    


    private void Awake()
    {
        m_movementAction.action.Enable();
        m_lookAction.action.Enable();
        m_crouchAction.action.Enable();
        m_sprintAction.action.Enable();
        m_walkAction.action.Enable();
        m_jumpAction.action.Enable();
        m_strafingAction.action.Enable();
        m_proneAction.action.Enable();
    }
}
