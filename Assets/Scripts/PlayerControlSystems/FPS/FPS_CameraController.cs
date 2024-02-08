using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS_CameraController : MonoBehaviour
{
    public string horizontalLookAxis = "Mouse X";
    public string verticalLookAxis = "Mouse Y";

    public CameraZoom cameraZoom;

    public Transform head;

    public float maxVerticalRotationAngle;

    private float verticalAimAngle;
    private float horizontalAimAngle;

    private void Update()
    {
        Vector2 rotationInput = new Vector2(Input.GetAxisRaw(horizontalLookAxis), Input.GetAxisRaw(verticalLookAxis)) * cameraZoom.targetFOV / cameraZoom.defaultFOV;

        verticalAimAngle = Mathf.Clamp(verticalAimAngle - rotationInput.y, -Mathf.Abs(maxVerticalRotationAngle), Mathf.Abs(maxVerticalRotationAngle));

        head.localRotation = Quaternion.Euler(verticalAimAngle, 0, 0);

        horizontalAimAngle += rotationInput.x;
        transform.Rotate(new Vector3(0, rotationInput.x, 0));
    }
}
