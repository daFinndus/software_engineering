using System.Collections;
using System.Collections.Generic;
using extOSC;
using UnityEngine;

public class GyroScript : MonoBehaviour
{
    private OSCTransmitter transmitter;
    private bool buttonPressed = false;

    // Start is called before the first frame update
    public void Start()
    {
        transmitter = GameObject.Find("OSCTransmitter").GetComponent<OSCTransmitter>();
        Debug.Log($"Transmitter is {transmitter.RemoteHost}:{transmitter.RemotePort}");

        Input.gyro.enabled = true;
        Debug.Log("\nGyro is now enabled!");
    }

    public void Update()
    {
        if (buttonPressed)
        {
            Debug.Log("Button is pressed!");
            GetGyro();

            OSCMessage message = new OSCMessage("/gyro");
            message.AddValue(OSCValue.Float(Input.gyro.attitude.x));
            message.AddValue(OSCValue.Float(Input.gyro.attitude.y));
            message.AddValue(OSCValue.Float(Input.gyro.attitude.z));
            message.AddValue(OSCValue.Float(Input.gyro.attitude.w));

            transmitter.Send(message);
        }
    }

    /// <summary>
    /// This function is called when the button is pressed
    /// </summary>
    public void PressButtonDown()
    {
        buttonPressed = true;
    }

    /// <summary>
    /// This function is called when the button is released
    /// </summary>
    public void PressButtonUp()
    {
        buttonPressed = false;
    }

    /// <summary>
    /// This function returns the three gyro attitude values
    /// </summary>
    public Quaternion GetGyro()
    {
        float x = Input.gyro.attitude.x;
        float y = Input.gyro.attitude.y;
        float z = Input.gyro.attitude.z;
        float w = Input.gyro.attitude.w;

        Debug.Log("x: " + x + ", y: " + y + ", z: " + z + ", w: " + w);
        return Input.gyro.attitude;
    }
}