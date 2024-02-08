using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPS_CameraController : MonoBehaviour
{
    [Header("Mouse Inputs")]
    [SerializeField] private string m_mouseXString = "Mouse X";
    public float mouseXInput => Input.GetAxis(m_mouseXString);

    [SerializeField] private string m_mouseYString = "Mouse Y";
    public float mouseYInput => Input.GetAxis(m_mouseYString);

    [SerializeField] private string m_mouseScrollWheelString = "Mouse ScrollWheel";
    public float mouseScrollWheelInput => Input.GetAxis(m_mouseScrollWheelString);


    [Header("Camera Settings")]
    [SerializeField] private Camera m_playerCamera;
    [SerializeField] private Transform m_targetTransform;
    [SerializeField] private Vector3 m_framing;

    [Header("Zoom Distance")]
    [SerializeField] private float m_zoomSpeed = 10f;
    [SerializeField] private float m_defaultZoom;
    [SerializeField] private float m_minZoom = 0f;
    [SerializeField] private float m_maxZoom = 10f;

    [Header("Invertions")]
    [SerializeField] private bool m_invertX;
    [SerializeField] private bool m_invertY;
    [SerializeField] private bool m_invertZoom;

    [Header("Vertical Limits")]
    [SerializeField] private float m_rotationSmoothing = 25f;
    [SerializeField] private float m_movementSmoothing = 5f;
    [SerializeField] private float m_defaultVerticalAngle = 0f;
    [SerializeField] [Range(-90, 90)] private float m_minVerticalAngle = -90;
    [SerializeField] [Range(-90, 90)] private float m_maxVerticalAngle = 90;


    [Header("Collisions")]
    [SerializeField] private bool m_avoidObstacles;
    [SerializeField] private float m_checkRadius = 0.1f;
    [SerializeField] private LayerMask m_collisionLayerMask = -1;
    [SerializeField] private List<Collider> m_ignoreColliders = new List<Collider>();

    private Vector3 m_planarDirection;
    public Vector3 planarDirection => m_planarDirection;

    private Vector3 m_targetPosition;
    private float m_targetDistance;

    private Quaternion m_targetRotation;
    private float m_targetVerticalAngle;

    private Vector3 m_newPosition;
    private Quaternion m_newRotation;

    private void OnValidate()
    {
        m_defaultZoom = Mathf.Clamp(m_defaultZoom, m_minZoom, m_maxZoom);
        m_defaultVerticalAngle = Mathf.Clamp(m_defaultVerticalAngle, m_minVerticalAngle, m_maxVerticalAngle);
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        m_planarDirection = m_targetTransform.forward;

        m_targetDistance = m_defaultZoom;
        m_targetVerticalAngle = m_defaultVerticalAngle;
    }

    private void LateUpdate()
    {
        HandleCameraRotation();
    }

    private void HandleCameraRotation()
    {
        float mouseScrollWheel = mouseScrollWheelInput * (m_invertZoom ? -1 : 1) * m_zoomSpeed;
        float mouseX = mouseXInput * (m_invertX ? -1 : 1);
        float mouseY = mouseYInput * (m_invertY ? -1 : 1);

        Vector3 focusPosition = m_targetTransform.position + m_playerCamera.transform.TransformDirection(m_framing);

        m_planarDirection = Quaternion.Euler(0, mouseX, 0) * m_planarDirection;
        m_targetDistance = Mathf.Clamp(m_targetDistance + mouseScrollWheel, m_minZoom, m_maxZoom);
        m_targetVerticalAngle = Mathf.Clamp(m_targetVerticalAngle + mouseY, m_minVerticalAngle, m_maxVerticalAngle);

        if (m_avoidObstacles)
        {
            RaycastHit[] hits = Physics.SphereCastAll(focusPosition, m_checkRadius, m_targetRotation * -Vector3.forward, m_targetDistance, m_collisionLayerMask);
            if (hits.Length > 0)
            {
                foreach (RaycastHit hit in hits)
                {
                    if (!m_ignoreColliders.Contains(hit.collider))
                    {
                        if (hit.distance < m_targetDistance)
                        {
                            m_targetDistance = hit.distance;
                        }
                    }
                }
            }
        }

        m_targetRotation = Quaternion.LookRotation(m_planarDirection) * Quaternion.Euler(m_targetVerticalAngle, 0, 0);
        m_targetPosition = focusPosition - (m_targetRotation * Vector3.forward) * m_targetDistance;

        // TODO: Заменить на DoTween
        m_newRotation = Quaternion.Slerp(m_playerCamera.transform.rotation, m_targetRotation, Time.deltaTime * m_rotationSmoothing);
        m_newPosition = Vector3.Lerp(m_playerCamera.transform.position, m_targetPosition, Time.deltaTime * m_movementSmoothing);

        m_playerCamera.transform.rotation = m_newRotation;
        m_playerCamera.transform.position = m_newPosition;
    }

    private void OnDrawGizmos()
    {
        if (m_playerCamera != null)
        {
            Debug.DrawLine(m_playerCamera.transform.position, m_playerCamera.transform.position + m_planarDirection);
        }
    }
}
