using UnityEngine;

public class FirstPersonViewBobbing : MonoBehaviour
{
    [SerializeField]
    private FirstPersonController firstPersonController;

    [SerializeField]
    private float walkingSpeed = 10;

    [SerializeField]
    private float walkingHeadBobMagnitude = 0.2f;

    [SerializeField]
    private float walkingArmBobbingOffsetMagnitude = -0.02f;

    [SerializeField]
    private float walkingArmOffsetMagnitude = 0.02f;

    [SerializeField]
    private float walkingEffectMagnitude = 5;

    [SerializeField]
    private float walkingLerpParameter = 5;

    [SerializeField]
    private Transform cameraHolder;

    [Header("Rotation Effect")]
    [SerializeField]
    public Transform armsHolder;
    [SerializeField]
    public float horizontalSpeed = 1;
    [SerializeField]
    public float verticalalSpeed = 1;
    [SerializeField]
    public float maxHorizontalOffset = 0.2f;
    [SerializeField]
    public float maxVerticalOffset = 0.2f;

    [Header("Audio")]
    [SerializeField]
    private AudioClip footstepSound;
    [SerializeField]
    private Transform footstepSoundSource;

    private Vector3 armsHolderPivot;
    private Vector3 swayOffset;
    private float walkingParameter;

    private Vector2 moveInput;
    private Vector2 rotationInput;

    private void Start()
    {
        firstPersonController.OnMovementInput += OnMovementInput;
        firstPersonController.OnRotationInput += OnRotationInput;
    }

    private void OnDestroy()
    {
        if (firstPersonController)
        {
            firstPersonController.OnMovementInput -= OnMovementInput;
            firstPersonController.OnRotationInput -= OnRotationInput;
        }
    }
    private void OnMovementInput(Vector2 input)
    {
        moveInput = input;
    }

    private void OnRotationInput(Vector2 input)
    {
        rotationInput = input;
    }

    void Update()
    {
        BobbingEffect();
    }

    private void BobbingEffect()
    {
        float walkingMagnitude = moveInput.magnitude;

        float previousWalkingParameter = walkingParameter;

        if (firstPersonController.mover.IsNearGround())
            walkingParameter += Time.deltaTime * walkingSpeed * walkingMagnitude * (firstPersonController.mover.IsCrouched ? firstPersonController.mover.crouchSpeedMultiplier : 1);
        walkingParameter %= 2 * Mathf.PI;

        bool footstep = false;
        if (walkingParameter < previousWalkingParameter)
            footstep = true;
        else if (previousWalkingParameter <= Mathf.PI && walkingParameter > Mathf.PI)
            footstep = true;

        if (footstep)
            PlayFootstepSound();

        float walkingCosine = Mathf.Sign(Mathf.Cos(walkingParameter)) * Mathf.Pow(Mathf.Cos(walkingParameter), 2f);
        float walkingSine = Mathf.Sign(Mathf.Sin(walkingParameter)) * Mathf.Pow(Mathf.Sin(walkingParameter), 2f);

        Vector3 armsAngle = new Vector3(Mathf.Sin(walkingParameter * 2), walkingSine, 0);
        Vector3 armsBobbingOffset = walkingMagnitude * walkingArmBobbingOffsetMagnitude * new Vector3(walkingCosine, Mathf.Abs(walkingCosine), 0);

        Quaternion targetArmRotation = Quaternion.Euler(walkingEffectMagnitude * walkingMagnitude * armsAngle);

        rotationInput.x *= horizontalSpeed;
        rotationInput.y *= verticalalSpeed;

        rotationInput.x = Mathf.Clamp(rotationInput.x, -Mathf.Abs(maxHorizontalOffset), Mathf.Abs(maxHorizontalOffset));
        rotationInput.y = Mathf.Clamp(rotationInput.y, -Mathf.Abs(maxVerticalOffset), Mathf.Abs(maxVerticalOffset));

        swayOffset = Vector3.Lerp(swayOffset, new Vector3(rotationInput.x, rotationInput.y, 0), Time.deltaTime * 10);
        Vector3 targetArmsPosition = armsHolderPivot - swayOffset - walkingArmOffsetMagnitude * new Vector3(moveInput.x, 0, moveInput.y) + armsBobbingOffset;

        armsHolder.localRotation = Quaternion.Slerp(armsHolder.localRotation, targetArmRotation, Time.deltaTime * walkingLerpParameter);
        armsHolder.localPosition = Vector3.Lerp(armsHolder.localPosition, targetArmsPosition, Time.deltaTime * 10);
        cameraHolder.localRotation = Quaternion.Slerp(cameraHolder.localRotation, Quaternion.Euler(walkingHeadBobMagnitude * walkingMagnitude * new Vector3(Mathf.Sin(walkingParameter * 2), 0, Mathf.Sin(walkingParameter))), Time.deltaTime * walkingLerpParameter);

        moveInput = Vector2.zero;
        rotationInput = Vector2.zero;
    }

    private void PlayFootstepSound()
    {
        if (footstepSound && footstepSoundSource)
        {
            AudioSource.PlayClipAtPoint(footstepSound, footstepSoundSource.position);
        }
    }
}
