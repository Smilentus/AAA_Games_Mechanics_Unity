using System;
using UnityEngine;

// Добавить прыжок
public class FirstPersonController : MonoBehaviour
{
    public bool enableInputByDefault = true;
    public CharacterMover mover;

    public CameraZoom cameraZoom;
    //public BaseScreenFade screenFade;

    public float zoomSpeed = 2;

    public bool crouchEnabled
    {
        get;
        set;
    } = true;

    private bool init;

    private Vector2 moveInput;
    private bool crouchInput;
    private Vector2 lookInput;
    private float zoomInput;

    public event Action<Vector2> OnMovementInput;
    public event Action<Vector2> OnRotationInput;

    void Start()
    {
        Init();
    }

    private void Init()
    {
        if (init)
        {
            return;
        }

        mover.Setup(gameObject);
        init = true;
    }

    private void OnDestroy()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    public void AddRotation()
    {
        Vector2 rotationInput = GetRotationInput();
        mover.Rotation(rotationInput);
        OnRotationInput?.Invoke(rotationInput);
    }

    public void AddMovement()
    {
        mover.ManualMovement(moveInput, transform.forward, transform.right);
        OnMovementInput?.Invoke(moveInput);
    }

    public void UpdateCrouch()
    {
        if (crouchInput && crouchEnabled)
        {
            SwitchCrouch();
        }
    }

    public void ApplyMovements()
    {
        if (!mover.collidersEnabled)
        {
            mover.EnableCollider();
        }
        
        mover.Gravity();
        mover.ApplyMovement();
        ApplyHeadPosition();
    }

    public void ApplyHeadPosition()
    {
        mover.ApplyHeadPosition();
    }

    public void UpdateZoom()
    {
        cameraZoom.AddZoom(zoomSpeed * zoomInput);
    }

    private void SwitchCrouch()
    {
        Init();

        if (mover.IsCrouched)
        {
            mover.StandUp();
        }
        else
        {
            mover.Crouch();
        }
    }

    public void HandleMoveInput(Vector2 input)
    {
        moveInput = input;
    }

    public void HandleCrouchInput(bool input)
    {
        crouchInput = input;
    }

    public void HandleLookInput(Vector2 input)
    {
        lookInput = input;
    }

    public void HandleZoomInput(float input)
    {
        zoomInput = input;
    }

    private Vector2 GetRotationInput()
    {
        return lookInput * cameraZoom.targetFOV / cameraZoom.defaultFOV;
    }

    public void Fade(Action OnFadeStart = null, Action OnFullFade = null, Action OnFadeEnd = null)
    {
        //screenFade.StartFade(OnFadeStart, OnFullFade, OnFadeEnd);
    }

    public void Teleport(Func<Vector3> positionGet, Func<Quaternion> rotationGet, float headVerticalAngle, Action OnFadeStart, Action OnBeforeTeleport, Action OnAfterTeleport, Action OnFadeEnd, Transform parent)
    {
        void FadeStart()
        {
            mover.DisableCollider();
            mover.ResetInertia();
            mover.SetVerticalVelocity(0);
            cameraZoom.ZoomOut();
            OnFadeStart?.Invoke();
        }

        void FadeFull()
        {
            OnBeforeTeleport?.Invoke();
            mover.StandUpImmediate();
            transform.SetParent(parent);
            transform.SetPositionAndRotation(positionGet(), rotationGet());
            mover.ResetSmoothedPosition();
            mover.SetVerticalRotation(headVerticalAngle);
            OnAfterTeleport?.Invoke();
        }

        void FadeEnd()
        {
            mover.ResetSmoothedPosition();
            OnFadeEnd?.Invoke();
        }

        Fade(FadeStart, FadeFull, FadeEnd);
    }

    public void TeleportImmediate(Vector3 position, Quaternion rotation, float headVerticalAngle = 0, Action OnTeleported = null)
    {
        Init();

        mover.ResetInertia();
        mover.SetVerticalVelocity(0);
        cameraZoom.ZoomOut();
        transform.position = position;
        transform.rotation = rotation;
        mover.SetVerticalRotation(headVerticalAngle);
        mover.ResetSmoothedPosition();
        OnTeleported?.Invoke();
    }
}
