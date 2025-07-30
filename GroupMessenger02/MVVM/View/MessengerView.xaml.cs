using System.Collections.Specialized;
using GroupMessenger02.MVVM.ViewModels;

namespace GroupMessenger02.MVVM.View;

public partial class MessengerView : ContentPage
{
    public MessengerView(MessengerViewModel viewmodel)
    {
        InitializeComponent();
        BindingContext = viewmodel;
    }
}
