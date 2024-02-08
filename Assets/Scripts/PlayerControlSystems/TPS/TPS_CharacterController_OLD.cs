using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPS_CharacterController_OLD : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TPS_CameraController cameraController;
    [SerializeField] private CharacterController characterController;


    [Header("Inputs")]
    [SerializeField] private string m_moveAxisX = "Horizontal";
    public float moveAxisXInputRaw => Input.GetAxisRaw(m_moveAxisX);

    [SerializeField] private string m_moveAxisY = "Vertical";
    public float moveAxisYInputRaw => Input.GetAxisRaw(m_moveAxisY);
    
    [SerializeField] private KeyCode m_sprintButtonKeyCode;


    [Header("Movement")]
    [SerializeField] private float m_walkSpeed = 3f;
    [SerializeField] private float m_runSpeed = 6f;
    [SerializeField] private float m_springSpeed = 10f;


    [Header("Smoothness")]
    [SerializeField] private float m_standingRotationSmoothing = 7f;
    [SerializeField] private float m_movementSmoothing = 7f;


    private bool m_isStrafing;
    private bool m_isSprinting;

    private Vector3 m_moveInputVector;

    private float m_targetSpeed;
    private Quaternion m_targetRotation;

    private Vector3 m_newVelocity;
    private float m_newSpeed;
    private Quaternion m_newRotation;

    private void Update()
    {
        m_moveInputVector = new Vector3(moveAxisXInputRaw, 0, moveAxisYInputRaw);
        Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraController.planarDirection);
        Vector3 moveInputVectorOriented = m_moveInputVector = cameraPlanarRotation * m_moveInputVector.normalized;
        
        // Вычисляем инпуты для стрейфа или спринта
        if (m_isStrafing)
        {
            m_isSprinting = Input.GetKeyDown(m_sprintButtonKeyCode) && (m_moveInputVector != Vector3.zero);
            m_isStrafing = Input.GetMouseButton(1) && !m_isSprinting;
        }
        else
        {
            m_isSprinting = Input.GetKey(m_sprintButtonKeyCode) && (m_moveInputVector != Vector3.zero);
            m_isStrafing = Input.GetMouseButtonDown(1);
        }

        // Получаем скорость в зависимости от инпутов
        if (m_isSprinting)
        {
            m_targetSpeed = m_moveInputVector != Vector3.zero ? m_springSpeed : 0;
        }
        else if (m_isStrafing)
        {
            m_targetSpeed = m_moveInputVector != Vector3.zero ? m_walkSpeed : 0;
        }
        else
        {
            m_targetSpeed = m_moveInputVector != Vector3.zero ? m_runSpeed : 0;
        }

        // Вычисляем новые значения скорости персонажа
        m_newSpeed = Mathf.Lerp(m_newSpeed, m_targetSpeed, Time.deltaTime * m_movementSmoothing);
        m_newVelocity = moveInputVectorOriented * m_newSpeed;
        characterController.SimpleMove(m_newVelocity);

        // Применяем итоговый поворот персонажа в зависимости от условий
        if (m_isStrafing)
        {
            m_targetRotation = Quaternion.LookRotation(cameraController.planarDirection);
            m_newRotation = Quaternion.Slerp(transform.rotation, m_targetRotation, Time.deltaTime * m_standingRotationSmoothing);
            transform.rotation = m_newRotation;
        }
        else if (m_targetSpeed != 0)
        {
            m_targetRotation = Quaternion.LookRotation(moveInputVectorOriented);
            m_newRotation = Quaternion.Slerp(transform.rotation, m_targetRotation, Time.deltaTime * m_standingRotationSmoothing);
            transform.rotation = m_newRotation;
        }
    }
}
