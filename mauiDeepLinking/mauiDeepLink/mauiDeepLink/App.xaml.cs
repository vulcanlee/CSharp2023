using CommunityToolkit.Mvvm.Messaging;
using mauiDeepLink.Messages;

namespace mauiDeepLink
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }
    }
}
