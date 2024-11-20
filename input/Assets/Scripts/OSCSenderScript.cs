using System.Net;
using extOSC;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SenderScript : MonoBehaviour
{
    // Input port describes the port that the input uses to receive messages
    // Output port describes the port that the input uses to send messages to
    private string inputIP = "";
    private int inputPort = 13576;

    private string outputIP = "";
    private int outputPort = 13574;

    private InputField ipInput;
    private Text placeholder;

    private InputField portInput;

    private InputField ipController;
    private InputField portController;

    private Text debug;

    private OSCTransmitter transmitter;
    private OSCReceiver receiver;

    public void Start()
    {
        DeclareComponents();
        InitializeTransmitter();
        SetLocalIP();
    }

    /// <summary>
    /// This function declares every component that is needed for the script to work.
    /// </summary>
    private void DeclareComponents()
    {
        // Declare the input and text fields for the osc connection
        ipInput = GameObject.FindGameObjectWithTag("Input IP").GetComponent<InputField>();
        placeholder = ipInput.placeholder.GetComponent<Text>();
        portInput = GameObject.FindGameObjectWithTag("Input Port").GetComponent<InputField>();
        ipController = GameObject.FindGameObjectWithTag("Controller IP").GetComponent<InputField>();
        portController = GameObject.FindGameObjectWithTag("Controller Port").GetComponent<InputField>();

        // Declare the debug text field
        debug = GameObject.FindGameObjectWithTag("Debug").GetComponent<Text>();
    }

    private void InitializeTransmitter()
    {
        // Declare the transmitter and receiver for the osc connection
        transmitter = GetComponent<extOSC.OSCTransmitter>();
        receiver = GetComponent<extOSC.OSCReceiver>();
        receiver.enabled = false;

        // This has to be custom to set the local ip address
        receiver.LocalHostMode = OSCLocalHostMode.Custom;

        // This is to prevent conflict errors
        transmitter.RemotePort = outputPort;
        Debug.Log("Transmitter is sending to port: " + transmitter.RemotePort);

        receiver.LocalPort = inputPort;
        Debug.Log("Receiver is listening on port: " + receiver.LocalPort);
    }

    private void SetLocalIP()
    {
        string ip = GetLocalIPAddress();

        if (string.IsNullOrEmpty(ip))
        {
            placeholder.text = "No network adapters were found.";
            inputIP = "";

            receiver.LocalHost = "127.0.0.1";
        }
        else
        {
            placeholder.text = "This is your local ip: " + ip;
            Debug.Log("Set input IP to: " + ip);
            inputIP = ip;

            receiver.LocalHost = ip;
        }
    }

    /// <summary>
    /// This function connects to a osc receiver with a given ip address and a port.
    /// It sends one initial osc message to represent a successful connection.
    /// </summary>
    public void ConnectToController()
    {
        try
        {
            if (string.IsNullOrEmpty(ipController.text))
            {
                HandleError("Please enter a valid IP address.");
                return;
            }

            if (!string.IsNullOrEmpty(portController.text))
            {
                outputPort = int.Parse(portController.text);
            }

            outputIP = ipController.text;

            Debug.Log($"Connecting to {outputIP}:{outputPort}");

            transmitter.RemoteHost = outputIP;
            transmitter.RemotePort = outputPort;
            transmitter.Connect();

            // Send initial message to connect to receiver
            if (!string.IsNullOrEmpty(ipInput.text))
            {
                Debug.Log("Setting input IP to: " + ipInput.text);
                inputIP = ipInput.text;
                receiver.LocalHost = inputIP;
            }

            if (!string.IsNullOrEmpty(portInput.text))
            {
                Debug.Log("Setting input port to: " + portInput.text);
                inputPort = int.Parse(portInput.text);
                receiver.LocalPort = inputPort;
            }

            // Disable and re-enable the receiver to apply the new settings
            receiver.enabled = false;
            receiver.enabled = true;

            Debug.Log($"Sending message to {outputIP}:{outputPort}");

            Debug.Log($"Sending message with input IP: {inputIP} and input port: {inputPort}");
            OSCMessage message = new("/connect");
            message.AddValue(OSCValue.String(inputIP));
            message.AddValue(OSCValue.Int(inputPort));
            transmitter.Send(message);

            // Wait for answer from input
            ConnectToOutput();
        }
        catch
        {
            HandleError("Error while connecting to controller.");
        }
    }

    /// <summary>
    /// This function is called when the system receives a message from the output.
    /// </summary>
    protected void Connect(OSCMessage message)
    {
        Debug.Log("Connection is initialized.");
        receiver.enabled = false;

        // Send initial message to start the game
        message = new("/start");
        message.AddValue(OSCValue.Int(0));
        transmitter.Send(message);
        Debug.Log($"Going to send {message}!");

        ChangeScene();
    }

    /// <summary>
    /// This function is called when the system receives a message from the output.
    /// </summary>
    private void ConnectToOutput()
    {
        try
        {
            receiver.enabled = true;
            Debug.Log($"Listening on port: {receiver.LocalHost}:{receiver.LocalPort}");

            // Generic listener for the connect address
            receiver.Bind("/connect", Connect);
        }
        catch
        {
            HandleError("Error while connecting to output.");
        }
    }

    /// <summary>
    /// This function changes the scene to the GyroScene.
    /// It also transfers the current OSCTransmitter object.
    /// </summary>
    private void ChangeScene()
    {
        SceneManager.LoadScene("GyroScene");
        DontDestroyOnLoad(transmitter);
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
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && !IPAddress.IsLoopback(ip))
            {
                byte[] bytes = ip.GetAddressBytes();
                if (bytes[0] == 169 && bytes[1] == 254)
                {
                    // Skip APIPA addresses
                    continue;
                }
                return ip.ToString();
            }
        }

        Debug.Log("There is no IPv4 network adapter in the system.");
        return "";
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