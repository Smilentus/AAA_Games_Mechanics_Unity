using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class FirstPersonInteractorCursorView : MonoBehaviour
{
    [SerializeField]
    private FirstPersonRayInteractor m_rayInteractorReference;

    [SerializeField]
    private Image m_cursorImage;


    [SerializeField]
    private Color m_defaultColor;

    [SerializeField]
    private Color m_highlightedColor;


    private Sequence cursorSmoothings;


    private void Awake()
    {
        if (m_rayInteractorReference != null)
        {
            m_rayInteractorReference.onInteractableSelectionStarted += OnInteractableSelectionStarted;
            m_rayInteractorReference.onInteractableSelectionEnded += OnInteractableSelectionEnded;
        }
    }

    private void Start()
    {
        OnInteractableSelectionEnded();
    }
    

    private void OnDestroy()
    {
        if (m_rayInteractorReference != null)
        {
            m_rayInteractorReference.onInteractableSelectionStarted -= OnInteractableSelectionStarted;
            m_rayInteractorReference.onInteractableSelectionEnded -= OnInteractableSelectionEnded;
        }
    }


    private void OnInteractableSelectionStarted()
    {
        if (cursorSmoothings != null)
        {
            cursorSmoothings.Kill();
        }

        cursorSmoothings = DOTween.Sequence();
        cursorSmoothings.Append(m_cursorImage.DOColor(m_highlightedColor, .15f));
        cursorSmoothings.Append(m_cursorImage.transform.DOScale(1.35f, .15f));
        cursorSmoothings.Play();
    }

    private void OnInteractableSelectionEnded()
    {
        if (cursorSmoothings != null)
        {
            cursorSmoothings.Kill();
        }

        cursorSmoothings = DOTween.Sequence();
        cursorSmoothings.Append(m_cursorImage.DOColor(m_defaultColor, .15f));
        cursorSmoothings.Append(m_cursorImage.transform.DOScale(1f, .15f));
        cursorSmoothings.Play();
    }
}