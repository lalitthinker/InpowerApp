using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Plugin.Connectivity;

namespace InPowerApp.Model
{
    public static class InternetConnectivityModel
    {
        public static bool CheckConnection(Context context, bool ShowToast = false)
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                return true;
            }
            else
            {
                if (ShowToast)
                    Toast.MakeText(context, "Unable to process your request, No internet", ToastLength.Short).Show();
                return false;
            }
        }
    }
}