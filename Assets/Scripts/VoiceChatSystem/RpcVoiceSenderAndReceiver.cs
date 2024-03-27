using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;


public class RpcVoiceSenderAndReceiver : NetworkBehaviour, IAudioSender, IAudioReceiver
{
    public Button debugButton;


    public bool IsAllowedToSyncVoice { get; set; }


    private Action<byte[]> _handler;


    private void Awake()
    {
        debugButton.onClick.AddListener(() => {
            IsAllowedToSyncVoice = !IsAllowedToSyncVoice;

            if (IsAllowedToSyncVoice)
            {
                debugButton.GetComponentInChildren<TMP_Text>().text = "«¿œ»—‹";
            }
            else
            {
                debugButton.GetComponentInChildren<TMP_Text>().text = "Ã”“";
            }    
        });

        if (IsAllowedToSyncVoice)
        {
            debugButton.GetComponentInChildren<TMP_Text>().text = "«¿œ»—‹";
        }
        else
        {
            debugButton.GetComponentInChildren<TMP_Text>().text = "Ã”“";
        }
    }


    public override void OnNetworkSpawn()
    {
        if (!IsLocalPlayer)
        {
            Destroy(this.transform.GetChild(0).gameObject);
        }
    }


    public void Dispose()
    {

    }

    public void Send(byte[] payload)
    {
        if (!IsLocalPlayer) return;

        if (!IsAllowedToSyncVoice) return;

        SendAtServerRpc(new NetworkVoiceChunkData() { 
            ownerId = this.OwnerClientId,
            voiceChunk = payload
        });
    }


    [ServerRpc(RequireOwnership = false)]
    public void SendAtServerRpc(NetworkVoiceChunkData chunkData)
    {
        ReceivePayloadAtClientRpc(chunkData);
    }

    [ClientRpc]
    public void ReceivePayloadAtClientRpc(NetworkVoiceChunkData chunkData)
    {
        if (IsLocalPlayer) return;

        _handler?.Invoke(chunkData.voiceChunk);
    }

    public void OnReceived(Action<byte[]> handler)
    {
        _handler = handler;
    }
}

public struct NetworkVoiceChunkData : INetworkSerializable
{
    public byte[] voiceChunk;
    public ulong ownerId;

    
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref voiceChunk);
        serializer.SerializeValue(ref ownerId);
    }
}
