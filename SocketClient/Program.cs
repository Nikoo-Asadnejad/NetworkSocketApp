// See https://aka.ms/new-console-template for more information

using System.Net;
using System.Net.Sockets;
using System.Text;

IPHostEntry ipHostInfo = await Dns.GetHostEntryAsync("localhost");
IPAddress ipAddress = ipHostInfo.AddressList[1];

IPEndPoint ipEndPoint = new(ipAddress, 11_000);

using Socket client = new(
    ipEndPoint.AddressFamily, 
    SocketType.Stream, 
    ProtocolType.Tcp);

await client.ConnectAsync(ipEndPoint);

while (true)
{
    string message = Console.ReadLine();
    
    while (true)
    {
        // Send message.
        message += "<|EOM|>";
        var messageBytes = Encoding.UTF8.GetBytes(message);
        _ = await client.SendAsync(messageBytes, SocketFlags.None);
        Console.WriteLine($"Socket client sent message: \"{message}\"");

        // Receive ack.
        var buffer = new byte[1_024];
        var received = await client.ReceiveAsync(buffer, SocketFlags.None);
        var response = Encoding.UTF8.GetString(buffer, 0, received);
        
        if(response.Contains("acknowledgment"))
            break;
    
        // Sample output:
        //     Socket client sent message: "Hi friends 👋!<|EOM|>"
        //     Socket client received acknowledgment: "<|ACK|>"
    }
    
    if(Console.ReadKey().Key == ConsoleKey.F1)
        break;
}




client.Shutdown(SocketShutdown.Both);