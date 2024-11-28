using System.Collections;
using System.Collections.Generic;
using extOSC;
using UnityEngine;

/// <summary>
/// This class is responsible for sending the gyro attitude values to the server.
/// It acts as a kind of controller for the gyro values.
/// </summary>
public class GyroScript : MonoBehaviour
{
    private OSCTransmitter transmitter;
    private OSCReceiver receiver;

    private bool buttonPressed = false;

    void Start()
    {
        InitializeReceiver();
        InitializeGyro();
    }

    /// <summary>
    /// This function initializes the transmitter and receiver.
    /// It then destroys the receiver used for establishing the connection.
    /// This step can be skipped if needed.
    /// </summary>
    private void InitializeReceiver()
    {
        transmitter = GameObject.Find("OSCTransmitter").GetComponent<OSCTransmitter>();
        receiver = GameObject.Find("OSCTransmitter").GetComponent<OSCReceiver>();

        Debug.Log($"Transmitter is {transmitter.RemoteHost}:{transmitter.RemotePort}");
        Debug.Log($"Receiver is {receiver.LocalHost}:{receiver.LocalPort}");
    }

    /// <summary>
    /// This function initializes the gyro.
    /// </summary>
    private void InitializeGyro()
    {
        Input.gyro.enabled = true;
        Input.gyro.updateInterval = 0.01f;
        Debug.Log("\nGyro is now enabled!");
    }

    void Update()
    {
        if (buttonPressed)
        {
            Debug.Log("Button is pressed!");
            float[] values = GetGyro();

            OSCMessage message = new("/gyro");
            
            foreach (var value in values)
            {
                message.AddValue(OSCValue.Float(value));
            }

            transmitter.Send(message);
        }
    }

    /// <summary>
    /// This function is called when the button is pressed.
    /// </summary>
    public void PressButtonDown()
    {
        buttonPressed = true;
    }

    /// <summary>
    /// This function is called when the button is released.
    /// It sends a message to the server.
    /// </summary>
    public void PressButtonUp()
    {
        buttonPressed = false;

        OSCMessage message = new("/finish");
        message.AddValue(OSCValue.String("Button is released!"));
        transmitter.Send(message);
    }

    /// <summary>
    /// This function returns the three gyro values
    /// It gets the values for x, y, z, w, the rotation and acceleration values
    /// </summary>
    /// <returns>An array of gyro values.</returns>
    public float[] GetGyro()
    {
        float x = Input.gyro.attitude.x;
        float y = Input.gyro.attitude.y;
        float z = Input.gyro.attitude.z;
        float w = Input.gyro.attitude.w;

        float rotationX = Input.gyro.rotationRate.x;
        float rotationY = Input.gyro.rotationRate.y;
        float rotationZ = Input.gyro.rotationRate.z;

        float accelerationX = Input.gyro.userAcceleration.x;
        float accelerationY = Input.gyro.userAcceleration.y;
        float accelerationZ = Input.gyro.userAcceleration.z;

        float[] values = { x, y, z, w, rotationX, rotationY, rotationZ, accelerationX, accelerationY, accelerationZ };

        return values;
    }
}