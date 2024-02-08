using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public Camera mainCamera;
    [Space]
    public float defaultFOV = 70;
    public float zoomFOV = 30;
    public float targetFOV
    {
        get 
        { 
            return Mathf.Lerp(defaultFOV, zoomFOV, currentZoomLevel); 
        }
    }
    [Space]
    public float FOVLerpParameter = 10;
    public float currentZoomLevel
    {
        get;
        private set;
    }

    void Update()
    {
        mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, targetFOV, Time.deltaTime * FOVLerpParameter);
    }

    public void AddZoom(float increment)
    {
        currentZoomLevel = Mathf.Clamp01(currentZoomLevel + increment);
    }

    public void ZoomIn()
    {
        currentZoomLevel = 1;
    }

    public void ZoomOut()
    {
        currentZoomLevel = 0;
    }
}
