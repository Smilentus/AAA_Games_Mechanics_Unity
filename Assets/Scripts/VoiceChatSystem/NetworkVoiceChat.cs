using NAudio.Wave;
using Unity.Netcode;
using UnityEngine;


public class NetworkVoiceChat : NetworkBehaviour
{
    [SerializeField]
    private RpcVoiceSenderAndReceiver m_voiceSenderAndReceiver;

    [SerializeField]
    private int m_voiceChatPort = 7878;


    private INetworkChatCodec selectedCodec;
    private volatile bool connected;
    private NetworkAudioPlayer player;
    private NetworkAudioSender audioSender;


    private int selectedMicIndex = 0;


    private void Awake()
    {
        InitializeCodecs();
        GetInputDevices();
    }


    private void Update()
    {
        if (!IsLocalPlayer) return;

        // ...
    }


    public override void OnNetworkSpawn()
    {
        ConnectToVoiceServer();
    }

    public override void OnNetworkDespawn()
    {
        DisconnectFromVoiceChat();
    }


    /*
     * На сервере крутится обработка голоса (пока попробуем на RPC для теста)
     * На клиенте крутится прослушка микрофона и отправка на сервер
     */

    private void ConnectToVoiceServer()
    {
        player = new NetworkAudioPlayer(selectedCodec, m_voiceSenderAndReceiver);
        audioSender = new NetworkAudioSender(selectedCodec, selectedMicIndex, m_voiceSenderAndReceiver);

        connected = true;

        Debug.Log("Успешно подключились к голосовому чату!");
    }

    private void DisconnectFromVoiceChat()
    {
        connected = false;

        audioSender?.Dispose();
        selectedCodec?.Dispose();
        player?.Dispose();
    }


    private void InitializeCodecs()
    {
        selectedCodec = new UncompressedPcmChatCodec();
    }

    private void GetInputDevices()
    {
        for (int n = 0; n < WaveIn.DeviceCount; n++)
        {
            var capabilities = WaveIn.GetCapabilities(n);

            Debug.Log($"Mic: {capabilities.ProductName}");
        }

        selectedMicIndex = 0;
    }
}