using UnityEngine;


public class FirstPersonControllerPossessable : NetworkPossessableEntity
{
    [SerializeField]
    private FirstPersonInputHandler m_firstInputHandler;

    [SerializeField]
    private Camera m_fpvCamera;

    [SerializeField]
    private AudioListener m_audioListener;


    protected override void OnPossess()
    {
        Debug.Log($"Possess SimpleControllablePossessable => {this.gameObject}");

        m_firstInputHandler.IsReadingInput = true;
        m_fpvCamera.enabled = true;
        m_audioListener.enabled = true;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    protected override void OnPhaseOut()
    {
        Debug.Log($"OnPhaseOut SimpleControllablePossessable => {this.gameObject}");

        m_firstInputHandler.IsReadingInput = false;
        m_fpvCamera.enabled = false;
        m_audioListener.enabled = false;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}