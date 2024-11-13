using UnityEngine;

public class InputManager : MonoBehaviour
{
    public float throttle = 0f;
    public float steer = 0f;
    public float brake = 0f;
    public float handbrake = 0f;
    public float throttleChangeSpeed = 1f;
    public float steerChangeSpeed = 2f;
    public float brakeChangeSpeed = 1.5f;
    public float clutchChangeSpeed = 5f;

    private int mode = 1;
    public int getMode => mode;

    private void Start()
    {
    }

    private void Update()
    {
        // Switch modes
        if (Input.GetKeyDown(KeyCode.R)) mode = -1;
        if (Input.GetKeyDown(KeyCode.N)) mode = 0;
        if (Input.GetKeyDown(KeyCode.F)) mode = 1;

        // Get input values
        float throttleInput = Input.GetAxis("Vertical");
        float steerInput = Input.GetAxis("Horizontal");

        // Call functions to handle each input
        HandleThrottle(throttleInput);
        HandleSteering(steerInput);
        HandleBrake();
        HandleClutch();
    }

    // Throttle handling function
    private void HandleThrottle(float throttleInput)
    {
        throttle += throttleInput * throttleChangeSpeed * Time.deltaTime;
        throttle = Mathf.Clamp(throttle, 0f, 1f);
    }

    // Steering handling function
    private void HandleSteering(float steerInput)
    {
        steer += steerInput * steerChangeSpeed * Time.deltaTime;
        steer = Mathf.Clamp(steer, -1f, 1f);
    }

    private void HandleBrake()
    {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            brake += brakeChangeSpeed * Time.deltaTime; // 0.8 *
        }
        else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            brake -= brakeChangeSpeed * Time.deltaTime; // 0.8 *
        }
        brake = Mathf.Clamp(brake, 0f, 1f);
    }

    // Clutch handling function
    private void HandleClutch()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            handbrake += clutchChangeSpeed * Time.deltaTime;
        }
        else
        {
            handbrake = Mathf.MoveTowards(handbrake, 0f, clutchChangeSpeed * Time.deltaTime);
        }
        handbrake = Mathf.Clamp(handbrake, 0f, 1f);
    }
}
