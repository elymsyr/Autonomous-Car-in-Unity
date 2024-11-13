using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(InputManager))]
[RequireComponent(typeof(Rigidbody))]

public class CarController : MonoBehaviour
{
    public InputManager inputManager;
    public WheelColliders colliders;
    public WheelMeshes meshes;
    public float maxTurn = 45;
    public float speedMeter = 22.5f;
    public bool setCM = false;
    public Vector3 CM = new Vector3(0, 0.5f, 0);
    public Rigidbody body;
    public float RPM = 0f;

    public float motorPower = 2000f;
    public float brakePower = 50000f;
    public float speed;
    public float KMH;
    private float wheelRPM;
    public float redLine;
    public float idleRPM;
    public float differentialRatio;
    public float currentTorque;
    public AnimationCurve hpToRPMCurve;

    void Start()
    {
        inputManager = GetComponent<InputManager>();
        body = GetComponent<Rigidbody>();
        if (setCM){body.centerOfMass = CM;}
        if (colliders != null)
        {
            colliders.Initialize();
        }
        else
        {
            Debug.LogError("WheelColliders reference is not assigned in the Inspector!");
        }

        if (meshes != null)
        {
            meshes.Initialize();
        }
        else
        {
            Debug.LogError("WheelMeshes reference is not assigned in the Inspector!");
        }
    }

    void FixedUpdate()
    {
        foreach (WheelCollider wheel in colliders.steeringwheels)
        {
            wheel.steerAngle = maxTurn * inputManager.steer;
            wheel.transform.localEulerAngles = new Vector3(0f, inputManager.steer * maxTurn, 0f);
        }

        speed = colliders.RRWheel.rpm * colliders.RRWheel.radius * 2f * Mathf.PI / speedMeter;
        KMH = Mathf.Lerp(KMH, speed, Time.deltaTime);
        ApplyMotor();
        ApplyBrake();
    }

    void ApplyBrake()
    {
        if (inputManager.handbrake > 0){
            colliders.FRWheel.brakeTorque = inputManager.handbrake * brakePower;
            colliders.FLWheel.brakeTorque = inputManager.handbrake * brakePower;
            colliders.RRWheel.brakeTorque = inputManager.handbrake * brakePower;
            colliders.RLWheel.brakeTorque = inputManager.handbrake * brakePower;
        }
        else{
            colliders.FRWheel.brakeTorque = inputManager.brake * brakePower* 0.8f;
            colliders.FLWheel.brakeTorque = inputManager.brake * brakePower * 0.8f;
            colliders.RRWheel.brakeTorque = inputManager.brake * brakePower * 0.5f;
            colliders.RLWheel.brakeTorque = inputManager.brake * brakePower *0.5f;
        }
    }

    void ApplyMotor() {

        currentTorque = CalculateTorque();
        colliders.RRWheel.motorTorque = currentTorque * inputManager.throttle * inputManager.getMode;
        colliders.RLWheel.motorTorque = currentTorque * inputManager.throttle * inputManager.getMode;
    }

    float CalculateTorque()
    {
        RPM = Mathf.Lerp(RPM, Mathf.Max(idleRPM, redLine * inputManager.throttle) + UnityEngine.Random.Range(-50, 50), Time.deltaTime);
        wheelRPM = Mathf.Abs((colliders.RRWheel.rpm + colliders.RLWheel.rpm) / 2f) * differentialRatio;
        RPM = Mathf.Lerp(RPM, Mathf.Max(idleRPM - 100, wheelRPM), Time.deltaTime * 3f);
        return (hpToRPMCurve.Evaluate(RPM / redLine) * motorPower / RPM) * differentialRatio * 5250f;
    }

    public float GetSpeedRatio()
    {
        var gas = Mathf.Clamp(Mathf.Abs(inputManager.throttle), 0.5f, 1f);
        return RPM * gas / redLine;
    }
}

[System.Serializable]
public class WheelColliders
{
    public WheelCollider FRWheel;  // Front Right Wheel Collider
    public WheelCollider FLWheel;  // Front Left Wheel Collider
    public WheelCollider RRWheel;  // Rear Right Wheel Collider
    public WheelCollider RLWheel;  // Rear Left Wheel Collider

    // A list that holds all the wheel colliders, initialized once
    private List<WheelCollider> _list;
    private List<WheelCollider> _steeringwheels;

    public List<WheelCollider> list => _list;
    public List<WheelCollider> steeringwheels => _steeringwheels;

    // Constructor or initialization method
    public void Initialize()
    {
        _list = new List<WheelCollider> { FRWheel, FLWheel, RRWheel, RLWheel };
        _steeringwheels = new List<WheelCollider> { FRWheel, FLWheel };
    }
}

[System.Serializable]
public class WheelMeshes
{
    public MeshRenderer FRWheel;
    public MeshRenderer FLWheel;
    public MeshRenderer RRWheel;
    public MeshRenderer RLWheel;

    // Private list initialized once
    private List<MeshRenderer> _list;

    // Public property to access the list
    public List<MeshRenderer> list => _list;

    // Initialize method to create the list once
    public void Initialize()
    {
        _list = new List<MeshRenderer> { FRWheel, FLWheel, RRWheel, RLWheel };
    }
}

// [System.Serializable]
// public class WheelParticles
// {
//     public ParticleSystem FRWheel;
//     public ParticleSystem FLWheel;
//     public ParticleSystem RRWheel;
//     public ParticleSystem RLWheel;

//     public TrailRenderer FRWheelTrail;
//     public TrailRenderer FLWheelTrail;
//     public TrailRenderer RRWheelTrail;
//     public TrailRenderer RLWheelTrail;

//     // Private lists initialized once
//     private List<ParticleSystem> _listParticle;
//     private List<TrailRenderer> _listTrail;

//     // Public properties to access the lists
//     public List<ParticleSystem> listParticle => _listParticle;
//     public List<TrailRenderer> listTrail => _listTrail;

//     // Initialize method to create the lists once
//     public void Initialize()
//     {
//         _listParticle = new List<ParticleSystem> { FRWheel, FLWheel, RRWheel, RLWheel };
//         _listTrail = new List<TrailRenderer> { FRWheelTrail, FLWheelTrail, RRWheelTrail, RLWheelTrail };
//     }
// }
