namespace MauiApp3
{
    public partial class MainPage : ContentPage
    {

        public MainPage(MainPageViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = ViewModel;
        }
        ~MainPage()
        {
        }

        MainPageViewModel ViewModel;

        private void Button_Clicked(object sender, EventArgs e)
        {
            //Sample1PageViewModel = null;
            //GC.Collect();
            //GC.Collect();
            //GC.Collect();
            //GC.Collect();

            //App.Current.MainPage = new MainPage(new MainPageViewModel());
            App.Current.MainPage = new Sample1Page(new Sample1PageViewModel());
            //ViewModel.Dispose();
            this.BindingContext = null;
            ViewModel = null;
            //Dispose();
            //MainPageViewModel= null;
            GC.Collect();
            GC.Collect();
            GC.Collect(2);
            GC.WaitForPendingFinalizers();
        }
    }

}
