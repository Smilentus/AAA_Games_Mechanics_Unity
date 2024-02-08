using UnityEngine;


public class DebugPossession : MonoBehaviour
{
    [SerializeField]
    private PossessionableController possessionableController;


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray, 1000f);

            if (hits.Length > 0)
            {
                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider.TryGetComponent<IPossessableEntity>(out IPossessableEntity entity))
                    {
                        if (entity.IsPossessed)
                        {
                            possessionableController.TryPhaseOutFromEntity(entity);
                        }
                        else
                        {
                            possessionableController.TryPossessToEntity(entity);
                        }

                        break;
                    }
                }
                
            }

        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            possessionableController.PhaseOutFromEverything();
        }
        
    }
}
