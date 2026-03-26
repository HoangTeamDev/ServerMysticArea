using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerMysticArea.GameServer
{
    public class Message
    {
        public short Command { get; private set; }
        private DataReader _reader;
        private DataWriter _writer;
        public Message(short command, byte[] data)
        {
            Command = command;
            _reader = new DataReader(data);
        }
        public Message(short command)
        {
            Command = (short)command;
            _writer = new DataWriter(Command);
        }
        public void writeByte(byte value) => _writer.WriteByte(value);
        public void writeInt(int value) => _writer.WriteInt(value);
        public void writeShort(short value) => _writer.WriteShort(value);
        public void WriteFloat(float value) => _writer.WriteFloat(value);
        public void writeLong(long value) => _writer.WriteLong(value);
        public void writeBool(bool value) => _writer.WriteBool(value);

        public void writeUTF(string value) => _writer.WriteUTF(value);
        public byte readByte() => (byte)_reader.ReadByte();
        public string readUTF() => _reader.ReadUTF();
        public int readInt() => _reader.ReadInt();
        public short readShort() => _reader.ReadShort();
        public float readFloat() => _reader.ReadFloat();
        public long readLong() => _reader.ReadLong();
        public bool readBool() => _reader.ReadBool();

        public byte[] ToArray()
        {
            return _writer?.ToArray();
        }

        public void cleanup()
        {
            //_writer.Close();
        }
    }
}
