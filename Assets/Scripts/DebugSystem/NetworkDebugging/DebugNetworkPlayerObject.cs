using Unity.Netcode;
using UnityEngine;


public class DebugNetworkPlayerObject : NetworkBehaviour
{
    [SerializeField]
    private GameObject m_localPlayerPrefab;

    [SerializeField]
    private GameObject m_nonLocalPlayerPrefab;


    private void Awake()
    {
        Debug.Log("Player Awake");
    }

    private void Start()
    {
        Debug.Log("Player Start");
    }


    public override void OnNetworkSpawn()
    {
        Debug.Log("Player OnNetworkSpawn");

        if (IsLocalPlayer)
        {
            Instantiate(m_localPlayerPrefab, this.transform);
        }
        else
        {
            Instantiate(m_nonLocalPlayerPrefab, this.transform);
        }
    }
}