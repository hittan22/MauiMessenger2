using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using GroupMessenger02.MVVM.Models;
using GroupMessenger02.MVVM.View;
using GroupMessenger02.Net;

namespace GroupMessenger02.MVVM.ViewModels
{
    public class MessengerViewModel : INotifyPropertyChanged
    {
        private readonly Server _server;
        private User _currentUser;
        private Chat _currentChat;
        private Message _newMessage;
        private ObservableCollection<User> _users;
        private ObservableCollection<Chat> _chats;
        public ICommand ConnectCommand { get; }
        public ICommand SendMessageCommand { get; }

        public MessengerViewModel()
        {
            _server = new Server();
            _currentUser = new User();
            _currentUser.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(User.Username))
                    {
                        ((Command)SendMessageCommand).ChangeCanExecute();
                        ((Command)ConnectCommand).ChangeCanExecute();
                    }
                };
            _newMessage = new Message();
            _newMessage.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(Message.Content))
                    {
                        ((Command)SendMessageCommand).ChangeCanExecute();
                    }
                };
            _currentChat = new Chat();
            _users = new ObservableCollection<User>();
            _chats = new ObservableCollection<Chat>();

            ConnectCommand = new Command(
                execute: async () => await ConnectAsync(),
                canExecute: () => !string.IsNullOrEmpty(CurrentUser.Username));
            SendMessageCommand = new Command(
                execute: async () => await SendMessageAsync(),
                canExecute: () => !string.IsNullOrEmpty(NewMessage.Content) && !string.IsNullOrEmpty(CurrentUser.Username));

            _server.ConnectedEvent += UserConnected;
            _server.MsgReceivedEvent += MessageReceived;
            _server.UserDisconnectedEvent += RemoveUser;
            _server.ConnectionSuccessful += LoadChat;
        }

        public User CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnPropertyChanged();
                ((Command)ConnectCommand).ChangeCanExecute();
            }
        }
        public Chat CurrentChat
        {
            get => _currentChat;
            set
            {
                _currentChat = value;
                OnPropertyChanged();
            }
        }

        public Message NewMessage
        {
            get => _newMessage;
            set
            {
                _newMessage = value;
                OnPropertyChanged();
                ((Command)SendMessageCommand).ChangeCanExecute();
            }
        }

        public ObservableCollection<User> Users
        {
            get => _users;
            set
            {
                _users = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Chat> Chats
        {
            get => _chats;
            set
            {
                _chats = value;
                OnPropertyChanged();
            }
        }

        private async Task ConnectAsync()
        {
            try
            {
                _server.ConnectToServer(CurrentUser);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting: {ex.Message}");
                // TODO: Show error in UI
            }
        }


        private void LoadChat(User currUser, Chat main)
        {
            CurrentUser = currUser;
            NewMessage.Sender = CurrentUser;
            CurrentChat = main;
            Chats.Add(main);
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Application.Current.MainPage.Navigation.PushAsync(new MessengerView(this));
            });
        }

        private async Task SendMessageAsync()
        {
            try
            {
                NewMessage.ChatId = CurrentChat.Id;
                _server.SendMessageToServer(NewMessage);
                NewMessage.Sender = CurrentUser;
                NewMessage.Content = "";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message: {ex.Message}");
            }
        }

        private void UserConnected()
        {
            var user = _server.packetReader.ReadMessage<User>();

            MainThread.BeginInvokeOnMainThread(() => Users.Add(user));
        }

        private void MessageReceived()
        {
            Message msg = _server.packetReader.ReadMessage<Message>();
            MainThread.BeginInvokeOnMainThread(() => _chats.Where(x => x.Id == msg.ChatId).First().Messages.Add(msg));
        }

        private void RemoveUser(User user)
        {
            var userToRemove = Users.FirstOrDefault(x => x.Uid == user.Uid);
            if (userToRemove != null)
            {
                MainThread.BeginInvokeOnMainThread(() => Users.Remove(userToRemove));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}