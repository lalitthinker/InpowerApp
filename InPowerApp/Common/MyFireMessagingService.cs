using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Messaging;
using Android.Graphics;
using Android.Media;
using Android.Support.V4.App;
using static InPowerApp.Resource;

namespace InPowerApp.Common
{
    [Service, IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class MyFirebaseListenerService : FirebaseMessagingService
    {
        string GROUP_KEY_WORK_EMAIL = "com.android.example.WORK_EMAIL";
        public override void OnMessageReceived(RemoteMessage message)
        {
            base.OnMessageReceived(message);


            var notification = message.GetNotification();
            var title = notification.Title;
            var body = notification.Body;
            string userid =(CommonHelper.GetUserId().ToString());
            if (userid != "" && CommonHelper.GetUserId().ToString()== userid)
            {
                SendNotification(title, body);
            }
           
        }

        private void SendNotification(string title, string body)
        {
           
          
            var intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            // intent.PutExtra("launchArguments", "stuff");

            var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot);

            var defaultSoundUri = RingtoneManager.GetDefaultUri(RingtoneType.Notification);

            var notificationBuilder =
                new NotificationCompat.Builder(this)
                    .SetSmallIcon(Resource.Drawable.InpowerIcon85x85)
                    .SetContentTitle(title)
                    .SetContentText(body)

                    .SetAutoCancel(true)
                    .SetSound(defaultSoundUri)
                    .SetContentIntent(pendingIntent)
                    .SetGroup(title);
            var notificationManager = NotificationManager.FromContext(this);
            notificationManager.Notify(0, notificationBuilder.Build());

            var notificationBuilder2 =
               new NotificationCompat.Builder(this)
                   .SetSmallIcon(Resource.Drawable.InpowerIcon85x85)
                   .SetContentTitle(title)
                   .SetContentText(body)

                   .SetAutoCancel(true)
                   .SetSound(defaultSoundUri)
                   .SetContentIntent(pendingIntent)
                   .SetGroup(title);
            var notificationManage2r = NotificationManager.FromContext(this);
            notificationManager.Notify(0, notificationBuilder.Build());

            Notification summaryNotification =
    new NotificationCompat.Builder(this)
        .SetContentTitle(title)
        .SetContentTitle("Two new messages")
        .SetSmallIcon(Resource.Drawable.InpowerIcon85x85)
        .SetStyle(new NotificationCompat.InboxStyle()
                .AddLine(title)
                .AddLine(body)
                .SetBigContentTitle(body)
                .SetSummaryText(body))
        .SetGroup(title)
        .SetGroupSummary(true)
        .Build();

            NotificationManagerCompat notificationManager3 = NotificationManagerCompat.From(this);
            notificationManager.Notify(1, summaryNotification);
            notificationManager.Notify(2, summaryNotification);
            notificationManager.Notify(1, summaryNotification);

        }
    }
}

