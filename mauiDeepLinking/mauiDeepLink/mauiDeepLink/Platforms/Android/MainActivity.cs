using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Content;
using Android.Runtime;
using Android.Widget;
using CommunityToolkit.Mvvm.Messaging;
using mauiDeepLink.Messages;

namespace mauiDeepLink
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
            DataScheme = "call", DataHost = "my.com", DataPathPrefix = "/sum"
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
         DataScheme = "calls", DataHost = "my.com", DataPathPrefix = "/sum"
         )
    ]

    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var uri = Intent?.Data;

            if (uri != null)
            {
                string apitoken = uri.ToString().Substring(uri.ToString().LastIndexOf('=') + 1);
                // This won't work because our MainPage or anyother ui is not registered when app is fully killed
                // where as App.Xaml.cs is registered  so register the weakreference manager inside the App.Xaml.cs or else it won't work.


                Task.Run(async()=>
                {
                    //await Task.Delay(1500);
                    WeakReferenceMessenger.Default
                    .Send(new NotificationItemMessage(apitoken));
                });
                //WeakReferenceMessenger.Default
                //    .Send(new NotificationItemMessage(apitoken));

                //WeakReferenceMessenger.Default.Send(new NotificationItemMessage(apitoken));
            }
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            var action = intent.Action;
            var strLink = intent.DataString;
            if (Intent.ActionView == action && !string.IsNullOrWhiteSpace(strLink))
            {
                //this get's triggered when the link clicked for the 2nd, 3rd or any infinity time and app is open in backgraound

                //Toast.MakeText(this, strLink, ToastLength.Short).Show();
                //Extract out token from here:
                string apitoken = strLink.Substring(strLink.LastIndexOf('=') + 1);

                Task.Run(async () =>
                {
                    //await Task.Delay(1500);
                    WeakReferenceMessenger.Default
                    .Send(new NotificationItemMessage(apitoken));
                });
            };
        }
    }
}
