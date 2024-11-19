using System.Collections.Generic;
using System;
using extOSC;
using UnityEngine;
using UnityEngine.UI;

public class ReceiverScript : MonoBehaviour
{
    private int receivePort = 13575;
    public void ConnectToOSCReceiver()
    {
        var messages = GameObject.FindGameObjectWithTag("Messages").GetComponent<Text>().text = "";
        var receiver = gameObject.GetComponent<OSCReceiver>() ?? gameObject.AddComponent<OSCReceiver>();

        receiver.LocalPort = receivePort;
        receiver.enabled = true;

        Debug.Log("Listening on port: " + receiver.LocalPort);

        // Generic listener for debugging
        receiver.Bind("/*", MessageReceived);
    }

    protected void MessageReceived(OSCMessage message)
    {

        Debug.Log("Received a message for address: " + message.Address);

        var messages = GameObject.FindGameObjectWithTag("Messages");

        messages.GetComponent<Text>().text += message;
    }
}
