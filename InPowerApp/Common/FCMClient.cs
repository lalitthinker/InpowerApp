using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Firebase.Iid;
using PCL.Service;
using static PCL.Service.CommonService;

namespace InPowerApp.Common
{
 public   class FCMClient
    {
        [Service]
        [IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
        public class MyFirebaseIIDService : FirebaseInstanceIdService
        {
            const string TAG = "MyFirebaseIIDService";
            public  override void OnTokenRefresh()
            {
                var refreshedToken = FirebaseInstanceId.Instance.Token;
                Log.Debug(TAG, "Refreshed token: " + refreshedToken);
               
                
                SendRegistrationToServer(refreshedToken);
              
            }
          public  void SendRegistrationToServer(string AndroidToken)
            {
                AndroidTokenViewModel _objAndroidTokenViewModel = new AndroidTokenViewModel();
                _objAndroidTokenViewModel.AndroidToken = AndroidToken;
              
                  new CommonService().PostUpdateAndroidToken(_objAndroidTokenViewModel);
             


            }
        }
    }
}