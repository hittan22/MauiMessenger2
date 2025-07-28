using System.IO;
using System.Text;
using System.Text.Json;
using GroupMessenger02.MVVM.Models;

namespace GroupMessenger02.Net.IO
{
    public class PacketReader
    {
        private readonly BinaryReader _reader;

        public PacketReader(Stream stream)
        {
            _reader = new BinaryReader(stream, Encoding.Unicode); // UTF-16
        }

        public byte ReadOpCode()
        {
            try
            {
                return _reader.ReadByte();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading opcode: {ex.Message}");
                throw; // Rethrow to let outer code handle
            }
        }

        public T? ReadMessage<T>()
        {
            try
            {
                int messageLength = _reader.ReadInt32();
                string json = Encoding.Unicode.GetString(_reader.ReadBytes(messageLength));

                var message = JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                if (message == null)
                {
                    throw new InvalidDataException("Received message is empty.");
                }

                return message;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading message: {ex.Message}");
                return default;
            }
        }
    }
}