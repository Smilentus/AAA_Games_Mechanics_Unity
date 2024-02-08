using UnityEngine;


public class FirstPersonControllerPossessable : BasePossessableEntity
{
    [SerializeField]
    private FirstPersonInputHandler m_firstInputHandler;

    [SerializeField]
    private Camera m_fpvCamera;


    protected override void OnPossess()
    {
        Debug.Log($"Possess SimpleControllablePossessable => {this.gameObject}");

        m_firstInputHandler.IsReadingInput = true;
        m_fpvCamera.enabled = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    protected override void OnPhaseOut()
    {
        Debug.Log($"OnPhaseOut SimpleControllablePossessable => {this.gameObject}");

        m_firstInputHandler.IsReadingInput = false;
        m_fpvCamera.enabled = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}