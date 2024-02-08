using System;
using Unity.Netcode;
using UnityEngine;


/// <summary>
///     Основной объект, который умеет воспроизводить какие-то диалоги и т.п.
///     Перенаправляет на общий контроллер игрока, чтобы контролировать в одном месте
/// </summary>
public class BaseNetworkPlayableVoiceObject : NetworkBehaviour
{
    public event Action<string> onVoiceClipSyncRequested;


    public event Action<BaseVoiceClipData> onVoiceClipStarted;
    public event Action onVoiceClipEnded;


    [SerializeField]
    private AudioSource m_audioSourceReference;
    public AudioSource AudioSourceReference => m_audioSourceReference;


    public virtual void SyncVoiceClip(string voiceClipGUID)
    {
        SendVoiceClipSyncOnServerRpc(voiceClipGUID);
    }


    public virtual void PlayVoiceClipData(BaseVoiceClipData voiceClipData)
    {
        if (voiceClipData == null || AudioSourceReference == null) return;

        if (AudioSourceReference.isPlaying)
        {
            AudioSourceReference.Stop();
        }

        AudioSourceReference.clip = voiceClipData.VoiceClip;
        AudioSourceReference.Play();

        onVoiceClipStarted?.Invoke(voiceClipData);

        SubtitlesController.Instance.OnVoiceClipPlayed(voiceClipData);
    }


    [ServerRpc]
    public void SendVoiceClipSyncOnServerRpc(string voiceClipGUID)
    {
        ReceiveVoiceClipOnClientRpc(voiceClipGUID);
    }

    [ClientRpc]
    public void ReceiveVoiceClipOnClientRpc(string voiceClipGUID)
    {
        PlayVoiceClipData(VoiceWarehouseController.Instance.GetVoiceClipData(voiceClipGUID));
    }
}
