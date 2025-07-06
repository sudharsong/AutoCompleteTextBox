using System;
using System.Buffers.Binary;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace codecrafterskafka.src
{
    public static class Utilities
    {
        public static async Task<byte[]> ReadExactlyAsync(this Socket socket,
                                                         int count,
                                                         CancellationToken token)
        {
            var buffer = new byte[count];
            int offset = 0;

            while (offset < count)
            {
                // ← use the Memory<byte> overload, which takes a CancellationToken
                int n = await socket.ReceiveAsync(
                    buffer.AsMemory(offset, count - offset),
                    SocketFlags.None,
                    token);

                if (n == 0)
                {
                    // peer closed before we got 'count' bytes
                    return Array.Empty<byte>();
                }

                offset += n;
            }

            return buffer;
        }

        public static async Task SendAllAsync(this Socket socket,
                                      ReadOnlyMemory<byte> buffer,
                                      CancellationToken token)
        {
            int sent = 0;
            while (sent < buffer.Length)
            {
                //int n = s.Send(buffer.ToArray()[sent..]);
                int n = await socket.SendAsync(buffer[sent..], SocketFlags.None, token);
                if (n == 0) throw new IOException("Peer closed while sending");
                sent += n;
            }
        }

        public static void WriteToBuffer(this ArrayBufferWriter<byte> writer, short value)
        {
            Span<byte> span = writer.GetSpan(2);
            BinaryPrimitives.WriteInt16BigEndian(span, value);
            writer.Advance(2);
        }

        public static void WriteToBuffer(this ArrayBufferWriter<byte> writer, int value)
        {
            Span<byte> span = writer.GetSpan(4);
            BinaryPrimitives.WriteInt32BigEndian(span, value);
            writer.Advance(4);
        }

        public static void WriteToBuffer(this ArrayBufferWriter<byte> writer, long value)
        {
            Span<byte> span = writer.GetSpan(4);
            BinaryPrimitives.WriteInt64BigEndian(span, value);
            writer.Advance(8);
        }

        public static void WriteToBuffer(this ArrayBufferWriter<byte> writer, byte value)
        {
            Span<byte> span = writer.GetSpan(1);
            span[0] = value;
            writer.Advance(1);
        }

        public static void WriteToBuffer(this ArrayBufferWriter<byte> writer, byte[] value)
        {
            Span<byte> span = writer.GetSpan(value.Length);
            value.CopyTo(span);
            writer.Advance(value.Length);
        }

        public static void WriteVarIntToBuffer(this ArrayBufferWriter<byte> writer, int value)
        {
            uint zig = (uint)((value << 1) ^ (value >> 31));

            // 2) Emit 7 bits per byte, setting the high bit if more follow
            while ((zig & ~0x7Fu) != 0)
            {
                // write (lower7bits | continuation)
                writer.GetSpan(1)[0] = (byte)((zig & 0x7F) | 0x80);
                writer.Advance(1);
                zig >>= 7;
            }

            // last byte (high bit = 0)
            writer.GetSpan(1)[0] = (byte)zig;
            writer.Advance(1);
        }

        public static int ReadInt32FromBuffer(this byte[] buffer, ref int offset)
        {            
            var result = BinaryPrimitives.ReadInt32BigEndian(buffer.AsSpan(offset, 4));
            offset += 4;
            return result;
        }

        public static long ReadInt64FromBuffer(this byte[] buffer, ref int offset)
        {
            var result = BinaryPrimitives.ReadInt64BigEndian(buffer.AsSpan(offset, 8));
            offset += 8;
            return result;
        }

        public static short ReadInt16FromBuffer(this byte[] buffer, ref int offset)
        {
            var result = BinaryPrimitives.ReadInt16BigEndian(buffer.AsSpan(offset, 2));
            offset += 2;
            return result;
        }

        public static Guid ReadGuidFromBuffer(this byte[] buffer, ref int offset)
        {
            //Guid result = new Guid(buffer.AsSpan(offset, 16).ToArray());
            //offset += 16;
            //return result;
            var b = buffer.AsSpan(offset, 16).ToArray();
            Array.Reverse(b, 0, 4);  // Data1
            Array.Reverse(b, 4, 2);  // Data2
            Array.Reverse(b, 6, 2);  // Data3
                                     // Data4 (bytes 8–15) stays in network order
            offset += 16;
            // 3. Construct the Guid from the now-correct little-endian byte array
            return new Guid(b);
        }

        public static string ReadStringFromBuffer(this byte[] buffer, ref int offset, int length)
        { 
            var result = Encoding.UTF8.GetString(buffer.Skip(offset).Take(length).ToArray());
            offset += length;
            return result;
        }

        public static byte ReadByteFromBuffer(this byte[] buffer, ref int offset)
        {
            var result = buffer[offset];
            offset += 1;
            return result;
        }

        public static int ReadVarInt(this byte[] buffer, ref int offset)
        {
            int value = 0;
            int shift = 0;
            bool continuationBit = true;
            for (int i = 0; i < 5 && continuationBit; i++)
            {
                if (offset >= buffer.Length)
                {
                    throw new IndexOutOfRangeException("End of buffer reading varint.");
                }
                continuationBit = (buffer[offset] & 0x80) == 0x80;
                value |= (buffer[offset++] & 0x7f) << shift;
                shift += 7;
            }
            return (value >> 1) ^ -(value & 1);
        }

        internal static uint ReadUVarInt(this byte[] buffer, ref int offset)
        {
            uint value = 0;
            int shift = 0;

            bool continuationBit = true;
            for (int i = 0; i < 5 && continuationBit; i++)
            {
                if (offset >= buffer.Length)
                {
                    throw new IndexOutOfRangeException("End of buffer reading varint.");
                }
                continuationBit = (buffer[offset] & 0x80) == 0x80;
                value |= (uint)(buffer[offset++] & 0x7f) << shift;
                shift += 7;
            }
            return value;
        }

        //public static byte[] GetBytes(this int num)
        //{
        //    var result = BitConverter.GetBytes(num);
        //    if(BitConverter.IsLittleEndian)
        //    {
        //        Array.Reverse(result);
        //    }

        //    return result;
        //}

        //public static byte[] GetBytes(this short num)
        //{
        //    var result = BitConverter.GetBytes(num);
        //    if (BitConverter.IsLittleEndian)
        //    {
        //        Array.Reverse(result);
        //    }

        //    return result;
        //}
    }
}
