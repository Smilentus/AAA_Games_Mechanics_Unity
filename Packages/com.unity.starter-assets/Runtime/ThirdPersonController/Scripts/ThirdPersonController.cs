using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

// TODO: redecorate and recook code [1/2]

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerInput))]
    public class ThirdPersonController : MonoBehaviour
    {
        private const float _inputThreshold = 0.01f;
        
        private readonly int _animIDSpeed = Animator.StringToHash("Speed");
        private readonly int _animIDGrounded = Animator.StringToHash("Grounded");
        private readonly int _animIDJump = Animator.StringToHash("Jump");
        private readonly int _animIDFreeFall = Animator.StringToHash("FreeFall");
        private readonly int _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        private readonly int _animIDStrafing = Animator.StringToHash("IsStrafing");
        private readonly int _animIDHorizontal = Animator.StringToHash("HorizontalMovement");
        private readonly int _animIDVertical = Animator.StringToHash("VerticalMovement");


        [Header("Movement Settings")]
        [SerializeField]
        private float m_strafingSpeed = 1.0f;
        public float StrafingSpeed => m_strafingSpeed;

        [SerializeField]
        private float m_walkingSpeed = 2.0f;
        public float WalkingSpeed => m_walkingSpeed;

        [SerializeField]
        private float m_sprintSpeed = 5.335f;
        public float SprintSpeed => m_sprintSpeed;

        [SerializeField]
        private float m_jumpHeight = 1.2f;
        public float JumpHeight => m_jumpHeight;

        [SerializeField]
        private float m_gravity = -9.8f;
        public float Gravity => m_gravity;

        [SerializeField]
        private float m_jumpTimeout = 0.5f;
        public float JumpTimeout => m_jumpTimeout;

        [SerializeField]
        private float m_fallTimeOut = 0.15f;
        public float FallTimeout => m_fallTimeOut;

        [Tooltip("Скорость поворота в указанном направлении перемещения")]
        [Range(0.0f, 0.3f)]
        private float m_rotationSmoothTime = 0.12f;
        public float RotationSmoothTime => m_rotationSmoothTime;

        private float m_speedChangeRate = 10.0f;
        public float SpeedChangeRate => m_speedChangeRate;


        [Header("Footsteps Sounds")]
        [SerializeField]
        private AudioClip m_landingAudioClip;
        public AudioClip LandingAudioClip => m_landingAudioClip;

        [SerializeField]
        private AudioClip[] m_footstepsAudioClips;
        public AudioClip[] FootstepAudioClips => m_footstepsAudioClips;

        [Range(0, 1)]
        [SerializeField]
        private float m_footstepAudioVolume = 0.5f;
        public float FootstepAudioVolume => m_footstepAudioVolume;

        [SerializeField]
        private float m_groundedOffset = -0.14f;
        public float GroundedOffset => m_groundedOffset;

        [SerializeField]
        private float m_groundedRadius = 0.28f;
        public float GroundedRadius => m_groundedRadius;

        [SerializeField]
        private LayerMask m_groundLayers;
        public LayerMask GroundLayers => m_groundLayers;


        [Header("Cinemachine Settings")]
        [SerializeField]
        private CinemachineVirtualCamera _cineCamera;

        [SerializeField]
        private GameObject m_cinemachineCameraTarget;

        [SerializeField]
        private float m_topClamp = 70.0f;

        [SerializeField]
        private float m_bottomClamp = -30.0f;

        [SerializeField]
        private float m_cameraAngleOverride = 0.0f;
        /// <summary>
        ///     Additional degress to override the camera. Useful for fine tuning camera position when locked
        /// </summary>
        public float CameraAngleOverride { get => m_cameraAngleOverride; set => m_cameraAngleOverride = value; }

        [SerializeField]
        private float m_cameraVerticalSens = 3f;
        public float CameraVerticalSens { get => m_cameraVerticalSens; set => m_cameraVerticalSens = value; }

        [SerializeField]
        private float m_cameraHorizontalSens = 3f;
        public float CameraHorizontalSens { get => m_cameraHorizontalSens; set => m_cameraHorizontalSens = value; }

        [SerializeField]
        private float m_strafingCameraSpeed = 3f;
        public float StrafingCameraSpeed { get => m_strafingCameraSpeed; set => m_strafingCameraSpeed = value; }

        [SerializeField]
        private bool m_lockCameraPosition = false;
        public bool LockCameraPosition { get => m_lockCameraPosition; set => m_lockCameraPosition = value; }


        [Header("References")]
        [SerializeField]
        private PlayerInput m_playerInput;

        [SerializeField]
        private Animator m_animator;

        [SerializeField]
        private CharacterController m_controller;

        [SerializeField]
        private StarterAssetsInputs m_input;

        [SerializeField]
        private GameObject m_mainCamera;


        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private bool _isGrounded = true;
        private float _moveSpeed;
        private float _animationBlend;
        private float _horizontalBlend = 0.0f;
        private float _verticalBlend = 0.0f;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        private bool _hasAnimator;

        private Cinemachine3rdPersonFollow _cineCameraTransposer;


        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return m_playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }


        private void Awake()
        {
            // Реально так надо? А может не надо?
            CinemachineComponentBase componentBase = _cineCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
            if (componentBase is Cinemachine3rdPersonFollow)
            {
                _cineCameraTransposer = componentBase as Cinemachine3rdPersonFollow;
            }

            _hasAnimator = m_animator != null;
        }

        private void Start()
        {
            _cinemachineTargetYaw = m_cinemachineCameraTarget.transform.rotation.eulerAngles.y;

            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
        }

        private void FixedUpdate()
        {
            JumpAndGravity();
            GroundedCheck();
            Move();
            CameraRotation();
        }

        private void GroundedCheck()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            _isGrounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

            if (_hasAnimator)
            {
                m_animator.SetBool(_animIDGrounded, _isGrounded);
            }
        }

        private void CameraRotation()
        {
            if (m_input.look.sqrMagnitude >= _inputThreshold && !LockCameraPosition)
            {
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.fixedDeltaTime;

                _cinemachineTargetYaw += m_input.look.x * CameraHorizontalSens * deltaTimeMultiplier;
                _cinemachineTargetPitch += m_input.look.y * CameraVerticalSens * deltaTimeMultiplier;
            }

            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, m_bottomClamp, m_topClamp);

            m_cinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + m_cameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }

        private void Move()
        {
            bool isStrafing = Input.GetMouseButton(1);

            float targetSpeed = m_input.sprint ? SprintSpeed : WalkingSpeed;

            if (isStrafing)
            {
                targetSpeed = StrafingSpeed;
            }

            if (m_input.move == Vector2.zero) targetSpeed = 0.0f;

            float currentHorizontalSpeed = new Vector3(m_controller.velocity.x, 0.0f, m_controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = m_input.analogMovement ? m_input.move.magnitude : 1f;

            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                _moveSpeed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.fixedDeltaTime * SpeedChangeRate);

                _moveSpeed = Mathf.Round(_moveSpeed * 1000f) / 1000f;
            }
            else
            {
                _moveSpeed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.fixedDeltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            Vector3 inputDirection = new Vector3(m_input.move.x, 0.0f, m_input.move.y).normalized;

            float rotation = 0.0f;

            if (m_input.move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + m_mainCamera.transform.eulerAngles.y;
            }

            if (_cineCameraTransposer != null)
            {
                _cineCameraTransposer.CameraDistance = isStrafing
                    ? Mathf.Lerp(_cineCameraTransposer.CameraDistance, 1.0f, StrafingCameraSpeed * Time.fixedDeltaTime)
                    : Mathf.Lerp(_cineCameraTransposer.CameraDistance, 3.0f, StrafingCameraSpeed * Time.fixedDeltaTime);
            }

            if (isStrafing)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + m_mainCamera.transform.eulerAngles.y;

                rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _cinemachineTargetYaw, ref _rotationVelocity, RotationSmoothTime);
            }
            else
            {
                rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);
            }

            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);

            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            m_controller.Move(targetDirection.normalized * (_moveSpeed * Time.fixedDeltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.fixedDeltaTime);


            _verticalBlend = Mathf.Clamp(Mathf.Lerp(_verticalBlend, m_input.move.y, 5f * Time.fixedDeltaTime), -1, 1);
            _horizontalBlend = Mathf.Clamp(Mathf.Lerp(_horizontalBlend, m_input.move.x, 5f * Time.fixedDeltaTime), -1, 1);


            //if (isStrafing && Input.GetKeyDown(KeyCode.Alpha1))
            //{
            //    if (_hasAnimator)
            //    {
            //        _animator.SetTrigger("Punch");
            //    }
            //}

            //if (isStrafing && Input.GetKeyDown(KeyCode.Alpha2))
            //{
            //    if (_hasAnimator)
            //    {
            //        _animator.SetTrigger("Kick");
            //    }
            //}

            //if (isStrafing && Input.GetKeyDown(KeyCode.Alpha3))
            //{
            //    if (_hasAnimator)
            //    {
            //        _animator.SetTrigger("HeadButt");
            //    }
            //}

            //if (isStrafing && Input.GetKeyDown(KeyCode.Alpha4))
            //{
            //    if (_hasAnimator)
            //    {
            //        _animator.SetTrigger("ElbowLowKick");
            //    }
            //}


            if (_hasAnimator)
            {
                m_animator.SetFloat(_animIDSpeed, _animationBlend);
                m_animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
                m_animator.SetBool(_animIDStrafing, isStrafing);

                m_animator.SetFloat(_animIDVertical, _verticalBlend);
                m_animator.SetFloat(_animIDHorizontal, _horizontalBlend);
            }
        }

        private void JumpAndGravity()
        {
            if (_isGrounded)
            {
                _fallTimeoutDelta = FallTimeout;

                if (_hasAnimator)
                {
                    m_animator.SetBool(_animIDJump, false);
                    m_animator.SetBool(_animIDFreeFall, false);
                }

                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                if (m_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                    if (_hasAnimator)
                    {
                        m_animator.SetBool(_animIDJump, true);
                    }
                }

                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.fixedDeltaTime;
                }
            }
            else
            {
                _jumpTimeoutDelta = JumpTimeout;

                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.fixedDeltaTime;
                }
                else
                {
                    if (_hasAnimator)
                    {
                        m_animator.SetBool(_animIDFreeFall, true);
                    }
                }

                m_input.jump = false;
            }

            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.fixedDeltaTime;
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (_isGrounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(m_controller.center), FootstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(m_controller.center), FootstepAudioVolume);
            }
        }
    }
}