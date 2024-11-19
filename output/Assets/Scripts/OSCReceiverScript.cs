using System.Collections.Generic;
using System;
using extOSC;
using UnityEngine;
using UnityEngine.UI;
using System.Net;

public class ReceiverScript : MonoBehaviour
{
    private int receivePort = 13575;

    Text address;
    Text messages;

    OSCReceiver receiver;

    public void Start()
    {
        address = GameObject.FindGameObjectWithTag("Internet Address").GetComponent<Text>();
        address.text = "";

        receiver = gameObject.GetComponent<OSCReceiver>();
        address.text = "Local IP Address: " + receiver.LocalHost;
    }

    public void ConnectToOSCReceiver()
    {
        messages = GameObject.FindGameObjectWithTag("Messages").GetComponent<Text>();
        messages.text = "";

        receiver.LocalPort = receivePort;
        receiver.enabled = true;

        Debug.Log("Listening on port: " + receiver.LocalPort);

        // Generic listener for debugging
        receiver.Bind("/*", MessageReceived);
    }

    protected void MessageReceived(OSCMessage message)
    {

        Debug.Log("Received a message for address: " + message.Address);

        Text messages = GameObject.FindGameObjectWithTag("Messages").GetComponent<Text>();

        messages.text += message;
    }
}
