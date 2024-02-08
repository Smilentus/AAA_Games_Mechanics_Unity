using UnityEngine;


public enum CharacterState { Standing, Crouching, Proning };


public class TPS_CharacterController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] 
    private TPS_CameraController cameraController;
    
    [SerializeField] 
    private CharacterController characterController;
    
    [SerializeField]
    private TPS_InputHandler inputHandler;


    [Header("Standing Settings")]
    [SerializeField] private float m_walkSpeedDefault = 3f;
    [SerializeField] private float m_runSpeedDefault = 6f;
    [SerializeField] private float m_sprintSpeedDefault = 10f;
    [SerializeField] private float m_standingRotationSmoothing = 7f;
    [SerializeField] private float m_movementSmoothing = 7f;


    [Header("Crouching Settings")]
    [SerializeField] private float m_crouchingSpeedDefault = 3f;
    [SerializeField] private float m_crouchingSprintDefault = 4f;
    [SerializeField] private float m_crouchingRotationSmoothing = 7f;


    [Header("Proning Settings")]
    [SerializeField] private float m_proningSpeedDefault = 1f;
    [SerializeField] private float m_proningSprintDefault = 1.5f;
    [SerializeField] private float m_proningRotationSmoothing = 7f;


    [Header("Capsules (Radius, Height, YOffset)")]
    [SerializeField] private Vector3 m_standingCapsule = Vector3.zero;
    [SerializeField] private Vector3 m_crouchingCapsule = Vector3.zero;
    [SerializeField] private Vector3 m_proningCapsule = Vector3.zero;


    [Header("Collisions")]
    [SerializeField] private bool m_generateCollisionMaskAutomatic;
    [SerializeField] private LayerMask m_collisionCheckLayerMask;


    [Header("Extra Debug")]
    [SerializeField] private Transform debugCubeGraphics;

    private CharacterState m_characterState;
    private Collider[] m_obstructions = new Collider[8];

    private float m_currentMoveSpeed;
    private float m_currentSprintSpeed;
    private float m_currentRotationSmoothing;

    private bool m_isWalking;
    private bool m_isStrafing;
    private bool m_isSprinting;

    private bool m_isSliding;

    private Vector3 m_moveInputVector;

    private float m_targetSpeed;
    private Quaternion m_targetRotation;

    private Vector3 m_newVelocity;
    private float m_newSpeed;
    private Quaternion m_newRotation;

    private void Start()
    {
        if (m_generateCollisionMaskAutomatic)
        {
            int mask = 0;
            for (int i = 0; i < 32; i++)
            {
                if (!Physics.GetIgnoreLayerCollision(gameObject.layer, i))
                {
                    mask |= 1 << i;
                }
            }
            m_collisionCheckLayerMask = mask;
        }

        m_characterState = CharacterState.Standing;
        m_currentMoveSpeed = m_runSpeedDefault;
        m_currentSprintSpeed = m_sprintSpeedDefault;
        m_currentRotationSmoothing = m_standingRotationSmoothing;

        SetCapsuleDimensions(m_standingCapsule);
    }

    private void Update()
    {
        ChangeMovementState();

        m_moveInputVector = new Vector3(inputHandler.MovementInput.x, 0, inputHandler.MovementInput.y);

        Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraController.planarDirection);
        Vector3 moveInputVectorOriented = m_moveInputVector = cameraPlanarRotation * m_moveInputVector.normalized;

        m_isWalking = inputHandler.IsWalking;
        m_isSprinting = inputHandler.IsSprinting && (m_moveInputVector != Vector3.zero);

        // Вычисляем инпуты для состояния передвижения
        if (m_isStrafing)
        {
            m_isStrafing = inputHandler.IsStrafing && !m_isSprinting;
        }
        else
        {
            m_isStrafing = inputHandler.StrafingAction.action.WasPerformedThisFrame();
        }

        // Получаем скорость в зависимости от инпутов
        if (m_isSprinting)
        {
            m_targetSpeed = m_moveInputVector != Vector3.zero ? m_currentSprintSpeed : 0;
        }
        else if (m_isWalking)
        {
            m_targetSpeed = m_moveInputVector != Vector3.zero ? m_walkSpeedDefault : 0;
        }
        else
        {
            m_targetSpeed = m_moveInputVector != Vector3.zero ? m_currentMoveSpeed : 0;
        }

        // Вычисляем новые значения скорости персонажа
        m_newSpeed = Mathf.Lerp(m_newSpeed, m_targetSpeed, Time.deltaTime * m_movementSmoothing);
        m_newVelocity = moveInputVectorOriented * m_newSpeed;
        characterController.SimpleMove(m_newVelocity);

        // Применяем итоговый поворот персонажа в зависимости от условий
        if (m_isStrafing)
        {
            m_targetRotation = Quaternion.LookRotation(cameraController.planarDirection);
            m_newRotation = Quaternion.Slerp(transform.rotation, m_targetRotation, Time.deltaTime * m_currentRotationSmoothing);
            transform.rotation = m_newRotation;
        }
        else if (m_targetSpeed != 0)
        {
            m_targetRotation = Quaternion.LookRotation(moveInputVectorOriented);
            m_newRotation = Quaternion.Slerp(transform.rotation, m_targetRotation, Time.deltaTime * m_currentRotationSmoothing);
            transform.rotation = m_newRotation;
        }
    }


    private void ChangeMovementState()
    {
        //Debug.Log($"New Speed -> {m_newSpeed}");
        //Debug.Log($"Target Speed -> {m_targetSpeed}");
        //Debug.Log($"New Velocity -> {m_newVelocity}");
        //Debug.Log($"CharacterController.velocity -> {characterController.velocity}");

        switch (m_characterState)
        {
            case CharacterState.Standing:
                if (inputHandler.CrouchAction.action.WasPerformedThisFrame())
                {
                    RequestStateChange(CharacterState.Crouching);
                }
                else if (inputHandler.ProneAction.action.WasPerformedThisFrame())
                {
                    RequestStateChange(CharacterState.Proning);
                }
                break;
            case CharacterState.Crouching:
                if (inputHandler.CrouchAction.action.WasPerformedThisFrame())
                {
                    RequestStateChange(CharacterState.Standing);
                }
                else if (inputHandler.ProneAction.action.WasPerformedThisFrame())
                {
                    RequestStateChange(CharacterState.Proning);
                }
                break;
            case CharacterState.Proning:
                if (inputHandler.CrouchAction.action.WasPerformedThisFrame())
                {
                    RequestStateChange(CharacterState.Crouching);
                }
                else if (inputHandler.ProneAction.action.WasPerformedThisFrame())
                {
                    RequestStateChange(CharacterState.Standing);
                }
                break;
        }
    }

    private void ChangeCurrentStateValues(float _moveSpeed, float _sprintSpeed, float _rotationSmoothing)
    {
        m_currentMoveSpeed = _moveSpeed;
        m_currentSprintSpeed = _sprintSpeed;
        m_currentRotationSmoothing = _rotationSmoothing;
    }

    public bool RequestStateChange(CharacterState newState)
    {
        if (newState == m_characterState) return true;

        switch (m_characterState)
        {
            case CharacterState.Standing:
                if (newState == CharacterState.Crouching)
                {
                    if (!CharacterOverlap(m_crouchingCapsule))
                    {
                        ChangeCurrentStateValues(m_crouchingSpeedDefault, m_crouchingSpeedDefault, m_crouchingRotationSmoothing);
                        m_characterState = CharacterState.Crouching;
                        SetCapsuleDimensions(m_crouchingCapsule);
                        return true;
                    }
                }
                else if (newState == CharacterState.Proning)
                {
                    if (!CharacterOverlap(m_proningCapsule))
                    {
                        ChangeCurrentStateValues(m_proningSpeedDefault, m_proningSpeedDefault, m_proningRotationSmoothing);
                        m_characterState = CharacterState.Proning;
                        SetCapsuleDimensions(m_proningCapsule);
                        return true;
                    }
                }
                break;
            case CharacterState.Crouching:
                if (newState == CharacterState.Standing)
                {
                    if (!CharacterOverlap(m_standingCapsule))
                    {
                        ChangeCurrentStateValues(m_runSpeedDefault, m_sprintSpeedDefault, m_standingRotationSmoothing);
                        m_characterState = CharacterState.Standing;
                        SetCapsuleDimensions(m_standingCapsule);
                        return true;
                    }
                }
                else if (newState == CharacterState.Proning)
                {
                    if (!CharacterOverlap(m_proningCapsule))
                    {
                        ChangeCurrentStateValues(m_proningSpeedDefault, m_proningSpeedDefault, m_proningRotationSmoothing);
                        m_characterState = CharacterState.Proning;
                        SetCapsuleDimensions(m_proningCapsule);
                        return true;
                    }
                }
                break;
            case CharacterState.Proning:
                if (newState == CharacterState.Standing)
                {
                    if (!CharacterOverlap(m_standingCapsule))
                    {
                        ChangeCurrentStateValues(m_runSpeedDefault, m_sprintSpeedDefault, m_standingRotationSmoothing);
                        m_characterState = CharacterState.Standing;
                        SetCapsuleDimensions(m_standingCapsule);
                        return true;
                    }
                }
                else if (newState == CharacterState.Crouching)
                {
                    if (!CharacterOverlap(m_crouchingCapsule))
                    {
                        ChangeCurrentStateValues(m_crouchingSpeedDefault, m_crouchingSpeedDefault, m_crouchingRotationSmoothing);
                        m_characterState = CharacterState.Crouching;
                        SetCapsuleDimensions(m_crouchingCapsule);
                        return true;
                    }
                }
                break;
        }

        return false;
    }

    private bool CharacterOverlap(Vector3 dimensions)
    {
        float radius = dimensions.x;
        float height = dimensions.y;
        Vector3 center = new Vector3(characterController.center.x, dimensions.z, characterController.center.z);

        Vector3 point0;
        Vector3 point1;

        if (height < radius * 2)
        {
            point0 = transform.position + center;
            point1 = transform.position + center;
        }
        else
        {
            point0 = transform.position + center + (transform.up * (height * 0.5f - radius));
            point1 = transform.position + center - (transform.up * (height * 0.5f - radius));
        }

        int numOverlaps = Physics.OverlapCapsuleNonAlloc(point0, point1, radius, m_obstructions, m_collisionCheckLayerMask);
        for (int i = 0; i < numOverlaps; i++)
        {
            if (m_obstructions[i] == characterController)
            {
                numOverlaps--;
            }
        }

        return numOverlaps > 0;
    }

    private void SetCapsuleDimensions(Vector3 dimensions)
    {
        characterController.center = new Vector3(characterController.center.x, dimensions.z, characterController.center.z);
        characterController.radius = dimensions.x;
        characterController.height = dimensions.y;

        if (debugCubeGraphics != null)
        {
            debugCubeGraphics.localPosition = new Vector3(0, dimensions.y / 2f, 0);
            debugCubeGraphics.localScale = new Vector3(debugCubeGraphics.localScale.x, dimensions.y, debugCubeGraphics.localScale.z);
        }
    }
}
