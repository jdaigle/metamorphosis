using System;

namespace Metamorphosis
{
    /// <summary>
    /// Reads primitive values from a buffer or writes primitive values to a buffer in networking order.
    /// </summary>
    public static class KafkaBitConverter
    {
        /// <summary>
        /// Reads a signed byte and advance the buffer read cursor.
        /// </summary>
        /// <param name="buffer">The buffer to read from.</param>
        /// <returns></returns>
        public static sbyte ReadByte(ByteBuffer buffer)
        {
            buffer.ValidateRead(FixedWidth.Byte);
            sbyte data = (sbyte)buffer.Buffer[buffer.ReadOffset];
            buffer.CompleteRead(FixedWidth.Byte);
            return data;
        }

        /// <summary>
        /// Reads a 16-bit signed integer and advance the buffer read cursor.
        /// </summary>
        /// <param name="buffer">The buffer to read from.</param>
        /// <returns></returns>
        public static short ReadShort(ByteBuffer buffer)
        {
            buffer.ValidateRead(FixedWidth.Short);
            short data = (short)((buffer.Buffer[buffer.ReadOffset] << 8) | buffer.Buffer[buffer.ReadOffset + 1]);
            buffer.CompleteRead(FixedWidth.Short);
            return data;
        }

        /// <summary>
        /// Reads a 32-bit signed integer and advance the buffer read cursor.
        /// </summary>
        /// <param name="buffer">The buffer to read from.</param>
        /// <returns></returns>
        public static int ReadInt(ByteBuffer buffer)
        {
            buffer.ValidateRead(FixedWidth.Int);
            int data = ReadInt(buffer.Buffer, buffer.ReadOffset);
            buffer.CompleteRead(FixedWidth.Int);
            return data;
        }

        /// <summary>
        /// Reads a 32-bit signed integer from the buffer at the specified offset.
        /// </summary>
        /// <param name="buffer">The buffer to read from.</param>
        /// <param name="offset">The offset in the buffer to start reading.</param>
        /// <returns></returns>
        public static int ReadInt(byte[] buffer, int offset)
        {
            return (buffer[offset] << 24) | (buffer[offset + 1] << 16) | (buffer[offset + 2] << 8) | buffer[offset + 3];
        }

        /// <summary>
        /// Reads a 64-bit signed integer from the buffer and advance the buffer read cursor.
        /// </summary>
        /// <param name="buffer">The buffer to read from.</param>
        /// <returns></returns>
        public static long ReadLong(ByteBuffer buffer)
        {
            buffer.ValidateRead(FixedWidth.Long);
            long high = ReadInt(buffer.Buffer, buffer.ReadOffset);
            long low = (uint)ReadInt(buffer.Buffer, buffer.ReadOffset + 4);
            long data = (high << 32) | low;
            buffer.CompleteRead(FixedWidth.Long);
            return data;
        }

        /// <summary>
        /// Writes a signed byte into the buffer and advance the buffer write cursor.
        /// </summary>
        /// <param name="buffer">The buffer to write.</param>
        /// <param name="data">The data to write.</param>
        public static void WriteByte(ByteBuffer buffer, sbyte data)
        {
            buffer.ValidateWrite(FixedWidth.Byte);
            buffer.Buffer[buffer.WriteOffset] = (byte)data;
            buffer.AppendWrite(FixedWidth.Byte);
        }

        /// <summary>
        /// Writes a 16-bit signed integer into the buffer and advance the buffer write cursor.
        /// </summary>
        /// <param name="buffer">The buffer to write.</param>
        /// <param name="data">The data to write.</param>
        public static void WriteShort(ByteBuffer buffer, short data)
        {
            buffer.ValidateWrite(FixedWidth.Short);
            buffer.Buffer[buffer.WriteOffset] = (byte)(data >> 8);
            buffer.Buffer[buffer.WriteOffset + 1] = (byte)data;
            buffer.AppendWrite(FixedWidth.Short);
        }

        /// <summary>
        /// Writes a 32-bit signed integer into the buffer and advance the buffer write cursor.
        /// </summary>
        /// <param name="buffer">The buffer to write.</param>
        /// <param name="data">The data to write.</param>
        public static void WriteInt(ByteBuffer buffer, int data)
        {
            buffer.ValidateWrite(FixedWidth.Int);
            WriteInt(buffer.Buffer, buffer.WriteOffset, data);
            buffer.AppendWrite(FixedWidth.Int);
        }

        /// <summary>
        /// Writes a 32-bit unsigned integer into the buffer at specified offset.
        /// </summary>
        /// <param name="buffer">The buffer to write.</param>
        /// <param name="offset">The offset of the buffer.</param>
        /// <param name="data">The data to write.</param>
        public static void WriteInt(byte[] buffer, int offset, int data)
        {
            buffer[offset] = (byte)(data >> 24);
            buffer[offset + 1] = (byte)(data >> 16);
            buffer[offset + 2] = (byte)(data >> 8);
            buffer[offset + 3] = (byte)data;
        }

        /// <summary>
        /// Writes a 64-bit signed integer into the buffer and advance the buffer write cursor.
        /// </summary>
        /// <param name="buffer">The buffer to write.</param>
        /// <param name="data">The data to write.</param>
        public static void WriteLong(ByteBuffer buffer, long data)
        {
            WriteInt(buffer, (int)(data >> 32));
            WriteInt(buffer, (int)data);
        }

        /// <summary>
        /// Reads bytes from one buffer into another.
        /// </summary>
        /// <param name="buffer">Source buffer.</param>
        /// <param name="data">Destination buffer</param>
        /// <param name="offset">The start position to write.</param>
        /// <param name="count">The number of bytes to read.</param>
        public static void ReadBytes(ByteBuffer buffer, byte[] data, int offset, int count)
        {
            // TODO: does this really belong here?
            buffer.ValidateRead(count);
            Buffer.BlockCopy(buffer.Buffer, buffer.ReadOffset, data, offset, count);
            buffer.CompleteRead(count);
        }

        /// <summary>
        /// Writes bytes from one buffer into another.
        /// </summary>
        /// <param name="buffer">The destination buffer.</param>
        /// <param name="data">The source buffer</param>
        /// <param name="offset">The position in source buffer to start.</param>
        /// <param name="count">The number of bytes to write.</param>
        public static void WriteBytes(ByteBuffer buffer, byte[] data, int offset, int count)
        {
            // TODO: does this really belong here?
            buffer.ValidateWrite(count);
            Buffer.BlockCopy(data, offset, buffer.Buffer, buffer.WriteOffset, count);
            buffer.AppendWrite(count);
        }

        public static class FixedWidth
        {
            public const int Byte = 1;
            public const int Short = 2;
            public const int Int = 4;
            public const int Long = 8;
        }
    }
}
