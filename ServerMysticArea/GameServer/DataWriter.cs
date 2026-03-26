using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerMysticArea.GameServer
{
    public class DataWriter
    {
        private MemoryStream stream;
        private BinaryWriter writer;

        public DataWriter(int capacity = 256)
        {
            stream = new MemoryStream(capacity);
            writer = new BinaryWriter(stream, Encoding.UTF8);
        }
        public void WriteUTF(string value) => WriteString(value);
        public void WriteInt(int value) => writer.Write(value);
        public void WriteFloat(float value) => writer.Write(value);
        public void WriteBool(bool value) => writer.Write(value);
        public void WriteLong(long value) => writer.Write(value);
        public void WriteByte(byte value) => writer.Write(value);
        public void WriteShort(short value) => writer.Write(value);
        public void WriteUShort(ushort value) => writer.Write(value);

        // Custom string format: ushort length + UTF8 bytes
        public void WriteString(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                writer.Write((ushort)0);
                return;
            }

            byte[] bytes = Encoding.UTF8.GetBytes(value);
            writer.Write((ushort)bytes.Length);
            writer.Write(bytes);
        }

        public byte[] ToArray()
        {
            return stream.ToArray();
        }

        public void Reset()
        {
            stream.Position = 0;
            stream.SetLength(0);
        }

        public int Length => (int)stream.Length;
    }
}
