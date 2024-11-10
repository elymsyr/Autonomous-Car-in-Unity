using UnityEngine;

public class CarCamera : MonoBehaviour
{
    // Reference to the car's transform
    public Transform carTransform;

    // Offset for the left and right cameras (relative to the car)
    private Vector3 leftCameraOffset = new Vector3(0.5516226f ,1f ,0.9850001f);
    private Vector3 rightCameraOffset = new Vector3(-0.5516226f ,1f ,0.9850001f);

    // The fixed rotation (so cameras do not roll or over-rotate)
    private Quaternion fixedRotation;

    void Start()
    {
        // Set the initial fixed rotation based on the car's orientation
        fixedRotation = Quaternion.Euler(carTransform.eulerAngles.x, carTransform.eulerAngles.y, carTransform.eulerAngles.z);
    }

    void Update()
    {
        // Update the left camera's position and keep the rotation fixed to avoid over-rotation
        if (transform.name == "LeftCamera")
        {
            transform.position = carTransform.position + carTransform.TransformDirection(leftCameraOffset);
            transform.rotation = fixedRotation;
        }

        // Update the right camera's position and keep the rotation fixed
        if (transform.name == "RightCamera")
        {
            transform.position = carTransform.position + carTransform.TransformDirection(rightCameraOffset);
            transform.rotation = fixedRotation;
        }
    }
}
