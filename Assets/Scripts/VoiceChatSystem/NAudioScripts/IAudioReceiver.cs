using System;

interface IAudioReceiver : IDisposable
{
    void OnReceived(Action<byte[]> handler);
}