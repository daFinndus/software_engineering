using System.Collections;
using System.Collections.Generic;
using extOSC;
using UnityEngine;
using UnityEngine.UI;

public class SenderScript : MonoBehaviour
{

    private int receivePort = 13574;

    public void ConnectToOSCReceiver()
    {
        var transmitter = GetComponent<extOSC.OSCTransmitter>();
        var inputField = GameObject.FindGameObjectWithTag("Internet Adress").GetComponent<InputField>();

        Debug.Log("Connecting to " + inputField.text);

        transmitter.RemoteHost = inputField.text;
        transmitter.RemotePort = receivePort;
        transmitter.Connect();

        OSCMessage message = new("/controller");

        message.AddValue(OSCValue.String("Not converted for output"));
        message.AddValue(OSCValue.Array(OSCValue.Int(1), OSCValue.Int(2), OSCValue.Int(3)));

        Debug.Log("Going to send:" + message + " to " + inputField.text + ":" + transmitter.RemotePort);
        transmitter.Send(message);
    }
}
