using controller_crossplatform.Functions;

class Program
{
    static void Main()
    {
        List<string> addresses = LocalIP.GetLocalIPAdress();
        DisplayWelcomeMessage();
        DisplayLocalIPs(addresses);

        string receivePort = GetPort("Enter listener port: ", "13574");
        string senderIP = GetSenderIP();
        string senderPort = GetPort("Enter sender port: ", "13575");

        OSCSender sender = new(senderIP.Trim(), int.Parse(senderPort));
        OSCReceiver receiver = new(sender, int.Parse(receivePort));
        Thread receiverThread = new(receiver.ListenToSender);

        Console.WriteLine($"\nListener is listening to port: {receivePort}");
        Console.WriteLine($"Sender is sending to: {senderIP}:{senderPort}");

        receiverThread.Start();
    }

    static void DisplayWelcomeMessage()
    {
        Console.WriteLine("\nWelcome to the controller setup!");
    }

    static void DisplayLocalIPs(List<string> addresses)
    {
        foreach (string address in addresses)
        {
            Console.WriteLine("Found local IP: " + address);
        }
    }

    static string GetPort(string prompt, string defaultPort)
    {
        Console.WriteLine($"\nNow you should enter the port to listen to, by default {defaultPort}.");
        Console.Write(prompt);
        string portInput = Console.ReadLine()!;
        return string.IsNullOrEmpty(portInput) ? defaultPort : portInput;
    }

    static string GetSenderIP()
    {
        Console.WriteLine("\nNow enter the IP to send to, the local IP of the output.");
        string senderIP = "";
        while (senderIP.Length == 0)
        {
            Console.Write("Enter sender IP: ");
            senderIP = Console.ReadLine()!;
        }

        return senderIP;
    }
}