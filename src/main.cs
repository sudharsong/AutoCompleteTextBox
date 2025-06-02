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
        var requestBuffer = new byte[13];
        var readBytes = await socket.ReceiveAsync(requestBuffer);
        if(readBytes == 0)
        {
            Console.WriteLine("Peer Closed");
            throw new InvalidOperationException("Peer Closed");
        }

        var requesLength = BinaryPrimitives.ReadInt32BigEndian(requestBuffer.AsSpan(0, 4));
        var apiKey = BinaryPrimitives.ReadInt16BigEndian(requestBuffer.AsSpan(4, 2));
        var apiVersion = BinaryPrimitives.ReadInt16BigEndian(requestBuffer.AsSpan(6, 2));
        var correlationId = BinaryPrimitives.ReadInt32BigEndian(requestBuffer.AsSpan(8, 4));
        byte[] buffer = new byte[8];
        BinaryPrimitives.WriteInt32BigEndian(buffer.AsSpan(0, 4), 0);
        BinaryPrimitives.WriteInt32BigEndian(buffer.AsSpan(4, 4), correlationId);
        await socket.SendAsync(buffer);
    }
}