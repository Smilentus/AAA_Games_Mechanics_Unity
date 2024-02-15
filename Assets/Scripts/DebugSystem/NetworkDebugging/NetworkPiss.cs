using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;


public class NetworkPiss : NetworkBehaviour
{
    [SerializeField]
    private InputActionReference m_pissAction;

    [SerializeField]
    private ParticleSystem m_particleSystem;


    private bool isPissing;


    private void Awake()
    {
        m_pissAction.action.Enable();

        m_pissAction.action.performed += OnPissActionPerformed;
    }

    public override void OnDestroy()
    {
        m_pissAction.action.performed -= OnPissActionPerformed;
        
        base.OnDestroy();
    }


    private void OnPissActionPerformed(InputAction.CallbackContext obj)
    {
        if (!IsLocalPlayer) return;

        SyncPissOnServerRpc(!isPissing);
    }


    [ServerRpc]
    private void SyncPissOnServerRpc(bool _isPissing)
    {
        ReceivePissOnClientRpc(_isPissing);
    }

    [ClientRpc]
    private void ReceivePissOnClientRpc(bool _isPissing)
    {
        isPissing = _isPissing;

        if (isPissing)
        {
            m_particleSystem.Play();
        }
        else
        {
            m_particleSystem.Stop();
        }
    }
}