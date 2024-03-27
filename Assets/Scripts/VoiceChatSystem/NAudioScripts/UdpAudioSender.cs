using System.Net;
using System.Net.Sockets;

class UdpAudioSender : IAudioSender
{
    private readonly UdpClient udpSender;
    public UdpAudioSender(IPEndPoint endPoint)
    {
        udpSender = new UdpClient();
        udpSender.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        udpSender.Connect(endPoint);
    }

    public void Send(byte[] payload)
    {
        udpSender.Send(payload, payload.Length);
    }

    public void Dispose()
    {
        udpSender?.Close();
    }
}
