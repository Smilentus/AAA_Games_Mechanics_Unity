using System.Collections.Generic;
using UnityEngine;


public class OnScreenTargetingControllerPresenter : MonoBehaviour
{
    [SerializeField]
    protected Camera m_camera;

    [SerializeField]
    protected Canvas m_targetingCanvas;

    [SerializeField]
    protected bool m_showDebugLog;

    [SerializeField]
    protected DefaultOnScreenTargetUI m_onScreenTargetViewPrefab;


    protected Dictionary<IOnScreenTargetObject, IOnScreenTargetUI> onScreenTargetObjectViews = new Dictionary<IOnScreenTargetObject, IOnScreenTargetUI>();
    protected RectTransform m_targetCanvasRect;


    protected OnScreenTargetingController controller;


    protected virtual void Awake()
    {
        m_targetCanvasRect = m_targetingCanvas?.GetComponent<RectTransform>();

        controller = FindObjectOfType<OnScreenTargetingController>(true);

        controller.onTargetObjectAdded += OnTargetObjectAdded;
        controller.onTargetObjectRemoved += OnTargetObjectRemoved;
    }


    protected virtual void OnDestroy()
    {
        controller.onTargetObjectAdded -= OnTargetObjectAdded;
        controller.onTargetObjectRemoved -= OnTargetObjectRemoved;
    }

    protected virtual void LateUpdate()
    {
        UpdateTargets();
    }


    protected virtual void UpdateTargets()
    {
        controller.SortNullTargets();

        foreach (IOnScreenTargetObject targetObject in controller.TargetObjects)
        {
            if (targetObject == null) continue;

            FollowTarget(targetObject);
        }
    }

    protected Vector2 WorldToCanvasPosition(RectTransform canvas, Camera camera, Vector3 position)
    {
        Vector2 canvasPosition = camera.WorldToViewportPoint(position);

        Vector3 canvasBordered = canvas.sizeDelta;

        canvasPosition.x *= canvasBordered.x;
        canvasPosition.y *= canvasBordered.y;

        canvasPosition.x -= canvasBordered.x * canvas.pivot.x;
        canvasPosition.y -= canvasBordered.y * canvas.pivot.y;

        return canvasPosition;
    }

    protected virtual void OnTargetObjectAdded(IOnScreenTargetObject targetObject)
    {
        onScreenTargetObjectViews[targetObject] = Instantiate(m_onScreenTargetViewPrefab, m_targetingCanvas.transform);
        onScreenTargetObjectViews[targetObject].Setup();
    }

    protected virtual void OnTargetObjectRemoved(IOnScreenTargetObject targetObject)
    {
        if (targetObject == null) return;

        onScreenTargetObjectViews.Remove(targetObject);
    }

    protected virtual void FollowTarget(IOnScreenTargetObject targetObject) {
        if (controller == null || controller.PlayerTransform == null) return;

        Vector3 fromPlayerToTargetDirection = targetObject.TargetTransform.position - controller.PlayerTransform.position;
        Ray fromPlayerToTargetDirectionRay = new Ray(controller.PlayerTransform.position, fromPlayerToTargetDirection);

        if (m_showDebugLog)
        {
            Debug.DrawRay(controller.PlayerTransform.position, fromPlayerToTargetDirection);
        }

        Plane[] cameraFrustrumPlanes = GeometryUtility.CalculateFrustumPlanes(m_camera);

        float fromPlayerToCameraFrustrumMinRaycastDistance = Mathf.Infinity;
        int cameraFrustrumPlaneIndex = 0;

        // [0] Left
        // [1] Right
        // [2] Down
        // [3] Up
        // [4] Near
        // [5] Far
        for (int i = 0; i < 4; i++)
        {
            if (cameraFrustrumPlanes[i].Raycast(fromPlayerToTargetDirectionRay, out float frustrumDistance))
            {
                if (frustrumDistance < fromPlayerToCameraFrustrumMinRaycastDistance)
                {
                    cameraFrustrumPlaneIndex = i;
                    fromPlayerToCameraFrustrumMinRaycastDistance = frustrumDistance;
                }
            }
        }

        fromPlayerToCameraFrustrumMinRaycastDistance = Mathf.Clamp(fromPlayerToCameraFrustrumMinRaycastDistance, 0, fromPlayerToTargetDirection.magnitude);

        Vector3 cameraFrustrumIntersectWorldPosition = fromPlayerToTargetDirectionRay.GetPoint(fromPlayerToCameraFrustrumMinRaycastDistance);

        if (onScreenTargetObjectViews[targetObject] != null)
        {
            Vector3 canvasAnchoredPosition = WorldToCanvasPosition(m_targetCanvasRect, m_camera, cameraFrustrumIntersectWorldPosition);

            onScreenTargetObjectViews[targetObject].SetOnScreenCalculationsData(targetObject, new OnScreenTargetUICalculations()
            {
                CameraFrustrumIntersectWorldPosition = cameraFrustrumIntersectWorldPosition,
                CameraFrustrumPlaneIndex = cameraFrustrumPlaneIndex,
                CanvasAnchoredPosition = canvasAnchoredPosition,
                FromPlayerToTargetDirection = fromPlayerToTargetDirection,
                FromPlayerToCameraFrustrumMinRaycastDistance = fromPlayerToCameraFrustrumMinRaycastDistance,
                IsOutOfScreen = fromPlayerToTargetDirection.magnitude > fromPlayerToCameraFrustrumMinRaycastDistance
            });
        }
    }
}