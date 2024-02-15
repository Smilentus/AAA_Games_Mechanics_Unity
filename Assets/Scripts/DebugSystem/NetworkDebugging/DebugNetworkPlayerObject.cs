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

        //GameObject instantiatedPlayerController;

        //if (IsLocalPlayer)
        //{
        //    instantiatedPlayerController = Instantiate(m_localPlayerPrefab, this.transform);
        //}
        //else
        //{
        //    instantiatedPlayerController = Instantiate(m_nonLocalPlayerPrefab, this.transform);
        //}

        //instantiatedPlayerController.transform.SetParent(this.transform);

        //if (IsServer)
        //{
        //    if (instantiatedPlayerController.TryGetComponent<NetworkObject>(out NetworkObject playerNetworkObject))
        //    {
        //        playerNetworkObject.SpawnAsPlayerObject(OwnerClientId);
        //    }
        //}
    }
}