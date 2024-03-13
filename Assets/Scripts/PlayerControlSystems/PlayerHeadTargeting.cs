using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerHeadTargeting : MonoBehaviour
{
    [SerializeField]
    private Transform m_targetingTransform;


    [SerializeField]
    private float m_smoothingReturnTime;


    private Vector3 originalLocalPosition;
    private Quaternion originalLocalRotation;


    public bool IsTargeting { get; set; }


    private void Start()
    {
        originalLocalPosition = m_targetingTransform.localPosition;
        originalLocalRotation = m_targetingTransform.localRotation;
    }


    private void Update()
    {
        if (!IsTargeting)
        {
            ReturnHeadToOriginal();
            return;
        }

        TargetHeadAtPosition();
    }


    private void ReturnHeadToOriginal()
    {
        m_targetingTransform.localPosition = Vector3.Lerp(
            m_targetingTransform.localPosition, 
            originalLocalPosition, 
            m_smoothingReturnTime * Time.deltaTime
        );
    }

    private void TargetHeadAtPosition()
    {
        m_targetingTransform.localPosition = Vector3.Lerp(
            m_targetingTransform.localPosition, 
            m_targetingTransform.InverseTransformVector(Camera.main.transform.forward * 1000f), 
            m_smoothingReturnTime * Time.deltaTime
        );
    }
}
