using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using GroupMessenger02.MVVM.Models;
using GroupMessenger02.Net.IO;

namespace GroupMessenger02.Net
{
    public class Server
    {
        private readonly TcpClient _client;
        public PacketReader packetReader;

        public event Action ConnectedEvent;
        public event Action MsgReceivedEvent;
        public event Action<User> UserDisconnectedEvent;
        public event Action<User, Chat> ConnectionSuccessful;
        public event Action<Chat> ChatRefreshEvent;

        public Server()
        {
            _client = new TcpClient();
        }

        public void ConnectToServer(User user)
        {
            if (!_client.Connected)
            {
                try
                {
                    Console.WriteLine("Connecting...");
                    _client.Connect(IPAddress.Parse("127.0.0.1"), 7262);
                    packetReader = new PacketReader(_client.GetStream());

                    if (!string.IsNullOrEmpty(user.Username))
                    {
                        var connectPacket = new PacketBuilder();
                        connectPacket.WriteOpCode(0);
                        connectPacket.WriteMessage(user); // Send User as JSON
                        _client.Client.Send(connectPacket.GetPacketBytes());
                    }

                    ReadPackets();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Connection error: {ex.Message}");
                }
            }
        }

        private void ReadPackets()
        {
            Task.Run(() =>
            {
                try
                {
                    while (_client.Connected)
                    {
                        var opcode = packetReader.ReadOpCode();
                        switch (opcode)
                        {
                            case 1:
                                ConnectedEvent?.Invoke();
                                break;
                            case 5:
                                MsgReceivedEvent?.Invoke();
                                break;
                            case 8:
                                var userFromServer = packetReader.ReadMessage<User>();
                                var mainChat = packetReader.ReadMessage<Chat>();
                                ConnectionSuccessful?.Invoke(userFromServer, mainChat);
                                break;
                            case 10: // User disconnection
                                var userDisconnected = packetReader.ReadMessage<User>();
                                if (userDisconnected != null)
                                {
                                    UserDisconnectedEvent?.Invoke(userDisconnected);
                                }
                                break;
                            default:
                                Console.WriteLine($"Unknown opcode: {opcode}");
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error reading packets: {ex.Message}");
                }
            });
        }

        public void SendMessageToServer(Message message)
        {
            try
            {
                var messagePacker = new PacketBuilder();
                messagePacker.WriteOpCode(5);
                messagePacker.WriteMessage(message);
                _client.Client.Send(messagePacker.GetPacketBytes());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message: {ex.Message}");
            }
        }
    }
}