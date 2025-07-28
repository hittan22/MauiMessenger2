using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace GroupMessenger02.MVVM.Models
{
    public class User : INotifyPropertyChanged
    {
        private string _username;
        private string _uid;

        [JsonPropertyName("username")]
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        [JsonPropertyName("uid")]
        public string Uid
        {
            get => _uid;
            set
            {
                _uid = value;
                OnPropertyChanged();
            }
        }

        public User()
        {
            Username = "";
            Uid = "";
        }

        public User(string username, string uid = null)
        {
            Username = username;
            Uid = uid;
        }

        public string onString()
        {
            return Username;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}