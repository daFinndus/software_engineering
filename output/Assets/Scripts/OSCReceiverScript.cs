using System.Collections.Generic;
using System;
using extOSC;
using UnityEngine;
using UnityEngine.UI;
using System.Net;

public class ReceiverScript : MonoBehaviour
{
    // Input port describes the port that the output uses to receive messages
    private int inputPort = 13575;

    Text ipInput;
    Text portInput;

    Text debug;
    Text messages;

    private OSCReceiver receiver;
    private OSCTransmitter transmitter;

    private string outputIP;
    private int outputPort;

    public void Start()
    {
        DeclareComponents();

        // This is to prevent conflict errors
        receiver.LocalPort = inputPort;

        ipInput.text = "This is your local ip: " + GetLocalIPAddress();
    }

    /// <summary>
    /// This function declares every component that is needed for the script to work.
    /// </summary>
    private void DeclareComponents()
    {
        // Declare the input and text fields for the osc connection
        ipInput = GameObject.FindGameObjectWithTag("IP Input").GetComponent<Text>();
        portInput = GameObject.FindGameObjectWithTag("Port Input").GetComponent<Text>();

        // Declare the debug and osc message text field
        debug = GameObject.FindGameObjectWithTag("Debug").GetComponent<Text>();
        messages = GameObject.FindGameObjectWithTag("Messages").GetComponent<Text>();

        // Declare the transmitter and receiver for the osc connection
        transmitter = gameObject.GetComponent<OSCTransmitter>();
        receiver = gameObject.GetComponent<OSCReceiver>();
    }

    /// <summary>
    /// This function connects the system to the controller. 
    /// It opens the listener to receive osc messages.
    /// </summary>
    public void ConnectToController()
    {
        try
        {
            if (portInput.text != "")
            {
                inputPort = Int32.Parse(portInput.text);
                receiver.LocalPort = inputPort;
            }

            messages = GameObject.FindGameObjectWithTag("Messages").GetComponent<Text>();
            messages.text = "";

            receiver.enabled = true;

            Debug.Log("Listening on port: " + receiver.LocalPort);

            // Generic listener for debugging
            receiver.Bind("/*", MessageReceived);
            receiver.Bind("/connect", Connect);
        }
        catch
        {
            HandleError("Error while connecting to the controller.");
        }
    }

    /// <summary>
    /// This function is called when the system receives a message from the controller.
    /// </summary>
    /// <param name="message">The osc message.</param>
    protected void MessageReceived(OSCMessage message)
    {
        Debug.Log("Received a message for address: " + message.Address);

        Text messages = GameObject.FindGameObjectWithTag("Messages").GetComponent<Text>();

        messages.text = message.ToString();
    }

    /// <summary>
    /// This function is called when the system receives the connect message from the controller.
    /// </summary>
    /// <param name="message">The osc message.</param>
    protected void Connect(OSCMessage message)
    {
        Debug.Log("Successfully connected to the controller.");

        outputIP = message.Values[0].StringValue;
        outputPort = (int)message.Values[1].IntValue;

        ConnectToInput();
    }

    /// <summary>
    /// This function sends one message to the input to initialize the connection.
    /// </summary>
    private void ConnectToInput()
    {
        transmitter.RemoteHost = outputIP;
        transmitter.RemotePort = outputPort;
        transmitter.Connect();

        Debug.Log("Connected to input: " + transmitter.RemoteHost + ":" + transmitter.RemotePort);
        transmitter.Send(new OSCMessage("/connect"));

        transmitter.enabled = false;
    }

    /// <summary>
    /// This function returns the local ip address of the system.
    /// </summary>
    /// <returns>The local ip adress of the system.</returns>
    /// <exception cref="System.Exception">No network adapters were found.</exception>
    public static string GetLocalIPAddress()
    {
        var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new System.Exception("No network adapters with an IPv4 address in the system!");
    }

    /// <summary>
    /// This function is called when the system receives an error.
    /// </summary>
    /// <param name="error">The error message.</param>
    private void HandleError(string error)
    {
        Debug.LogError(error);
        debug.text = error;
    }
}
