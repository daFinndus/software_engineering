using System.Collections;
using System.Collections.Generic;
using extOSC;
using UnityEngine;
using UnityEngine.UI;

public class SenderScript : MonoBehaviour
{
    private int receivePort = 13574;

    private InputField inputField;
    private OSCTransmitter transmitter;

    public void ConnectToOSCReceiver()
    {
        transmitter = GetComponent<extOSC.OSCTransmitter>();
        inputField = GameObject.FindGameObjectWithTag("Internet Address").GetComponent<InputField>();

        Debug.Log("Connecting to " + inputField.text);

        transmitter.RemoteHost = inputField.text;
        transmitter.RemotePort = receivePort;
        transmitter.Connect();

        OSCMessage message = new("/controller");

        message.AddValue(OSCValue.String("Not converted for output"));
        message.AddValue(OSCValue.Array(OSCValue.Int(1), OSCValue.Int(2), OSCValue.Int(3)));

        transmitter.Send(message);
    }

    protected void SendMessage(OSCMessage message)
    {
        Debug.Log("Going to send:" + message + " to " + inputField.text + ":" + transmitter.RemotePort);
        transmitter.Send(message);
    }
}