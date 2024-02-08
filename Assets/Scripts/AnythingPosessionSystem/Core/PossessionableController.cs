using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PossessionableController : MonoBehaviour
{
    protected List<IPossessableEntity> possessedEntities = new List<IPossessableEntity>();


    [SerializeField]
    private int m_availablePossessionsAmount = 1;


    private Coroutine delayedCheckerCoroutine;


    private void Awake()
    {
        delayedCheckerCoroutine = StartCoroutine(DelayedCheckingForErrors());
    }

    private void OnDestroy()
    {
        if (delayedCheckerCoroutine != null)
        {
            StopCoroutine(delayedCheckerCoroutine);
        }
    }


    public void TryPossessToEntity(IPossessableEntity possessableEntity)
    {
        if (possessableEntity == null) return;
        if (possessedEntities.Contains(possessableEntity)) return;
        if (possessedEntities.Count >= m_availablePossessionsAmount) return;
        if (possessableEntity.IsPossessed) return;

        possessableEntity.Possess();
        possessedEntities.Add(possessableEntity);
    }

    public void TryPhaseOutFromEntity(IPossessableEntity possessableEntity)
    {
        if (possessableEntity == null) return;
        if (!possessedEntities.Contains(possessableEntity)) return;
        if (!possessableEntity.IsPossessed) return;

        possessableEntity.PhaseOut();
        possessedEntities.Remove(possessableEntity);
    }


    public void PhaseOutFromEverything()
    {
        for (int i = possessedEntities.Count - 1; i >= 0; i--)
        {
            TryPhaseOutFromEntity(possessedEntities[i]);
        }
    }


    private IEnumerator DelayedCheckingForErrors()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            CheckForErrors();
        }
    }

    private void CheckForErrors()
    {
        for (int i = possessedEntities.Count - 1; i >= 0; i--)
        {
            if (possessedEntities[i].Equals(null))
            {
                possessedEntities.RemoveAt(i);
            }
        }

        if (possessedEntities.Count > m_availablePossessionsAmount)
        {
            for (int i = m_availablePossessionsAmount; i < possessedEntities.Count; i++)
            {
                TryPhaseOutFromEntity(possessedEntities[i]);
            }
        }
    }
}