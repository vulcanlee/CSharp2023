using CommunityToolkit.Mvvm.Messaging;
using mauiDeepLink.Messages;

namespace mauiDeepLink
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();

            WeakReferenceMessenger.Default.Register<NotificationItemMessage>(this, (r, m) =>
            {
                string gettoken = m.Value.ToString();
                //Device.StartTimer(TimeSpan.FromSeconds(2), () =>
                //{
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    label.Text = gettoken;
                    //await AppShell.Current.DisplayAlert("Alert", gettoken, "ok");
                });
                //    return false;
                //});
            });


        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);

            int value2 = 100;
            string uriBack = "dplcalculators://calculator.com/sum?value=" + value2.ToString();
            Launcher.Default.OpenAsync(uriBack);

        }
    }

}
