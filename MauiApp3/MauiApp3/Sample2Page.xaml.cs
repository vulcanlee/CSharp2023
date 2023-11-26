namespace MauiApp3;

public partial class Sample2Page : ContentPage
{
	public Sample2Page(Sample2PageViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;
	}

	~Sample2Page()
	{
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        var page = App.Current.MainPage;
        App.Current.MainPage = new Sample1Page(new Sample1PageViewModel());
        //App.Current.MainPage = new MainPage(new MainPageViewModel());
         page = App.Current.MainPage;

        GC.Collect();
        GC.Collect();
        GC.Collect(2);
        GC.WaitForPendingFinalizers();
    }
}