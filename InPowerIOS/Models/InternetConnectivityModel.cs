using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using Plugin.Connectivity;

namespace InPowerApp.Model
{
    public static class InternetConnectivityModel
    {
        public static bool CheckConnection(bool ShowToast = false)
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                return true;
            }
            else
            {
                //if (ShowToast)
                    //Toast.MakeText(context, "Unable to process your request, No internet", ToastLength.Short).Show();
                return false;
            }
        }
    }
}