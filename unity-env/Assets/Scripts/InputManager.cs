using UnityEngine;

public class InputManager : MonoBehaviour
{
    public float throttle = 0f;
    public float steer = 0f;
    public float throttleChangeSpeed = 1f;
    public float steerChangeSpeed = 2f;
    private int mode = 2;
    public int getMode => mode;


    void Start()
    {

    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) mode = -1;
        if (Input.GetKeyDown(KeyCode.N)) mode = 0;
        if (Input.GetKeyDown(KeyCode.F)) mode = 1;

        float throttleInput = Input.GetAxis("Vertical");
        float steerInput = Input.GetAxis("Horizontal");


        throttle += throttleInput * throttleChangeSpeed * Time.deltaTime;
        throttle = Mathf.Clamp(throttle, 0f, 1f);


        steer += steerInput * steerChangeSpeed * Time.deltaTime;
        steer = Mathf.Clamp(steer, -1f, 1f);
    }
}
