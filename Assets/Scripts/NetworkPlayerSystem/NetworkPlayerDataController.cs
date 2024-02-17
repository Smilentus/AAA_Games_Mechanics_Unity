using Unity.Netcode;
using UnityEngine;


public class NetworkPlayerDataController : NetworkBehaviour
{
    // debug only
    public MeshRenderer meshRenderer;

    private PlayerColorData playerColorData;


    public override void OnNetworkSpawn()
    {
        RequestPlayerColorAtServerRpc();
    }


    [ServerRpc(RequireOwnership = false)]
    public void RequestPlayerColorAtServerRpc()
    {
        if (NetworkManager.ConnectedClients.Count == 0) return;

        int playerIndex = -1;

        int i = 0;
        // ������� ��� ������, ����� �� Approval ����� ������
        foreach (var client in NetworkManager.ConnectedClients.Values)
        {
            if (client.ClientId == NetworkObject.OwnerClientId)
            {
                playerIndex = i;
                break;
            }
            i++;
        }

        ApplyColorAtClientRpc(playerIndex);
    }

    // ������������ �� ���� ����� ���� (�������, ��� ��� ������ ���)
    [ClientRpc]
    public void ApplyColorAtClientRpc(int colorIndex)
    {
        playerColorData = NetworkPlayerColorizer.Instance.GetPlayerColorData(colorIndex);

        if (playerColorData != null)
        {
            meshRenderer.material.color = playerColorData.color;
        }
    }
}