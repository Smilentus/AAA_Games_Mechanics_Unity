using UnityEngine;
using DG.Tweening;

[System.Serializable]
public class CharacterMover
{
    [Range(0, 10)]
    public float radius = 0.5f;
    public CharacterController controller
    {
        get;
        private set;
    }

    public bool collidersEnabled => controller.enabled;

    [Header("Movement")]
    public float gravityModifier = 3;
    public float moveSpeed;
    public float crouchSpeedMultiplier = 1;
    public float inAirSpeed;
    public float jumpVelocity;
    public float intertiaResistance = 0.3f;
    [Space]
    [SerializeField] private float slopeLimit = 45;
    [SerializeField] private float stepOffset = 0.3f;

    protected Vector3 verticalVelocity;
    private Vector3 inertialMovementVector = Vector3.zero;
    private Vector3 movementVector;

    private Vector3 manualMovementVector = Vector3.zero;
    private Vector3 inAirMovementVector = Vector3.zero;

    [Header("Head and Rotation")]
    public Transform head;
    
    private Vector3 headStartLocalPosition;
    private Vector3 baseHeadPosition;
    public float maxVerticalRotationAngle = 90;
    public float VerticalAimAngle { get; private set; }
    public float HorizontalAimAngle { get; private set; }
    private Transform transform;

    [Header("Crouch")]
    public float headCrouchHeight = 0.4f;
    public float crouchTweenTime = 0.5f;
    public float crouchStandUpHeadPadding = 0.2f;
    public LayerMask crouchStandUpCheckMask;

    public bool IsCrouched
    {
        get;
        private set;
    }
    private Tweener crouchTweener;

    [Header("Misc")]
    public float smoothedPositionTime = 0.5f;
    private Vector3 smoothedPositionVelocityVector;
    public Vector3 smoothedPosition
    {
        get;
        private set;
    }

    public void Setup(GameObject gameObject)
    {
        transform = gameObject.transform;
        smoothedPosition = transform.position;
        headStartLocalPosition = baseHeadPosition = head.localPosition;

        controller = gameObject.AddComponent<CharacterController>();
        controller.center = baseHeadPosition / 2;
        controller.radius = radius;
        controller.height = baseHeadPosition.y;
        controller.skinWidth = 0.0001f;
        controller.slopeLimit = slopeLimit;
        controller.stepOffset = stepOffset;
    }

    public void ManualMovementCustomSpeed(Vector2 moveInput, Vector3 forward, Vector3 right, float speed)
    {
        if (moveInput.magnitude > 1)
            moveInput.Normalize();

        Vector3 moveVector3 = forward * moveInput.y + right * moveInput.x;

        Vector3 groundNormal = GetGroundNormal();
        // Slope correction
        if (groundNormal != Vector3.up)
        {
            Vector3 correctedVector = moveVector3.magnitude * (moveVector3 - Vector3.Dot(moveVector3, groundNormal) * groundNormal / groundNormal.sqrMagnitude).normalized;
            if (correctedVector.y < 0)
            {
                moveVector3 = correctedVector;
            }
        }

        if (IsNearGround())
        {
            manualMovementVector = Vector3.Lerp(manualMovementVector, moveVector3, Time.deltaTime * 10);
            inAirMovementVector = Vector3.zero;
        }
        else
        {
            inAirMovementVector = Vector3.Lerp(inAirMovementVector, moveVector3, Time.deltaTime * 10);
            NonInertialMovement(inAirSpeed * inAirMovementVector);
        }

        InertialMovement(manualMovementVector, speed);
    }

    public void ManualMovement(Vector2 moveInput, Vector3 forward, Vector3 right)
    {
        ManualMovementCustomSpeed(moveInput, forward, right, IsCrouched? (crouchSpeedMultiplier * moveSpeed) : moveSpeed);
    }

    public bool IsNearGround()
    {
        return Physics.Raycast(new Ray(transform.position, Vector3.down), 0.3f) || controller.isGrounded;
    }

    protected void InertialMovement(Vector3 moveDirection, float moveSpeed, bool checkGround = true, float multiplier = 1)
    {
        if (controller.isGrounded || IsNearGround() || !checkGround)
        {
            moveDirection *= moveSpeed * multiplier;

            float velocityMultiplier = GetControllerVelocityCoefficient(moveSpeed);
            float velocityMultiplierStrength = 0.7f;

            inertialMovementVector = moveDirection * (1 + velocityMultiplierStrength * (velocityMultiplier - 1));
        }
        movementVector += inertialMovementVector * Time.deltaTime;
        inertialMovementVector -= inertialMovementVector * Time.deltaTime * intertiaResistance;
    }

    public float GetControllerVelocityCoefficient(float moveSpeed)
    {
        Vector3 horizontalVelocity = controller.velocity;
        horizontalVelocity.y = 0;
        return Mathf.Clamp(horizontalVelocity.magnitude / moveSpeed, 0, 1);
    }

    public void RedirectInertia(Vector3 newDirection)
    {
        if (newDirection.magnitude == 0)
            return;

        manualMovementVector = Vector3.zero;
        inertialMovementVector = newDirection.normalized * inertialMovementVector.magnitude;
    }

    public void ResetInertia()
    {
        inertialMovementVector = Vector3.zero;
        verticalVelocity = Vector3.zero;
        movementVector = Vector3.zero;
        manualMovementVector = Vector3.zero;
    }

    public void SetVerticalVelocity(float val)
    {
        verticalVelocity = Vector3.up * val;
    }

    public void NonInertialMovement(Vector3 moveSpeedVector)
    {
        movementVector += moveSpeedVector * Time.deltaTime;
    }

    public void Gravity()
    {
        if (controller.isGrounded && verticalVelocity.y < 0)
            verticalVelocity = Vector3.zero;

        verticalVelocity += gravityModifier * Physics.gravity * Time.deltaTime;
        Vector3 deltaPosition = verticalVelocity * Time.deltaTime;
        Vector3 move = Vector3.up * deltaPosition.y;

        movementVector += move;
    }

    public void Jump()
    {
        if (!controller.isGrounded)
            return;
        verticalVelocity.y = jumpVelocity;
    }

    public void ApplyHeadPosition()
    {
        controller.height = baseHeadPosition.y;
        controller.center = baseHeadPosition / 2;
        head.localPosition = baseHeadPosition + transform.InverseTransformVector(smoothedPosition - transform.position);
    }

    public void Crouch()
    {
        IsCrouched = true;
        if (crouchTweener != null)
        {
            crouchTweener.Kill();
        }
        crouchTweener = DOTween.To(GetHeadPosition, SetHeadPosition, Vector3.up * headCrouchHeight, crouchTweenTime);
    }

    public void StandUp()
    {
        float rayDistance = Vector3.Distance(headStartLocalPosition, GetHeadPosition()) + crouchStandUpHeadPadding;
        if (Physics.Raycast(new Ray(head.parent.position + GetHeadPosition(), Vector3.up), rayDistance, crouchStandUpCheckMask))
        {
            return;
        }

        IsCrouched = false;
        if (crouchTweener != null)
        {
            crouchTweener.Kill();
        }
        crouchTweener = DOTween.To(GetHeadPosition, SetHeadPosition, headStartLocalPosition, crouchTweenTime);
    }

    public void StandUpImmediate()
    {
        IsCrouched = false;
        if (crouchTweener != null)
        {
            crouchTweener.Kill();
        }
        SetHeadPosition(headStartLocalPosition);
        ApplyHeadPosition();
    }

    public void CrouchImmediate()
    {
        IsCrouched = true;
        if (crouchTweener != null)
        {
            crouchTweener.Kill();
        }
        SetHeadPosition(Vector3.up * headCrouchHeight);
        ApplyHeadPosition();
    }

    private void SetHeadPosition(Vector3 value)
    {
        baseHeadPosition = value;
    }

    private Vector3 GetHeadPosition()
    {
        return baseHeadPosition;
    }

    public void ApplyMovement(float multiplier = 1)
    {
        if (controller.enabled)
        {
            controller.Move(movementVector * multiplier);
        }
        movementVector = Vector3.zero;
        ApplyHeadPosition();
        UpdateSmoothedPosition();
    }

    public Vector3 GetGroundNormal()
    {
        if (Physics.Raycast(new Ray(transform.position, Vector3.down), out RaycastHit hit, 1f))
            if (hit.normal != Vector3.up)
                return hit.normal;

        return Vector3.up;
    }

    public void Move(Vector3 motion)
    {
        if (!controller)
            return;
        controller.Move(motion);
        UpdateSmoothedPosition();
    }

    public void DestroyMovementComponents()
    {
        Object.Destroy(controller);
    }

    public void DisableCollider()
    {
        controller.enabled = false;
    }

    public void EnableCollider()
    {
        controller.enabled = true;
    }

    public void Rotation(Vector2 rotationInput)
    {
        VerticalAimAngle = Mathf.Clamp(VerticalAimAngle - rotationInput.y, -Mathf.Abs(maxVerticalRotationAngle), Mathf.Abs(maxVerticalRotationAngle));

        head.localRotation = Quaternion.Euler(VerticalAimAngle, 0, 0);

        HorizontalAimAngle += rotationInput.x;
        transform.Rotate(new Vector3(0, rotationInput.x, 0));
    }

    public void SetVerticalRotation(float val)
    {
        VerticalAimAngle = val;
        head.localRotation = Quaternion.Euler(VerticalAimAngle, 0, 0);
    }

    public void SetHorizontalRotation(float val)
    {
        HorizontalAimAngle = val;
        transform.rotation = Quaternion.Euler(0, HorizontalAimAngle, 0);
    }

    public void SetRotation(float horizontal, float vertical)
    {
        SetVerticalRotation(vertical);
        SetHorizontalRotation(horizontal);
    }

    public void LookAt(Vector3 target, float lerpParameter)
    {
        target.y = transform.position.y;
        Vector3 direction = target - transform.position;
        transform.rotation = Quaternion.LookRotation(Vector3.Lerp(transform.forward, direction, lerpParameter));
    }

    public void UpdateSmoothedPosition()
    {
        smoothedPosition = Vector3.SmoothDamp(smoothedPosition, transform.position, ref smoothedPositionVelocityVector, smoothedPositionTime);
    }

    public void ResetSmoothedPosition()
    {
        smoothedPosition = transform.position;
        smoothedPositionVelocityVector = Vector3.zero;
    }
}