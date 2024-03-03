using System;
using System.Collections.Generic;
using UnityEngine;


public class CursorController : MonoBehaviour
{
    private static CursorController _instance;
    public static CursorController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CursorController>();
            }

            return _instance;
        }
    }


    public event Action<string, bool> onCursorControllerStateChanged;


    [SerializeField]
    private bool setDefaultControllerAtStart = false;

    [SerializeField]
    private bool clearControllersAtDestroy = true;


    public bool IsCursorConfined { get; set; } = false;


    public bool IsCursorControlled => _cursorControllers.Count > 0;


    private List<string> _cursorControllers = new List<string>();



    private void Start()
    {
        if (setDefaultControllerAtStart)
        {
            RegisterCursorController("CursorController");
        }
    }

    private void OnDestroy()
    {
        if (clearControllersAtDestroy)
        {
            UnRegisterAllControllers();
        }
    }


    private void CheckAndChangeCursorState()
    {
        if (IsCursorControlled)
        {
            Cursor.visible = true;
            Cursor.lockState = IsCursorConfined ? CursorLockMode.Confined : CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }


    public void UnRegisterAllControllers()
    {
        for (int i = _cursorControllers.Count - 1; i >= 0; i--)
        {
            UnRegisterCursorController(_cursorControllers[i]);
        }

        CheckAndChangeCursorState();
    }


    public void RegisterCursorController(string controller)
    {
        if (_cursorControllers.Contains(controller)) return;

        _cursorControllers.Add(controller);

        CheckAndChangeCursorState();

        onCursorControllerStateChanged?.Invoke(controller, true);
    }

    public void UnRegisterCursorController(string controller)
    {
        if (!_cursorControllers.Contains(controller)) return;

        _cursorControllers.Remove(controller);

        CheckAndChangeCursorState();

        onCursorControllerStateChanged?.Invoke(controller, false);
    }
}