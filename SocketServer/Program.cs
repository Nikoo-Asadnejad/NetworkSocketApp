using System.Net;
using System.Net.Sockets;
using System.Text;

Console.ForegroundColor = ConsoleColor.Blue;
Console.WriteLine("Socket Server is running ...");

IPHostEntry ipHostInfo = await Dns.GetHostEntryAsync("localhost");
IPAddress ipAddress = ipHostInfo.AddressList[1];
IPEndPoint ipEndPoint = new(ipAddress, 11_000);

Console.WriteLine($"Socket Server is listing on {ipEndPoint}");
Console.ResetColor();

using Socket listener = new(
    ipEndPoint.AddressFamily,
    SocketType.Stream,
    ProtocolType.Tcp);

listener.Bind(ipEndPoint);
listener.Listen(100);

var handler = await listener.AcceptAsync();
while (true)
{
    // Receive message.
    var ackBuffer = new byte[1_024];
    var received = await handler.ReceiveAsync(ackBuffer, SocketFlags.None);
    var response = Encoding.UTF8.GetString(ackBuffer, 0, received);
    
    var eom = "<|EOM|>";
    if (response.IndexOf(eom) > -1 /* is end of message */)
    {
        Console.WriteLine(
            $"Socket server received message: \"{response.Replace(eom, "")}\"");

        var ackMessage = "<|ACK|>";
        var ackMessageBytes = Encoding.UTF8.GetBytes(ackMessage);
        await handler.SendAsync(ackMessageBytes, 0);
        Console.WriteLine(
            $"Socket server sent acknowledgment: \"{ackMessage}\"");
        
        Console.WriteLine();
        
    }
    // Sample output:
    //    Socket server received message: "Hi friends 👋!"
    //    Socket server sent acknowledgment: "<|ACK|>"
}