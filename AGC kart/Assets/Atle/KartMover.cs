using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class KartMover : MonoBehaviour
{
    [Header("Current stats")]
    [SerializeField] float currentSpeed;
    [SerializeField] Quaternion currentRotation;

    [Header("\t---STATS---")]
    [Header("Speeds")]
    [SerializeField] float maxForwardSpeed = 2f;
    [SerializeField] float maxBackwardsSpeed = 1f;
    [SerializeField] float minSpeed = 0.1f;
    [Header("Acceleration")]
    [SerializeField] float forwardsAcceleration = 0.4f;
    [SerializeField] float backwardsAcceleration = 0.5f;
    [SerializeField] float turnSpeed = 50f;
    [SerializeField] float drag = 0.2f;

    
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
        switch (verticalInput)
        {
            case 1:
                currentSpeed += forwardsAcceleration;
                break;

            case -1:
                currentSpeed -= backwardsAcceleration;
                break;

            default:
                currentSpeed = Mathf.Lerp(currentSpeed, 0, drag);

                if (MathF.Abs(currentSpeed) < minSpeed)
                    currentSpeed = 0;
                break;
        }
        currentSpeed = Mathf.Clamp(currentSpeed, -maxBackwardsSpeed, maxForwardSpeed);

        transform.position += transform.rotation * new Vector3(0, 0, currentSpeed);
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
