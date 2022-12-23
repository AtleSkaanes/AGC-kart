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

    //  Sigmoid curve
    [SerializeField] float SigmoidCurvePosition = 0f;

    public PlayerInputActions playerInput;

    InputAction drive;
    InputAction reverse;
    InputAction turn;


    void Awake()
    {
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
        if (SigmoidCurvePosition > Mathf.Epsilon && !drive.IsPressed()) 
            SigmoidCurvePosition -= 1 * Time.deltaTime;

        if (drive.IsPressed()) 
            CalculateSpeed(true);

        if (reverse.IsPressed())
            CalculateSpeed(false);
    }

    void CalculateSpeed(bool isDrivingForwards)
    {
        SigmoidCurvePosition += 1 * Time.deltaTime;
        currentSpeed = CalculateSigmoidcurve(SigmoidCurvePosition, isDrivingForwards);
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
