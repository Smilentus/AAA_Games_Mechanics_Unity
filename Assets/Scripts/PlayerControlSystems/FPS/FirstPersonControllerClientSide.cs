using Unity.Netcode;
using UnityEngine;


public class FirstPersonControllerClientSide : NetworkBehaviour
{
    [SerializeField]
    private FirstPersonInputHandler m_firstInputHandler;

    [SerializeField]
    private Camera m_fpvCamera;

    [SerializeField]
    private AudioListener m_audioListener;


    public override void OnNetworkSpawn()
    {
        if (IsLocalPlayer)
        {
            EnableController();
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsLocalPlayer)
        {
            DisableController();
        }
    }


    public void EnableController()
    {
        m_firstInputHandler.IsReadingInput = true;
        m_fpvCamera.enabled = true;
        m_audioListener.enabled = true;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void DisableController()
    {
        m_firstInputHandler.IsReadingInput = false;
        m_fpvCamera.enabled = false;
        m_audioListener.enabled = false;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}