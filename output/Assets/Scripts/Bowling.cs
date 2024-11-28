using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using extOSC;

public class Bowling : MonoBehaviour
{
    // This is all for the osc receiver
    private OSCReceiver receiver;

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
        receiver = GameObject.Find("OSCReceiver").GetComponent<OSCReceiver>();

        if (receiver == null)
        {
            SceneManager.LoadScene("Menu");
        }

        receiver.enabled = true;

        Debug.Log($"Receiver is {receiver.LocalHost}:{receiver.LocalPort}");

        receiver.Bind("/gyro", OnReceive);
        receiver.Bind("/finish", message => ReleaseBall());
        receiver.enabled = true;
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
        attitude = new Vector3(values[0], 0, values[2]);

        // Save acceleration and rotation values
        rotationRate = new Quaternion(values[4], values[5], values[6], values[3]);
        acceleration = new Vector3(values[9], 0, values[7]);

        UpdatePosition();
    }

    /// <summary>
    /// This function updates the position of the ball.
    /// It considers the rotation, acceleration and current position of the gyro.
    /// The ball is then moved accordingly.
    /// </summary>
    void UpdatePosition()
    {
        if (!Application.isEditor) return; // Only use while debugging, could result in conflicts with the physics engine

        bowlingPosition.SetPositionAndRotation(new Vector3(
            initialPosition.x + attitude[0] * gyroMultiplier,
            initialPosition.y + attitude[1] * gyroMultiplier,
            initialPosition.z + attitude[2] * gyroMultiplier
        ), rotationRate);

        Debug.Log($"Debug Position updated to {bowlingPosition.position}");
    }

    /// <summary>
    /// Releases the ball with the last known force.
    /// </summary>
    private void ReleaseBall()
    {
        ballReleased = true;

        // Translate acceleration to world space
        Vector3 worldAcceleration = bowlingPosition.TransformDirection(acceleration);

        // Prüfe, ob die Wurfbewegung ausreichend ist
        if (worldAcceleration.magnitude < 1f)
        {
            Debug.Log("Throw too weak, ignoring release.");
            return;
        }

        // Calculate angular velocity with rotation rate and release force with acceleration
        Vector3 angularVelocity = rotationRate.eulerAngles;
        Vector3 releaseForce = worldAcceleration * forceMultiplier;

        bowlingRigidbody.isKinematic = false;
        bowlingRigidbody.AddForce(releaseForce, ForceMode.Impulse);
        bowlingRigidbody.AddTorque(angularVelocity, ForceMode.Impulse);

        Debug.Log($"Ball released with force {releaseForce} and torque {angularVelocity}");
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