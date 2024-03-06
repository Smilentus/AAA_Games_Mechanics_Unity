using UnityEngine;


public interface IOnScreenTargetUI
{
    public void Setup();

    public void SetOnScreenCalculationsData(IOnScreenTargetObject targetObject, OnScreenTargetUICalculations calculations);
}


[System.Serializable]
public struct OnScreenTargetUICalculations
{
    public Vector3 FromPlayerToTargetDirection;

    public Vector3 CameraFrustrumIntersectWorldPosition;
    public Vector2 CanvasAnchoredPosition;

    public float FromPlayerToCameraFrustrumMinRaycastDistance;
    public int CameraFrustrumPlaneIndex;

    public bool IsOutOfScreen;
}