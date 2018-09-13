using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using InPowerApp.Common;
using InPowerApp.Repositories;
using PCL.Common;
using PCL.Model;
using PCL.Service;

namespace InPowerApp.Activities
{
    [Activity(Theme = "@style/MyTheme.Splash",MainLauncher =true, NoHistory = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation)]

    public class SplashActivity : AppCompatActivity
    {
        private static int SplashTimeOut = 3000;
        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);
            GlobalConstant.AccessToken= CommonHelper.GetAuthKey();
            SetContentView(Resource.Layout.SplashScreen);
            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Lollipop)
            {
                Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                Window.SetStatusBarColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.primary_material_dark)));
            }
        }
        // Launches the startup task
        protected override void OnResume()
        {
            base.OnResume();

            Task startupWork = new Task(() =>
            {
                Task.Delay(3000); // Simulate a bit of startup work.
            });

            startupWork.ContinueWith(t =>
            {
                DBInitializer.CreateDatabase();
                if (CommonHelper.IsUserLogin())
                {

                    GlobalConstant.AccessToken = CommonHelper.GetAuthKey();
                    StartActivity(new Intent(Application.Context, typeof(MainActivity)));
                }
                else
                {
                   

                    StartActivity(new Intent(Application.Context, typeof(ImageViewActivity)));
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());

            startupWork.Start();
        }

       
    }
}