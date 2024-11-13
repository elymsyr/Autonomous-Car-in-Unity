using UnityEngine;

public class WheelRotation : MonoBehaviour
{
    private WheelCollider wheelCollider;
    private MeshRenderer wheel; // MeshRenderer that will be moved and rotated
    private Quaternion quat;
    private Vector3 pos;

    void Start()
    {
        // Get the WheelCollider component attached to the same GameObject
        wheelCollider = this.GetComponent<WheelCollider>();
        Debug.Log("Current Object: " + wheelCollider.name);

        // Get the MeshRenderer component from the first child of wheelCollider
        if (wheelCollider.transform.childCount > 0)
        {
            // Get the first child with MeshRenderer
            wheel = wheelCollider.transform.GetComponentInChildren<MeshRenderer>();
            if (wheel != null)
            {
                Debug.Log("First Child MeshRenderer: " + wheel.name);
            }
            else
            {
                Debug.Log("No MeshRenderer found in the child objects.");
            }
        }
        else
        {
            Debug.Log("No child objects found.");
        }
    }

    void Update()
    {
        if (wheel != null) // Ensure wheel is not null before trying to access it
        {
            // Get the world position and rotation of the WheelCollider
            wheelCollider.GetWorldPose(out pos, out quat);

            // Update the wheel's position and rotation based on the WheelCollider
            wheel.transform.position = pos;
            wheel.transform.rotation = quat;
        }
    }
}
