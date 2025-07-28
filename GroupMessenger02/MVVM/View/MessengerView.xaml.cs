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

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();

        if (BindingContext is MessengerViewModel vm)
        {
            // First, unsubscribe if already subscribed (avoid duplicates)
            vm.Messages.CollectionChanged -= Messages_CollectionChanged;

            // Then re-subscribe
            vm.Messages.CollectionChanged += Messages_CollectionChanged;
        }
    }

    private void Messages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        // Scroll to the last message after layout update
        if (mvm.Messages.Count > 0)
        {
            Task.Delay(1000).ContinueWith(_ =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    MessagesView.ScrollTo(mvm.Messages.Count - 1, position: ScrollToPosition.End, animate: true);
                });
            });
        }
    }
}
