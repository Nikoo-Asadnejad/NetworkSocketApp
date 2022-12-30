using System.Net;
using System.Net.Sockets;
using System.Text;

Console.ForegroundColor = ConsoleColor.Blue;
Console.WriteLine("Socket Client is running ...");

IPHostEntry ipHostInfo = await Dns.GetHostEntryAsync("localhost");
IPAddress ipAddress = ipHostInfo.AddressList[1];
IPEndPoint ipEndPoint = new(ipAddress, 11_000);

Console.WriteLine($"Server ip is {ipEndPoint}");
Console.ResetColor();
Console.WriteLine();

using Socket client = new(
    ipEndPoint.AddressFamily,
    SocketType.Stream,
    ProtocolType.Tcp);

await client.ConnectAsync(ipEndPoint);

while (true)
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("Enter your message : ");
    Console.ResetColor();
    string message = Console.ReadLine();
    
    
    while (true)
    {
        // Send message.
        message += "<|EOM|>";
        var messageBytes = Encoding.UTF8.GetBytes(message);
        _ = await client.SendAsync(messageBytes, SocketFlags.None);
        Console.WriteLine($"Socket client sent message: \"{message.Replace("<|EOM|>","")}\"");

        // Receive ack.
        byte[] ackBuffer = new byte[1_024];
        int receivedAck = await client.ReceiveAsync(ackBuffer, SocketFlags.None);
        string aknowledge = Encoding.UTF8.GetString(ackBuffer, 0, receivedAck);

        if (aknowledge.Contains("<|ACK|>"))
        {
            Console.WriteLine($"Client has received acknowledgment {aknowledge}");
            break;
        }
           
    
        // Sample output:
        //     Socket client sent message: "Hi friends 👋!<|EOM|>"
        //     Socket client received acknowledgment: "<|ACK|>"
    }

    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("Press F1 to exit or press any key to continue ...");
    if(Console.ReadKey().Key == ConsoleKey.F1)
        break;
    
    Console.ResetColor();
}

client.Shutdown(SocketShutdown.Both);