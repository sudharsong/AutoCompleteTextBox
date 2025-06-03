using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace codecrafterskafka.src
{
    internal class KafkaServer
    {
        private int port;
        private CancellationToken token;

        public KafkaServer(int port, CancellationToken token)
        {
            this.port = port;
            this.token = token;
        }

        public async Task Start()
        {
            TcpListener server = new TcpListener(IPAddress.Loopback, port);
            server.Start();

            while(true)
            {
                var socket = await server.AcceptSocketAsync(this.token);
                Task.Run(() =>
                {
                    HandleRequestAync(socket,
                                             token);
                });
            }
        }

        private async Task HandleRequestAync(Socket socket, CancellationToken token)
        {
            var lenthBuffer = await socket.ReadExactlyAsync(4, token);    
            var requesLength = BinaryPrimitives.ReadInt32BigEndian(lenthBuffer);
            if(requesLength == 0)
            {
                throw new InvalidOperationException("Invalid Messge Length");
            }

            byte[] inputBuffer = await socket.ReadExactlyAsync(requesLength, token);    
            var apiKey = BinaryPrimitives.ReadInt16BigEndian(inputBuffer.AsSpan(0, 2));
            var apiVersion = BinaryPrimitives.ReadInt16BigEndian(inputBuffer.AsSpan(2, 2));
            var correlationId = BinaryPrimitives.ReadInt32BigEndian(inputBuffer.AsSpan(4, 4));
            ArrayBufferWriter<byte> writer = new ArrayBufferWriter<byte>(); 
            if (apiKey == 18 && apiVersion >= 0 && apiVersion <= 4)
            {
                PrepareValidApiKeyResponse(writer, correlationId);  
            }
            else
            {
                PrepareInValidApiKeyResponse(writer, correlationId);    
            }

            await socket.SendAllAsync(writer.WrittenMemory, token);

            if (socket.Connected && !token.IsCancellationRequested)
            {
                Task.Run(() => HandleRequestAync(socket, token));
            }
        }

        private void PrepareValidApiKeyResponse(ArrayBufferWriter<byte> writer, int correlationId)
        {
            var lengthSpan = writer.GetSpan(4);
            writer.Advance(4); // reserve space for length  


            writer.WriteInt32ToBuffer(correlationId); //correlationId            
            writer.WriteInt16ToBuffer(0); //ErrorCode
            writer.WriteByteToBuffer(3); //Api key version array length

            writer.WriteInt16ToBuffer(18); //Api key
            writer.WriteInt16ToBuffer(0); //Api key min version 
            writer.WriteInt16ToBuffer(4); //Api key max version
            writer.WriteByteToBuffer(0); //Tag field 

            writer.WriteInt16ToBuffer(75); //api valid key
            writer.WriteInt16ToBuffer(0); //Api key min version
            writer.WriteInt16ToBuffer(0); //Api key max version
            writer.WriteByteToBuffer(0); //Tag field

            writer.WriteInt32ToBuffer(120); //Throttle time
            writer.WriteByteToBuffer(0); //Tag field

            var length = writer.WrittenCount - 4; // calculate the length of the response   
            BinaryPrimitives.WriteInt32BigEndian(lengthSpan, length); // write the length to the reserved space 
        }

        private void PrepareInValidApiKeyResponse(ArrayBufferWriter<byte> writer, int correlationId)
        {
            var lengthSpan = writer.GetSpan(4);
            writer.Advance(4); // reserve space for length  

            writer.WriteInt32ToBuffer(correlationId); // Correlation ID
            writer.WriteInt16ToBuffer(35); // Error code for invalid API key    

            var length = writer.WrittenCount - 4; // calculate the length of the response   
            BinaryPrimitives.WriteInt32BigEndian(lengthSpan, length); // write the length to the reserved space 
        }
    }
}
