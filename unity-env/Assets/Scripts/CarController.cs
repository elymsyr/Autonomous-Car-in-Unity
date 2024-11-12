using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputManager))]
public class CarController : MonoBehaviour
{
    public InputManager inputManager;
    public List<WheelCollider> throotleWheels;
    public List<WheelCollider> steeringWheels;
    public float strenghtCoefficient = 2000f;
    public float nextTurn = 20f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inputManager = GetComponent<InputManager>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach(WheelCollider wheel in throotleWheels){
            wheel.motorTorque = strenghtCoefficient * Time.deltaTime * inputManager.throttle;
        }
        foreach(WheelCollider wheel in steeringWheels){
            wheel.steerAngle = nextTurn * inputManager.steer;
        }
    }
}
