using System.Collections.Specialized;
using GroupMessenger02.MVVM.ViewModels;

namespace GroupMessenger02.MVVM.View;

public partial class MessengerView : ContentPage
{
    private double _lastVisibleY = 0;
    private bool _isAtBottom = true;
    public MessengerViewModel mvm { get; set; }
    public MessengerView(MessengerViewModel viewmodel)
    {
        InitializeComponent();
        mvm = viewmodel;
        BindingContext = mvm;
    }
}
