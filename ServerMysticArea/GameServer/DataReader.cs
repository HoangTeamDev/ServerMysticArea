using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerMysticArea.GameServer
{
    public class DataReader
    {
        private MemoryStream stream;
        private BinaryReader reader;

        public DataReader(byte[] data)
        {
            stream = new MemoryStream(data);
            reader = new BinaryReader(stream, Encoding.UTF8);
        }

        public int ReadInt() => reader.ReadInt32();
        public float ReadFloat() => reader.ReadSingle();
        public bool ReadBool() => reader.ReadBoolean();
        public long ReadLong() => reader.ReadInt64();
        public byte ReadByte() => reader.ReadByte();
        public short ReadShort() => reader.ReadInt16();
        public ushort ReadUShort() => reader.ReadUInt16();

        // Custom string format: ushort length + UTF8 bytes
        public string ReadUTF()
        {
            ushort length = reader.ReadUInt16();
            if (length == 0)
                return "";

            byte[] bytes = reader.ReadBytes(length);
            return Encoding.UTF8.GetString(bytes);
        }

        public bool HasRemaining()
        {
            return stream.Position < stream.Length;
        }

        public long Position => stream.Position;
        public long Length => stream.Length;
    }
}
