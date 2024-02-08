using UnityEngine;
using UnityEngine.InputSystem;


public class SimpleControllablePossessable : BasePossessableEntity
{
    [SerializeField]
    private Transform m_movableTransform;

    [SerializeField]
    private float m_movementSpeed = 2f;

    [SerializeField]
    private InputActionReference m_movementAxis;


    private Vector2 movementVector;


    private void Awake()
    {
        m_movementAxis.action.Enable();
    }


    private void Update()
    {
        if (!isPossessed) return;

        movementVector = m_movementAxis.action.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        if (!isPossessed) return;

        m_movableTransform.position = Vector3.Lerp(
            this.transform.position,
            this.transform.position + new Vector3(movementVector.x, 0, movementVector.y),
            Time.fixedDeltaTime * m_movementSpeed);
    }


    protected override void OnPossess()
    {
        Debug.Log($"Possess SimpleControllablePossessable => {this.gameObject}");
    }

    protected override void OnPhaseOut()
    {
        Debug.Log($"OnPhaseOut SimpleControllablePossessable => {this.gameObject}");
    }
}