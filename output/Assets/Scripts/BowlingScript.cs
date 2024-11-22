using extOSC;
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
    private Vector3 lastGyroInput = Vector3.zero;
    private Vector3 previousGyroInput = Vector3.zero;

    private float lastGyroTime;
    private float previousGyroTime;

    public float gyroTimeout = 0.15f;
    public float gyroMultiplier = 10f;
    public float forceMultiplier = 0.5f;

    // Start is called before the first frame update
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
        bowlingPosition = GameObject.Find("Bowling").GetComponent<Transform>();
        bowlingRigidbody = GameObject.Find("Bowling").GetComponent<Rigidbody>();

        initialPosition = bowlingPosition.position;
        initialRotation = bowlingPosition.rotation;
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

        receiverBowling = GameObject.Find("OSCReceiver Bowling").GetComponent<OSCReceiver>();
        receiverBowling.LocalHostMode = OSCLocalHostMode.Custom;
        receiverBowling.enabled = false;

        receiverBowling.LocalHost = inputIP;
        receiverBowling.LocalPort = inputPort;
        Debug.Log($"Receiver is {receiverBowling.LocalHost}:{receiverBowling.LocalPort}");

        receiverBowling.Bind("/gyro", OnReceive);
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

    private void HandleGyro(OSCMessage message)
    {
        // Extract gyro data
        float xGyro = message.Values[0].FloatValue * gyroMultiplier;
        float yGyro = initialPosition.y;
        float zGyro = message.Values[2].FloatValue * gyroMultiplier;

        // Update last and previous gyro time and input
        previousGyroInput = lastGyroInput;
        previousGyroTime = lastGyroTime;

        lastGyroInput = new Vector3(xGyro, 0, 0);
        lastGyroTime = Time.time;

        UpdatePosition(new Vector3(xGyro, yGyro, zGyro));
    }

    void UpdatePosition(Vector3 position)
    {
        Debug.Log("Going to udpate the position!");
        bowlingPosition.position = position;
    }

    /// <summary>
    /// FixedUpdate checks for release conditions and applies force.
    /// </summary>
    public void FixedUpdate()
    {
        Debug.Log($"The last message was received before {Time.time - lastGyroTime}");

        // Check if the ball has been released and the gyro data has timed out
        if (!ballReleased && Time.time - lastGyroTime > gyroTimeout)
        {
            ReleaseBall();
        }
    }

    /// <summary>
    /// Releases the ball with the last known force.
    /// </summary>
    private void ReleaseBall()
    {
        ballReleased = true;

        // Calculate the difference in input and time to the previous message
        Vector3 deltaInput = lastGyroInput - previousGyroInput;
        float deltaTime = lastGyroTime - previousGyroTime;

        // If the time difference is zero, set it to a small value
        if (deltaTime <= 0) deltaTime = 0.01f;

        // Calculate force and velocity
        Vector3 velocity = deltaInput / deltaTime;
        Vector3 releaseForce = velocity * forceMultiplier;

        bowlingRigidbody.isKinematic = false;
        bowlingRigidbody.AddForce(releaseForce, ForceMode.Impulse);

        Debug.Log($"Throwing the ball with force: {lastGyroInput * 5.0f}");
        Debug.Log($"Ball is released!");
    }

    private void ResetBall()
    {
        ballReleased = false;

        bowlingRigidbody.isKinematic = true;
        bowlingPosition.SetPositionAndRotation(initialPosition, initialRotation);

        Debug.Log("Ball is reset!");
    }
}