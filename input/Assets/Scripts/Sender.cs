using System.Net;
using extOSC;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Sender : MonoBehaviour
{
    // Output port is the port that the input uses to send messages to
    private string outputIP = "";
    private int outputPort = 13574;

    private InputField ipInput;
    private InputField portInput;

    private Text debug;

    private OSCTransmitter transmitter;

    public void Start()
    {
        DeclareComponents();
        InitializeTransmitter();
    }

    /// <summary>
    /// This function declares every component that is needed for the script to work.
    /// </summary>
    private void DeclareComponents()
    {
        // Declare the input and text fields for the osc connection
        ipInput = GameObject.FindGameObjectWithTag("Input IP").GetComponent<InputField>();
        portInput = GameObject.FindGameObjectWithTag("Input Port").GetComponent<InputField>();

        // Declare the debug text field
        debug = GameObject.FindGameObjectWithTag("Debug").GetComponent<Text>();
    }

    private void InitializeTransmitter()
    {
        // Declare the transmitter for the osc connection
        transmitter = GetComponent<extOSC.OSCTransmitter>();

        // This is to prevent conflict errors
        transmitter.RemotePort = outputPort;
        Debug.Log("Transmitter is sending to port: " + transmitter.RemotePort);
    }

    /// <summary>
    /// This function connects to a osc receiver with a given ip address and a port.
    /// It sends one initial osc message to represent a successful connection.
    /// </summary>
    public void ConnectToController()
    {
        try
        {
            if (string.IsNullOrEmpty(ipInput.text))
            {
                HandleError("Please enter a valid IP address.");
                return;
            }

            if (!string.IsNullOrEmpty(portInput.text))
            {
                outputPort = int.Parse(portInput.text);
            }

            outputIP = ipInput.text;

            Debug.Log($"Connecting to {outputIP}:{outputPort}");

            transmitter.RemoteHost = outputIP;
            transmitter.RemotePort = outputPort;
            transmitter.Connect();

            Debug.Log($"Sending message to {outputIP}:{outputPort}");

            OSCMessage message = new("/connect");
            message.AddValue(OSCValue.String("Hello Output! Greetings, Input."));
            transmitter.Send(message);

            // Wait for answer from input
            ChangeScene();
        }
        catch
        {
            HandleError("Error while connecting to controller.");
        }
    }

    /// <summary>
    /// This function changes the scene to the GyroScene.
    /// It also transfers the current OSCTransmitter object.
    /// </summary>
    private void ChangeScene()
    {
        DontDestroyOnLoad(transmitter);
        SceneManager.LoadScene("Gyro");
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
                    Debug.Log("APIPA address detected: " + ip.ToString());
                    continue;
                }

                Debug.Log("Found Local IP: " + ip.ToString());
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