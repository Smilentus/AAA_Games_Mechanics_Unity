using UnityEngine;
using UnityEngine.UI;

public class DefaultOnScreenTargetUI : MonoBehaviour, IOnScreenTargetUI
{
    [SerializeField]
    protected RectTransform m_movableRect;


    [SerializeField]
    protected Image m_screenSpaceOutOfScreenImage;

    [SerializeField]
    protected Image m_screenSpaceOnScreenImage;


    [SerializeField]
    protected Sprite m_onScreenSprite;

    [SerializeField]
    protected Sprite m_outOfScreenSprite;


    [SerializeField]
    protected float m_minimalVisibleDistance = 1f;


    public Vector2 TargetImageSize => m_movableRect.rect.size;


    public virtual void Setup()
    {
        if (m_screenSpaceOnScreenImage != null)
        {
            m_screenSpaceOnScreenImage.enabled = m_onScreenSprite != null;
            m_screenSpaceOnScreenImage.sprite = m_onScreenSprite;
        }

        if (m_screenSpaceOutOfScreenImage != null)
        {
            m_screenSpaceOutOfScreenImage.enabled = m_outOfScreenSprite != null;
            m_screenSpaceOutOfScreenImage.sprite = m_outOfScreenSprite;
        }
    }


    public virtual void SetOnScreenCalculationsData(IOnScreenTargetObject targetObject, OnScreenTargetUICalculations calculations)
    {
        if (calculations.FromPlayerToTargetDirection.magnitude <= m_minimalVisibleDistance)
        {
            ToggleVisibility(false);
        }
        else
        {
            ToggleVisibility(true);

            m_movableRect.anchoredPosition = calculations.CanvasAnchoredPosition;

            SetRotation(calculations.CameraFrustrumPlaneIndex);
            SwitchIcons(calculations.IsOutOfScreen);
        }
    }


    protected virtual void ToggleVisibility(bool value)
    {
        m_screenSpaceOutOfScreenImage.enabled = value;
        m_screenSpaceOnScreenImage.enabled = value;
    }

    protected virtual void SwitchIcons(bool isOutOfScreen)
    {
        if (isOutOfScreen)
        {
            m_screenSpaceOutOfScreenImage.enabled = true;
            m_screenSpaceOnScreenImage.enabled = false;
        }
        else
        {
            m_screenSpaceOutOfScreenImage.enabled = false;
            m_screenSpaceOnScreenImage.enabled = true;
        }
    }

    protected virtual void SetRotation(int frustrumPlane)
    {   
        // [0] Left
        // [1] Right
        // [2] Down
        // [3] Up
        switch (frustrumPlane)
        {
            case 2: // Down
                m_screenSpaceOutOfScreenImage.transform.localEulerAngles = Vector3.forward * 270;
                break;
            case 0: // Left
                m_screenSpaceOutOfScreenImage.transform.localEulerAngles = Vector3.forward * 180;
                break;
            case 3: // Up
                m_screenSpaceOutOfScreenImage.transform.localEulerAngles = Vector3.forward * 90;
                break;
            case 1: // Right
                m_screenSpaceOutOfScreenImage.transform.localEulerAngles = Vector3.forward * 0;
                break;
            case -1:
                m_screenSpaceOutOfScreenImage.transform.localEulerAngles = Vector3.zero;
                break;
        }
    }
}