namespace MauiApp3;

public partial class Sample1Page : ContentPage
{
    ~Sample1Page()
    {
    }
    public Sample1Page(Sample1PageViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        App.Current.MainPage = new Sample2Page(new Sample2PageViewModel());
        //App.Current.MainPage = new MainPage(new MainPageViewModel());

        //this.BindingContext = null;

        GC.Collect();
        GC.Collect();
        GC.Collect(2);
        GC.WaitForPendingFinalizers();
    }
}