using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace GroupMessenger02.MVVM.Models
{
    public class Message : INotifyPropertyChanged
    {
        private string? _id;
        private string _content;
        private User _sender;
        private DateTime? _timestamp;
        private string _chatid;

        [JsonPropertyName("id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Id
        {
            get
            {
                if (_id != null)
                {
                    return _id;
                }
                else
                {
                    return "";
                }
            }
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }

        [JsonPropertyName("content")]
        public string Content
        {
            get => _content;
            set
            {
                _content = value;
                OnPropertyChanged();
            }
        }

        [JsonPropertyName("sender")]
        public User Sender
        {
            get => _sender;
            set
            {
                _sender = value;
                OnPropertyChanged();
            }
        }

        [JsonPropertyName("timestamp")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTime? Timestamp
        {
            get => _timestamp;
            set
            {
                _timestamp = value;
                OnPropertyChanged();
            }
        }

        [JsonPropertyName("chatid")]
        public string ChatId
        {
            get => _chatid;
            set
            {
                _chatid = value;
                OnPropertyChanged();
            }
        }

        public Message()
        {
            Content = "";
            Sender = new User();
        }

        // Constructor for received messages
        public Message(string id, string content, User sender, DateTime timestamp, string chatid)
        {
            Id = id;
            Content = content;
            Sender = sender;
            Timestamp = timestamp;
            ChatId = chatid;
        }

        // Convert to string for PacketBuilder (for sending to server)
        public override string ToString()
        {
            // For sending to server: include sender and content
            // Format: "sender:content" (adjust based on your server's expected format)
            return $"{Sender}: {Content}";
        }

        // INotifyPropertyChanged for MVVM
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}