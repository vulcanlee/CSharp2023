using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Content;
using Android.Runtime;
using Android.Widget;
using CommunityToolkit.Mvvm.Messaging;

namespace mauiCalculator
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]

    //ForHttp URL's
    [IntentFilter(new[] { Intent.ActionView },
     Categories = new[]
         {
             Intent.ActionView,
             Intent.CategoryDefault,
             Intent.CategoryBrowsable
         },
            DataScheme = "dplcalculator", DataHost = "calculator.com", DataPathPrefix = "/sum"
         )
     ]


    //ForHttp's URL's
    [IntentFilter(new[] { Intent.ActionView },
     Categories = new[]
         {
             Intent.ActionView,
             Intent.CategoryDefault,
             Intent.CategoryBrowsable
         },
         DataScheme = "dplcalculators", DataHost = "calculator.com", DataPathPrefix = "/sum"
         )
    ]

    public class MainActivity : MauiAppCompatActivity
    {
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var uri = Intent?.Data;

            if (uri != null)
            {
                await Task.Delay(1500);

                string apitoken = uri.ToString().Substring(uri.ToString().LastIndexOf('=') + 1);
                int value1 = int.Parse(apitoken.Split("@")[0]);
                int value2 = int.Parse(apitoken.Split("@")[1]);
                int value3 = value1 + value2;
                string uriBack = "calls://my.com/sum?value=" + value3.ToString();
                Launcher.Default.OpenAsync(uriBack);
            }
        }

        protected override async void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            var action = intent.Action;
            var strLink = intent.DataString;
            if (Intent.ActionView == action && !string.IsNullOrWhiteSpace(strLink))
            {
                await Task.Delay(1500);
                string apitoken = strLink.Substring(strLink.LastIndexOf('=') + 1);
                int value1 = int.Parse(apitoken.Split("@")[0]);
                int value2 = int.Parse(apitoken.Split("@")[1]);
                int value3 = value1 + value2;
                string uriBack = "calls://my.com/sum?value=" + value3.ToString();
                Launcher.Default.OpenAsync(uriBack);
            };
        }
    }
}
