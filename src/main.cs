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
    var socket = await  server.AcceptSocketAsync(tokenSource.Token); // wait for client
    HandleRequestAync(socket);
}

void HandleRequestAync(Socket socket)
{
    while(!tokenSource.IsCancellationRequested)
    {

    }
}