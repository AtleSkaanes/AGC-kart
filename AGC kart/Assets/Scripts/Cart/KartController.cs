using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class KartController : MonoBehaviour
{
    KartInputs kartInputs;

    InputAction drive;
    InputAction turning;
    InputAction breaking;

    float horizontalInput;
    float verticalInput;
    float currentSteerAngle;
    float currentBreakForce;
    bool isBreaking;

    [Header("Stats")]
    [SerializeField] private float motorForce;
    [SerializeField] private float breakForce;
    [SerializeField] private float maxSteeringAngle;

    [Header("Colliders")]
    [SerializeField] private WheelCollider frontLeftCollider;
    [SerializeField] private WheelCollider frontRightCollider;
    [SerializeField] private WheelCollider rearLeftCollider;
    [SerializeField] private WheelCollider rearRightCollider;

    [Header("Transforms")]
    [SerializeField] private Transform frontLeftTransform;
    [SerializeField] private Transform frontRightTransform;
    [SerializeField] private Transform rearLeftTransform;
    [SerializeField] private Transform rearRightTransform;

    private void Awake ()
    {
        kartInputs = new KartInputs();
    }

    private void FixedUpdate ()
    {
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
    }


    void HandleMotor ()
    {
        //add force to the front wheels based on the KartInput
        frontLeftCollider.motorTorque = verticalInput * motorForce;
        frontRightCollider.motorTorque = verticalInput * motorForce;
        currentBreakForce = isBreaking ? breakForce : 0f;
        
        if (isBreaking)
        {
            ApplyBreaking();
        }
    }

    void ApplyBreaking ()
    {
        //set the breaktorque to how much we are breaking
        frontLeftCollider.brakeTorque = currentBreakForce;
        frontRightCollider.brakeTorque = currentBreakForce;
        rearLeftCollider.brakeTorque = currentBreakForce;
        rearRightCollider.brakeTorque = currentBreakForce;
    }

    void GetInput()
    {
        verticalInput = drive.ReadValue<float>();
        horizontalInput = turning.ReadValue<float>();
        isBreaking = breaking.IsPressed();

        Debug.Log("verticalInput = " + verticalInput);
        Debug.Log("horizontalInput = " + horizontalInput);
        Debug.Log("isBreaking = " + isBreaking);
    }

    void HandleSteering ()
    {
        currentSteerAngle = maxSteeringAngle * horizontalInput;

        //set the steering angle of the wheels to the current angle
        frontLeftCollider.steerAngle = currentSteerAngle;
        frontRightCollider.steerAngle= currentSteerAngle;
    }

    private void UpdateWheels ()
    {
        UpdateSingleWheel(frontLeftCollider, frontLeftTransform);
        UpdateSingleWheel(frontRightCollider, frontRightTransform);
        UpdateSingleWheel(rearLeftCollider, rearLeftTransform);
        UpdateSingleWheel(rearRightCollider, rearRightTransform);

    }

    private void UpdateSingleWheel (WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);

        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////
    private void OnEnable ()
    {
        drive = kartInputs.Kart.Drive;
        drive.Enable();

        turning = kartInputs.Kart.Turning;
        turning.Enable();

        breaking = kartInputs.Kart.Breaking;
        breaking.Enable();
    }

    private void OnDisable ()
    {
        drive.Disable();
        turning.Disable();
        breaking.Disable();
    }
}
