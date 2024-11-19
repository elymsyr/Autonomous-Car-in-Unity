using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject targetObject;
    public Camera topCamera;
    public Camera leftCamera;
    public Camera rightCamera;
    public Camera insideCamera;
    public Vector3 topCameraLocalOffset = new Vector3(0f, 5f, -10f);
    public Vector3 leftOffset = new Vector3(-0.7f, 0.5f, 1.5f);
    public Vector3 rightOffset = new Vector3(0.7f, 0.5f, 1.5f);
    public Vector3 insideOffset = new Vector3(-0.39f,0.51f,0.08f);
    public Vector3 insideRotationOffset = new Vector3(10.12f, 4.368f, 0.1f);
    
    void Update()
    {
        // --- Top-down Camera (following behind the car) ---
        // Calculate position behind the car based on the car's forward direction
        Vector3 topCameraPosition = targetObject.transform.position + targetObject.transform.TransformDirection(topCameraLocalOffset);
        topCamera.transform.position = topCameraPosition;

        // Make the top-down camera look at the car from behind and above
        topCamera.transform.LookAt(targetObject.transform.position);

        // ---  Cameras ---
        // Position and rotate the left  camera
        leftCamera.transform.position = targetObject.transform.position + targetObject.transform.TransformDirection(leftOffset);
        leftCamera.transform.rotation = targetObject.transform.rotation;

        // Position and rotate the right  camera
        rightCamera.transform.position = targetObject.transform.position + targetObject.transform.TransformDirection(rightOffset);
        rightCamera.transform.rotation = targetObject.transform.rotation;

        // Position and rotate the inside  camera
        insideCamera.transform.position = targetObject.transform.position + targetObject.transform.TransformDirection(insideOffset);
        insideCamera.transform.rotation = Quaternion.Euler(targetObject.transform.rotation.eulerAngles + insideRotationOffset);
    }
}
