using NAudio.Wave;
using NAudioDemo.NetworkChatDemo;
using System.Net;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class VoiceChatController : MonoBehaviour
{
    public TMP_InputField debugPortInput;
    public Button connectButton;

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

    private void Start()
    {
        connectButton.onClick.AddListener(() =>
        {
            if (connected)
            {
                DisconnectFromVoiceChat();
            }
            else
            {
                ConnectToVoiceChat();
            }
        });
    }

    private void OnDestroy()
    {
        DisconnectFromVoiceChat();
    }


    private void ConnectToVoiceChat()
    {
        // “ут надо из ёнити потока RPC получать данные каким-то чудом
        //var receiver = (comboBoxProtocol.SelectedIndex == 0)
        //        ? (IAudioReceiver)new UdpAudioReceiver(endPoint.Port)
        //        : new TcpAudioReceiver(endPoint.Port);

        //var sender = (comboBoxProtocol.SelectedIndex == 0)
        //    ? (IAudioSender)new UdpAudioSender(endPoint)
        //    : new TcpAudioSender(endPoint);

        // —юда вбивать данные из места, которое будет вбивать IP и т.п. при подключении к серверу
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(debugPortInput.text), m_voiceChatPort);

        var receiver = (IAudioReceiver)new TcpAudioReceiver(endPoint.Port);
        var sender = (IAudioSender)new TcpAudioSender(endPoint);

        //var receiver = (IAudioReceiver)new TcpAudioReceiver(endPoint.Port);
        //var sender = (IAudioSender)new TcpAudioSender(endPoint);

        player = new NetworkAudioPlayer(selectedCodec, receiver);
        audioSender = new NetworkAudioSender(selectedCodec, selectedMicIndex, sender);
        connected = true;

        Debug.Log("”спешно подключились к голосовому чату!");
    }

    private void DisconnectFromVoiceChat()
    {
        if (connected)
        {
            connected = false;

            player.Dispose();
            audioSender.Dispose();
            selectedCodec.Dispose();
        }
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