using System;

interface IAudioSender : IDisposable
{
    void Send(byte[] payload);
}