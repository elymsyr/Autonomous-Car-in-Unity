using UnityEngine;

public class InputManager : MonoBehaviour
{
    public float throttle;
    public float steer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        throttle = Input.GetAxis("Vertical");
        steer = Input.GetAxis("Horizontal");   
    }
}
