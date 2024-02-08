using System.Collections;
using UnityEngine;


public class TestPlayableVoiceObject : BaseNetworkPlayableVoiceObject
{
    [SerializeField]
    private BaseVoiceClipData BaseVoiceClipDataRef;


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsHost)
        {
            StartCoroutine(DelayedSyncVoiceClip());
        }
    }


    public override void OnDestroy()
    {
        base.OnDestroy();

        StopAllCoroutines();
    }


    private IEnumerator DelayedSyncVoiceClip()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(3, 10));

            SyncVoiceClip(BaseVoiceClipDataRef.VoiceClipGUID);
        }
    }
}