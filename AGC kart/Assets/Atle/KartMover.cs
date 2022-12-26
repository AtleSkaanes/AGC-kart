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
    [Header("secret stats")]
    [SerializeField] float SigmoidCurvePosition = 0f;

    bool prevDirection;

    Rigidbody rb;

    public PlayerInputActions playerInput;

    InputAction drive;
    InputAction reverse;
    InputAction turn;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = new PlayerInputActions();
    }


    private void OnEnable()
    {
        drive = playerInput.Player.Drive;
        drive.Enable();
        //drive.performed += CalculateSpeed;

        reverse = playerInput.Player.Reverse;
        reverse.Enable();

        turn = playerInput.Player.Turning;
        turn.Enable();

    }

    private void OnDisable()
    {
        drive.Disable();
        reverse.Disable();
        turn.Disable();
    }


    void Update()
    {
        if (SigmoidCurvePosition > Mathf.Epsilon && !drive.IsPressed() && !reverse.IsPressed()) 
            SigmoidCurvePosition -= 1 * Time.deltaTime;

        if (drive.IsPressed())
        {
            CalculateSpeed(true);
            rb.AddRelativeForce(new Vector3(-currentSpeed, 0, 0));
        }
        if (reverse.IsPressed())
        {
            CalculateSpeed(false);
            rb.AddRelativeForce(new Vector3(currentSpeed, 0, 0));
        }

        if (MathF.Abs(rb.velocity.magnitude) > Mathf.Epsilon && turn.IsPressed())
        {
            rb.AddTorque(new Vector3(0, turn.ReadValue<Vector2>().x * turnSpeed, 0));
        }

        Debug.Log("can turn = " + (MathF.Abs(rb.velocity.magnitude) > Mathf.Epsilon));

    }

    void CalculateSpeed(bool isDrivingForwards)
    {
        if (prevDirection != isDrivingForwards)
            SigmoidCurvePosition = 0;

        SigmoidCurvePosition += 1 * Time.deltaTime;
        currentSpeed = CalculateSigmoidcurve(SigmoidCurvePosition, isDrivingForwards);

        prevDirection = isDrivingForwards;
    }

    float CalculateSigmoidcurve(float xPos, bool isDrivingForwards)
    {
        float acc = isDrivingForwards ? forwardsAcceleration : backwardsAcceleration;
        float maxSpeed = isDrivingForwards ? maxForwardSpeed : maxBackwardsSpeed;
        float startSpeed = isDrivingForwards ? forwardsStartSpeed : backwardsStartSpeed;

        // Equation to find the inclination of the sigmoid curve wher it cross' the y-axis at the given start-speed value
        float inclination = (Mathf.Log((maxSpeed - startSpeed) / startSpeed) / acc);

        // Returns the value of the sigmoid function from the given x-value
        return (maxSpeed / (1 + Mathf.Exp((float)-inclination * (xPos - acc))));
    }
}
