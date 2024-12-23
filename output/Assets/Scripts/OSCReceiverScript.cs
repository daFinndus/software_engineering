using extOSC;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ReceiverScript : MonoBehaviour
{
    // Input port describes the port that the output uses to receive messages
    private int inputPort = 13575;

    private Text ipInput;
    private Text portInput;

    private string outputIP;
    private int outputPort;

    private bool connected = false;

    private Text debug;
    private Text messages;

    private OSCReceiver receiver;
    private OSCTransmitter transmitter;


    public void Start()
    {
        DeclareComponents();
        InitializeReceiver();
        DisplayLocalIPAdress();
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

        Debug.Log("Declared all components successfully.");
    }

    /// <summary>
    /// This function initializes the receiver for the osc connection.
    /// </summary>
    private void InitializeReceiver()
    {
        receiver.enabled = false;
        receiver.LocalPort = inputPort;

        // Generic listener
        receiver.Bind("/*", MessageReceived);
        receiver.Bind("/connect", Connect);

        Debug.Log("The receiver is initialized.");
    }

    /// <summary>
    /// This function displays the local ip address of the system.
    /// </summary>
    private void DisplayLocalIPAdress()
    {
        ipInput.text = $"This is your local IP: {GetLocalIPAddress()}";
    }

    /// <summary>
    /// This function connects the system to the controller. 
    /// It opens the listener to receive osc messages.
    /// </summary>
    public void ConnectToController()
    {
        try
        {
            if (int.TryParse(portInput.text, out int parsedPort))
            {
                Debug.Log("Successfully parsed the port.");
                inputPort = parsedPort;
                receiver.LocalPort = inputPort;
            }

            ClearMessages();
            receiver.enabled = true;

            Debug.Log($"Listening on port: {receiver.LocalPort}");
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
        Debug.Log($"Received message for address: {message.Address}");
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
        outputPort = message.Values[1].IntValue;

        InitializeTransmitter();
    }

    /// <summary>
    /// Initializes the transmitter for the osc connection and sends a message to the input.
    /// </summary>
    private void InitializeTransmitter()
    {
        transmitter.RemoteHost = outputIP;
        transmitter.RemotePort = outputPort;
        transmitter.Connect();

        Debug.Log("Sending to input: " + transmitter.RemoteHost + ":" + transmitter.RemotePort);
        transmitter.Send(new OSCMessage("/connect"));

        transmitter.enabled = false;
        StartCoroutine(WaitForInput());
    }

    /// <summary>
    /// This function listens to /gyro messages from the input.
    /// If it receives one it changes the scene to the GyroScene.
    /// </summary>
    /// <summary> Waits for /gyro messages to establish the connection. </summary>
    private System.Collections.IEnumerator WaitForInput()
    {
        if (!connected)
        {
            receiver.Bind("/start", message =>
            {
                Debug.Log("Received the startup message.");
                connected = true;

                ChangeScene();
            });
            yield return null; // Wait for the next frame
        }
    }

    /// <summary>
    /// This function changes the scene to the GyroScene.
    /// It also transfers the current OSCTransmitter object.
    /// </summary>
    private void ChangeScene()
    {
        SceneManager.LoadScene("BowlingScene");
        DontDestroyOnLoad(receiver);
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

    /// <summary> 
    /// Clears the messages text field. 
    /// </summary>
    private void ClearMessages()
    {
        messages.text = string.Empty;
    }
}