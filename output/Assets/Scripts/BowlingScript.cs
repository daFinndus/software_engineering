using System.Collections.Generic;
using System.Linq;
using extOSC;
using Unity.VisualScripting;
using UnityEditor.VersionControl;
using UnityEngine;

public class BowlingScript : MonoBehaviour
{
    // This is all for the osc receiver
    private OSCReceiver receiverComponent;
    private GameObject receiverObject;
    private OSCReceiver receiverBowling;

    private string inputIP;
    private int inputPort;

    // This is for the Bowling Ball
    private Transform bowlingPosition;
    private Rigidbody bowlingRigidbody;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private bool ballReleased = false;

    // Gyro data
    private Vector3 attitude = Vector3.zero;
    private Quaternion rotationRate = new Quaternion(0, 0, 0, 0);
    private Vector3 acceleration = Vector3.zero;

    public float gyroTimeout = 0.15f;
    public float gyroMultiplier = 10f;
    public float forceMultiplier = 0.5f;

    public void Start()
    {
        InitializeReceiver();
        DeclareComponents();
    }

    /// <summary>
    /// This function declares the components that are needed for the script to work.
    /// </summary>
    private void DeclareComponents()
    {
        bowlingPosition = GetComponent<Transform>();
        bowlingRigidbody = GetComponent<Rigidbody>();

        initialPosition = bowlingPosition.position;
        initialRotation = bowlingPosition.rotation;

        if (bowlingRigidbody == null || bowlingPosition == null)
        {
            Debug.LogError("Bowling Rigidbody or Transform is not set!");
        }
    }

    /// <summary>
    /// This function copies the settings from the old receiver to the new receiver.
    /// Afterwards, it destroys the old receiver.
    /// </summary>
    private void InitializeReceiver()
    {
        // Set old receiver object and component
        receiverObject = GameObject.Find("OSCReceiver");
        receiverComponent = receiverObject.GetComponent<OSCReceiver>();
        receiverComponent.enabled = false;

        // Clear all binds
        inputIP = receiverComponent.LocalHost;
        inputPort = receiverComponent.LocalPort;

        Destroy(receiverObject);
        Debug.Log("Destroyed the old receiver.");

        receiverBowling = GameObject.Find("OSCReceiver Bowling").GetComponent<OSCReceiver>();
        receiverBowling.LocalHostMode = OSCLocalHostMode.Custom;
        receiverBowling.enabled = false;

        receiverBowling.LocalHost = inputIP;
        receiverBowling.LocalPort = inputPort;
        Debug.Log($"Receiver is {receiverBowling.LocalHost}:{receiverBowling.LocalPort}");

        receiverBowling.Bind("/gyro", OnReceive);
        receiverBowling.Bind("/finish", message => ReleaseBall());
        receiverBowling.enabled = true;


    }

    /// <summary>
    /// This function handles the receiving of messages.
    /// </summary>
    /// <param name="message">The received osc message.</param>
    private void OnReceive(OSCMessage message)
    {
        if (ballReleased)
        {
            ResetBall();
        }

        // Handle gyro data
        HandleGyro(message);
    }

    /// <summary>
    /// This function retrieves the gyro data from the osc message.
    /// It sorts it into a list and updates the position of the ball.
    /// </summary>
    /// <param name="message">The received osc message.</param>
    private void HandleGyro(OSCMessage message)
    {
        List<float> values = message.Values.Select(value => value.FloatValue).ToList();

        // Update last gyro data
        attitude = new Vector3(values[0], values[1], values[2]);

        // Save acceleration and rotation values
        rotationRate = new Quaternion(values[4], values[5], values[6], values[3]);
        acceleration = new Vector3(values[7], values[8], values[9]);

        UpdatePosition();
    }

    /// <summary>
    /// This function updates the position of the ball.
    /// It considers the rotation, acceleration and current position of the gyro.
    /// The ball is then moved accordingly.
    /// </summary>
    void UpdatePosition()
    {
        bowlingPosition.SetPositionAndRotation(new Vector3(
            initialPosition.x + attitude[0] * gyroMultiplier,
            initialPosition.y + attitude[1] * gyroMultiplier,
            initialPosition.z + attitude[2] * gyroMultiplier
        ), rotationRate);

        Debug.Log($"Successfully updated the position of the ball to {bowlingPosition.position}!");
    }

    /// <summary>
    /// Releases the ball with the last known force.
    /// </summary>
    private void ReleaseBall()
    {
        ballReleased = true;

        // Calculate release force with acceleration
        Vector3 releaseForce = acceleration * forceMultiplier;

        // Calculate angular velocity with rotation rate
        Vector3 angularVelocity = new Vector3(rotationRate.x, rotationRate.y, rotationRate.z) * gyroMultiplier;

        bowlingRigidbody.isKinematic = false;
        bowlingRigidbody.AddForce(releaseForce, ForceMode.Impulse);
        bowlingRigidbody.AddTorque(angularVelocity, ForceMode.Impulse);

        Debug.Log($"Ball is released!");

        Debug.Log($"Acceleration: {acceleration}, RotationRate: {rotationRate}");
        Debug.Log($"Calculated Force: {releaseForce}, Angular Velocity: {angularVelocity}");
    }

    /// <summary>
    /// Resets the ball to its initial position.
    /// </summary>
    private void ResetBall()
    {
        ballReleased = false;

        bowlingRigidbody.isKinematic = true;
        bowlingPosition.SetPositionAndRotation(initialPosition, initialRotation);

        Debug.Log("Ball is reset!");
    }
}