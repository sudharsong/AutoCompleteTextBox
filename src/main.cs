using System.Buffers.Binary;
using System.Net;
using System.Net.Sockets;

// You can use print statements as follows for debugging, they'll be visible when running tests.
Console.WriteLine("Logs from your program will appear here!");

// Uncomment this block to pass the first stage
CancellationTokenSource tokenSource = new CancellationTokenSource();
TcpListener server = new TcpListener(IPAddress.Any, 9092);
server.Start();
while (true)
{
    var socket = await server.AcceptSocketAsync(tokenSource.Token); // wait for client
   //socket.Listen();
    await HandleRequestAync(socket);
}

async Task HandleRequestAync(Socket socket)
{
    while(!tokenSource.IsCancellationRequested)
    {
        var requestBuffer = new byte[200];
        var request = await socket.ReceiveAsync(requestBuffer);
        byte[] buffer = new byte[8];
        BinaryPrimitives.WriteInt32BigEndian(buffer.AsSpan(0, 4), 0);
        BinaryPrimitives.WriteInt32BigEndian(buffer.AsSpan(4, 4), 7);
        await socket.SendAsync(buffer);
    }
}