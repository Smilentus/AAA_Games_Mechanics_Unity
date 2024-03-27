using UnityEngine;
using UnityEngine.Rendering;


public class IKFootHandler : MonoBehaviour
{
    [SerializeField]
    private IKFootHandler m_oppositeHandler;


    [SerializeField]
    private LayerMask m_collisionMask;

    [SerializeField]
    private float m_offsetY = 5;

    [SerializeField]
    private Transform m_movableTarget;


    [SerializeField]
    private float m_stepDistance = 0.5f;

    [SerializeField]
    private float m_stepHeight = 0.5f;

    [SerializeField]
    private float m_stepSpeed = 5f;


    private Vector3 _currentPosition;
    private Vector3 _newPosition;

    private float _lerp = 0f;


    public bool IsGrounded => _currentPosition == _newPosition;


    private void Start()
    {
        _currentPosition = this.transform.position;
        _newPosition = _currentPosition;
    }


    private void Update()
    {
        transform.position = _currentPosition;

        if (m_oppositeHandler != null && !m_oppositeHandler.IsGrounded)
        {
            return;
        }


        Ray ray = new Ray(m_movableTarget.position + new Vector3(0, m_offsetY, 0), Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 100, m_collisionMask.value))
        {
            if (Vector3.Distance(_newPosition, hit.point) >= m_stepDistance)
            {
                _lerp = 0;
                _newPosition = hit.point;
            }
        }

        if (_lerp < 1)
        {
            Vector3 footPosition = Vector3.Lerp(_currentPosition, _newPosition, _lerp);
            footPosition.y += Mathf.Sin(_lerp * Mathf.PI) * m_stepHeight;

            _currentPosition = footPosition;
            _lerp += Time.deltaTime * m_stepSpeed;
        }
        else
        {
            _currentPosition = _newPosition;
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(_currentPosition, 0.5f);

        if (m_movableTarget != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(m_movableTarget.position, 0.25f);
            Gizmos.DrawLine(_currentPosition, m_movableTarget.position);
        }
    }
}
