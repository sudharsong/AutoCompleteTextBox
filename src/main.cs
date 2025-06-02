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
    Task.Run(() =>  HandleRequestAync(socket, tokenSource.Token));
}


static async Task HandleRequestAync(Socket socket, CancellationToken token)
{
   // while (!token.IsCancellationRequested)
    //{
        Console.WriteLine("Parsing the request");
        var requestBuffer = new byte[1024];
        var readBytes = await socket.ReceiveAsync(requestBuffer);
        if (readBytes == 0)
        {
            Console.WriteLine("Peer Closed");
            throw new InvalidOperationException("Peer Closed");
        }

        var requesLength = BinaryPrimitives.ReadInt32BigEndian(requestBuffer.AsSpan(0, 4));
        var apiKey = BinaryPrimitives.ReadInt16BigEndian(requestBuffer.AsSpan(4, 2));
        var apiVersion = BinaryPrimitives.ReadInt16BigEndian(requestBuffer.AsSpan(6, 2));
        var correlationId = BinaryPrimitives.ReadInt32BigEndian(requestBuffer.AsSpan(8, 4));
        byte[] buffer = null;
        if (apiKey == 18 && apiVersion >= 0 && apiVersion <= 4)
        {
            buffer = new byte[23];
            BinaryPrimitives.WriteInt32BigEndian(buffer.AsSpan(0, 4), 19);
            BinaryPrimitives.WriteInt32BigEndian(buffer.AsSpan(4, 4), correlationId);
            BinaryPrimitives.WriteInt16BigEndian(buffer.AsSpan(8, 2), 0);
            buffer[10] = 2;
            BinaryPrimitives.WriteInt16BigEndian(buffer.AsSpan(11, 2), 18);
            BinaryPrimitives.WriteInt16BigEndian(buffer.AsSpan(13, 2), 0);
            BinaryPrimitives.WriteInt16BigEndian(buffer.AsSpan(15, 2), 4);
            buffer[17] = 0;
            BinaryPrimitives.WriteInt32BigEndian(buffer.AsSpan(18, 4), 120);
            buffer[22] = 0;
        }
        else
        {
            buffer = new byte[10];
            BinaryPrimitives.WriteInt32BigEndian(buffer.AsSpan(0, 4), 6);
            BinaryPrimitives.WriteInt32BigEndian(buffer.AsSpan(4, 4), correlationId);
            BinaryPrimitives.WriteInt16BigEndian(buffer.AsSpan(8, 2), 35);

        }
     
        await socket.SendAsync(buffer);

    if(socket.Connected && !token.IsCancellationRequested)
    {
        Task.Run(() => HandleRequestAync(socket, token));
    }
   // }
}