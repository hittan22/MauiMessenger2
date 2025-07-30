using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace GroupMessenger02.MVVM.Models
{
    public class Chat : INotifyPropertyChanged
    {
        private string? _id;
        private List<Message>? _messages;
        private List<User> _users;
        private string _name;

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

        [JsonPropertyName("messages")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<Message> Messages
        {
            get => _messages;
            set
            {
                _messages = value;
                OnPropertyChanged();
            }
        }

        [JsonPropertyName("users")]
        public List<User> Users
        {
            get => _users;
            set
            {
                _users = value;
                OnPropertyChanged();
            }
        }

        [JsonPropertyName("name")]
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }
        // Constructor for creating chats (before being sent to server for validation)
        public Chat()
        {
        }

        // Constructor for existing chats (when user added)
        public Chat(string id, List<Message> messages, List<User> users, string name)
        {
            Id = id;
            Messages = messages;
            Users = users;
            Name = name;
        }

        // INotifyPropertyChanged for MVVM
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}