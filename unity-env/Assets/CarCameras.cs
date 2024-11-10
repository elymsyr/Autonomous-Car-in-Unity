using UnityEngine;

public class CarCameras : MonoBehaviour
{
    public Transform carTransform;

    private Vector3 leftCameraOffset = new Vector3(-0.5516226f, 1f, 0.9850001f);
    private Vector3 rightCameraOffset = new Vector3(0.5516226f, 1f, 0.9850001f);
    private Vector3 topCameraOffset = new Vector3(0f, 3.01100016f, -4.41399956f);
    private Vector3 insideCameraOffset = new Vector3(-0.354999989f,1.04700005f,0.200000003f);

    private Quaternion topCameraRotationOffset = new Quaternion(0.168227732f, 0f, 0f, 0.985748172f);

    public Transform leftCamera;
    public Transform rightCamera;
    public Transform topCamera;
    public Transform insideCamera;

    void Update()
    {
        insideCamera.position = carTransform.TransformPoint(insideCameraOffset);
        leftCamera.position = carTransform.TransformPoint(leftCameraOffset);
        rightCamera.position = carTransform.TransformPoint(rightCameraOffset);
        topCamera.position = carTransform.TransformPoint(topCameraOffset);

        insideCamera.rotation = carTransform.rotation;
        leftCamera.rotation = carTransform.rotation;
        rightCamera.rotation = carTransform.rotation;

        Quaternion topCameraRotation = carTransform.rotation * topCameraRotationOffset;
        topCamera.rotation = topCameraRotation;
    }
}
