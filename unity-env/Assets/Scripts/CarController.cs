using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputManager))]
[RequireComponent(typeof(Rigidbody))]

public class CarController : MonoBehaviour
{
    public InputManager inputManager;
    public List<WheelCollider> throotleWheels;
    public List<GameObject> steeringWheels;
    public float strenghtCoefficient = 6000f;
    public float maxTurn = 22f;
    public bool setCM = false;
    public Vector3 CM = new Vector3(0, 0.5f, 0);
    public Rigidbody body;

    void Start()
    {
        inputManager = GetComponent<InputManager>();
        body = GetComponent<Rigidbody>();
        if (setCM){body.centerOfMass = CM;}
    }

    void FixedUpdate()
    {
        foreach (WheelCollider wheel in throotleWheels)
        {
            wheel.motorTorque = strenghtCoefficient * Time.deltaTime * inputManager.throttle * inputManager.getMode;
        }
        foreach (GameObject wheel in steeringWheels)
        {
            wheel.GetComponent<WheelCollider>().steerAngle = maxTurn * inputManager.steer;
            wheel.transform.localEulerAngles = new Vector3(0f, inputManager.steer * maxTurn, 0f);
        }
    }
}