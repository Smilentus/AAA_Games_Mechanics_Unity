using System.Collections;
using UnityEngine;


public class FirstPersonRayInteractorView : MonoBehaviour
{
    [SerializeField]
    private FirstPersonRayInteractor m_rayInteractorReference;

    [SerializeField]
    private GameObject m_interactionDefaultView;


    private Coroutine continiousInteractableCheckerCoroutine;


    private void Awake()
    {
        if (m_rayInteractorReference != null)
        {
            m_rayInteractorReference.onInteractableSelectionStarted += OnInteractableSelectionStarted;
            m_rayInteractorReference.onInteractableSelectionEnded += OnInteractableSelectionEnded;

        }

        continiousInteractableCheckerCoroutine = StartCoroutine(ContiniousInteractableChecker());
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

        if (continiousInteractableCheckerCoroutine != null)
        {
            StopCoroutine(continiousInteractableCheckerCoroutine);
        }
    }


    private void OnInteractableSelectionStarted()
    {
        m_interactionDefaultView.gameObject.SetActive(true);
    }

    private void OnInteractableSelectionEnded()
    {
        m_interactionDefaultView.gameObject.SetActive(false);
    }


    private IEnumerator ContiniousInteractableChecker()
    {
        while (true)
        {
            if (m_rayInteractorReference != null)
            {
                if (m_rayInteractorReference.SelectedInteractable == null)
                {
                    OnInteractableSelectionEnded();
                }
            }

            yield return new WaitForSeconds(0.1f);
        }
    }
}