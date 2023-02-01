using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KartMover : MonoBehaviour
{
    [Header("Current stats")]
    [SerializeField] float currentSpeed;
    [SerializeField] Quaternion currentRotation;

    [Header("stats")]
    [SerializeField] float maxForwardSpeed = 20f;
    [SerializeField] float maxBackwardsSpeed = 10f;
    [SerializeField] float forwardsAcceleration = 10f;
    [SerializeField] float backwardsAcceleration = 20f;
    [SerializeField] float forwardsStartSpeed = 1f;
    [SerializeField] float backwardsStartSpeed = 1f;
    [SerializeField] float turnSpeed = 50f;

  //  Sigmoid curve
  //  [Header("secret stats")]
  //  [SerializeField] float SigmoidCurvePosition = 0f;

    public PlayerInputActions playerInput;

    InputAction drive;
    InputAction turn;

    float verticalInput;
    float horizontalInput;


    void Awake()
    {
        playerInput = new PlayerInputActions();
    }


    void FixedUpdate()
    {
        ReadInputs();
        Drive();
    }

    void ReadInputs ()
    {
        verticalInput = drive.ReadValue<float>();
        horizontalInput = turn.ReadValue<float>();
    }

    void Drive ()
    {
        transform.position += transform.rotation * new Vector3(horizontalInput, 0, verticalInput).normalized;
    }

    //float CalculateSigmoidcurve(float xPos, bool isDrivingForwards)
    //{
    //    float acc = isDrivingForwards ? forwardsAcceleration : backwardsAcceleration;
    //    float maxSpeed = isDrivingForwards ? maxForwardSpeed : maxBackwardsSpeed;
    //    float startSpeed = isDrivingForwards ? forwardsStartSpeed : backwardsStartSpeed;

    //    // Equation to find the inclination of the sigmoid curve wher it cross' the y-axis at the given start-speed value
    //    float inclination = (Mathf.Log((maxSpeed - startSpeed) / startSpeed) / acc);

    //    // Returns the value of the sigmoid function from the given x-value
    //    return (maxSpeed / (1 + Mathf.Exp((float)-inclination * (xPos - acc))));
    //}

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    private void OnEnable ()
    {
        drive = playerInput.Player.Drive;
        drive.Enable();

        turn = playerInput.Player.Turning;
        turn.Enable();

    }

    private void OnDisable ()
    {
        drive.Disable();
        turn.Disable();
    }
}
