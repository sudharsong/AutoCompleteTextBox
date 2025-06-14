using codecrafterskafka.src.Design;
using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
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
            try
            {

                var lenthBuffer = await socket.ReadExactlyAsync(4, token);
                var requesLength = BinaryPrimitives.ReadInt32BigEndian(lenthBuffer);
                if (requesLength == 0)
                {
                    throw new InvalidOperationException("Invalid Messge Length");
                }


                byte[] inputBuffer = await socket.ReadExactlyAsync(requesLength, token);

                Request request = new Request(inputBuffer);

                ArrayBufferWriter<byte> writer = new ArrayBufferWriter<byte>();

                if (request.Head.ApiKey == 18 && request.Head.ApiVersion >= 0 && request.Head.ApiVersion <= 4)
                {
                    PrepareValidApiKeyResponse(writer, request.Head.CorrelationId);
                }
                else if (request.Head.ApiKey == 75 && request.Head.ApiVersion >= 0)
                {
                    var topicName = ((TopicPartitionRequestBodyV0)request.Body).Topics.FirstOrDefault()?.Name ?? string.Empty;
                    PrepareDescribeTopicPartitionsResponse(writer,
                        request.Head.CorrelationId,
                        topicName);
                }
                else
                {
                    PrepareInValidApiKeyResponse(writer, request.Head.CorrelationId);
                }

                Console.WriteLine(writer.WrittenMemory);
                await socket.SendAllAsync(writer.WrittenMemory, token);

                if (socket.Connected && !token.IsCancellationRequested)
                {
                    Task.Run(() => HandleRequestAync(socket, token));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void PrepareDescribeTopicPartitionsResponse(ArrayBufferWriter<byte> writer, int correlationId, string topicName)
        {
            TopicParitionResponseHeader header = new TopicParitionResponseHeader(correlationId);
            TopicParitionResponseBody body = new TopicParitionResponseBody();
            ResponseTopic topic = new ResponseTopic();
            topic.Content = topicName;
            topic.ErrorCode = 3;
            topic.UUID = new Guid("00000000-0000-0000-0000-000000000000");
            topic.PartitionsCount = 1;
            body.Topics = new ResponseTopic[] { topic };

            TopicPartitionResponse partitionResponse = new TopicPartitionResponse(header, body);
            partitionResponse.GetResponse(writer);
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
