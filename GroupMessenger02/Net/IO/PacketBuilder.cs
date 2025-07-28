using System.Text;
using System.Text.Json;

namespace GroupMessenger02.Net.IO
{
    public class PacketBuilder
    {
        private readonly MemoryStream _stream;
        private readonly BinaryWriter _writer;

        public PacketBuilder()
        {
            _stream = new MemoryStream();
            _writer = new BinaryWriter(_stream, Encoding.Unicode);
        }

        public void WriteOpCode(byte opcode)
        {
            _writer.Write(opcode);
        }

        public void WriteMessage<T>(T message)
        {
            // Serialize to JSON
            string json = JsonSerializer.Serialize(message, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            byte[] jsonBytes = Encoding.Unicode.GetBytes(json); // Use UTF-16
            _writer.Write(jsonBytes.Length); // Write length prefix
            _writer.Write(jsonBytes); // Write JSON data
        }

        public byte[] GetPacketBytes()
        {
            return _stream.ToArray();
        }
        public void Reset()
        {
            _stream.SetLength(0); // Clear the stream for reuse
            _stream.Position = 0; // Reset position
        }
    }
}