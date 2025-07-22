using codecrafterskafka.src.MetaData;
using src.Design.Base;
using src.Design.Fetch;
using src.Design.TopicPartition;
using src.MetaDatakafka.src;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

namespace codecrafterskafka.src
{
    internal class KafkaServer
    {
        private int port;
        private CancellationToken token;
        private LogMetaData? metaData;

        public KafkaServer(int port, CancellationToken token)
        {
            this.port = port;
            this.token = token;
        }

        public async Task LoadLog()
        {
            ///tmp/kraft-combined-logs/__cluster_metadata-0/00000000000000000000.log
            var executionPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (executionPath == null)
            {
                throw new InvalidOperationException("Execution path cannot be null");
            }

            //var logDirectory = Path.Combine(executionPath, "tmp", "kraft-combined-logs", "__cluster_metadata-0");
            var workingDirectory = Environment.CurrentDirectory;
            var logDirectory = System.IO.Path.Combine(workingDirectory,
                "/tmp/kraft-combined-logs/__cluster_metadata-0");
            if (!Directory.Exists(logDirectory))
            {
                Console.WriteLine($"Directory not exists {logDirectory}.");
                return;
            }
            
            var logFile = Path.Combine(logDirectory, "00000000000000000000.log");
            Console.WriteLine($"Found log file {logFile}.");
            if (File.Exists(logFile))
            {
                this.metaData = LogMetaData.Instance;
                await metaData.LoadLogMetaDataAsync(logFile, this.token);
            }
            else
            {
                Console.WriteLine("Log file does not exist.");
            }
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
                Console.WriteLine("Started Processing a Request"); 
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
                    var topics = ((TopicPartitionRequestBodyV0)request.Body).Topics;
                    var topicPartions = this.metaData.GetTopicPartitions(topics);
                    PrepareDescribeTopicPartitionsResponse(writer,
                                                               request.Head.CorrelationId,
                                                               topicPartions);
                }
                else if (request.Head.ApiKey == 1 && request.Head.ApiVersion <=16)
                {                    
                    Guid topicId = (request.Body as FetchRequestBodyV16).Topics != null && (request.Body as FetchRequestBodyV16).Topics.Count > 0 ?
                        (request.Body as FetchRequestBodyV16).Topics.FirstOrDefault().TopicID : Guid.Empty;
                    PrepareFetchResponse(writer, request.Head.CorrelationId, topicId);
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

        private void PrepareFetchResponse(ArrayBufferWriter<byte> writer, int correlationId, Guid topicID)
        {
            Console.WriteLine("Correlationid : " + correlationId);
            Console.WriteLine($"Preparing fetch response for topic id {topicID}"); 
            Console.WriteLine($"Topic Exist {this.metaData.GetTopicRecord(topicID)}");
            Console.WriteLine($"Partition Exist {this.metaData.IsTopicPartitionExist(topicID)}");   
            FetchResponseHeaderV16 headerV16 = new FetchResponseHeaderV16(correlationId);
            var partitions = new List<FetchResponseTopicPartition> {
                            new FetchResponseTopicPartition
                            {
                                PartitionIndex = 0,
                                ErrorCode = (short)(this.metaData.GetTopicRecord(topicID) == null ? 100 :
                                !this.metaData.IsTopicPartitionExist(topicID) ? 0: 0),
                                //HighWatermark = 1,
                                //LastStableOffset = 1,
                                //LogStartOffset = 0,
                                //AbortedTransactions = Array.Empty<AbortedTransaction>(),
                                Records = this.metaData.IsTopicPartitionExist(topicID) ? 
                                this.metaData.GetPartitionRecordBatches(topicID) :  new List<RecordBatch>(),
                                //Records = new List<RecordBatch>{new RecordBatch()}
                            } };
            var responses = new List<FetchResponseTopic> {
                    new FetchResponseTopic {
                        TopicID = topicID,
                        Partitions = partitions
                    } };

            Console.WriteLine($"Number of responses {responses.Count} with {partitions.Count} partitions");
          
            FetchResponseBodyV16 bodyV16 = new FetchResponseBodyV16()
            {
                ThrottleTime = 0,
                ErrorCode = 0,
                SessionId = 0,
                Responses = topicID != Guid.Empty ?  responses : new List<FetchResponseTopic>()
            };

            FetchResponseV16 fetchResponse = new FetchResponseV16(headerV16, bodyV16);
            fetchResponse.GetResponse(writer);
        }

        private void PrepareDescribeTopicPartitionsResponse(ArrayBufferWriter<byte> writer, 
            int correlationId, List<TopicPartitions> topicPartitions)
        {
            TopicParitionResponseHeaderV0 header = new TopicParitionResponseHeaderV0(correlationId);
            TopicParitionResponseBodyV0 body = new TopicParitionResponseBodyV0();
            ResponseTopic[] topics = new ResponseTopic[topicPartitions.Count];
            for (int i = 0; i < topicPartitions.Count; i++)
            {
                ResponseTopic topic = new ResponseTopic();
                topic.Content = topicPartitions[i].TopicName;
                topic.ErrorCode = (short)(topicPartitions[i].TopicID.Equals(Guid.Empty) ? 3 : 0);
                topic.UUID = topicPartitions[i].TopicID;
                topic.PartitionsCount = (byte)topicPartitions[i].PartitionCount;
                topic.Partitions = topicPartitions[i].Partitions;
                topics[i] = topic;
            }

            body.Topics = topics;
            TopicPartitionResponse partitionResponse = new TopicPartitionResponse(header, body);
            partitionResponse.GetResponse(writer);
        }

        private void PrepareValidApiKeyResponse(ArrayBufferWriter<byte> writer, int correlationId)
        {
            var lengthSpan = writer.GetSpan(4);
            writer.Advance(4); // reserve space for length  

            writer.WriteToBuffer(correlationId); //correlationId            
            writer.WriteToBuffer((short)0); //ErrorCode
            writer.WriteToBuffer((byte)4); //Api key version array length

            writer.WriteToBuffer((short)18); //Api key
            writer.WriteToBuffer((short)0); //Api key min version 
            writer.WriteToBuffer((short)4); //Api key max version
            writer.WriteToBuffer((byte)0); //Tag field 

            writer.WriteToBuffer((short)75); //api valid key
            writer.WriteToBuffer((short)0); //Api key min version
            writer.WriteToBuffer((short)0); //Api key max version
            writer.WriteToBuffer((byte)0); //Tag field

            writer.WriteToBuffer((short)1); //Api key
            writer.WriteToBuffer((short)0); //Api key min version 
            writer.WriteToBuffer((short)16); //Api key max version
            writer.WriteToBuffer((byte)0); //Tag field 

            writer.WriteToBuffer((int)120); //Throttle time
            writer.WriteToBuffer((byte)0); //Tag field

            var length = writer.WrittenCount - 4; // calculate the length of the response   
            BinaryPrimitives.WriteInt32BigEndian(lengthSpan, length); // write the length to the reserved space 
        }

        private void PrepareInValidApiKeyResponse(ArrayBufferWriter<byte> writer, int correlationId)
        {
            var lengthSpan = writer.GetSpan(4);
            writer.Advance(4); // reserve space for length  

            writer.WriteToBuffer(correlationId); // Correlation ID
            writer.WriteToBuffer((short)35); // Error code for invalid API key    

            var length = writer.WrittenCount - 4; // calculate the length of the response   
            BinaryPrimitives.WriteInt32BigEndian(lengthSpan, length); // write the length to the reserved space 
        }
    }
}
