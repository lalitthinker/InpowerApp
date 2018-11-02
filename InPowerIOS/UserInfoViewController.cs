using System;
using Foundation;
using MonoTouch.Dialog;
using UIKit;

namespace InPowerIOS
{
    public class UserInfoViewController : DialogViewController
    {
        public UserInfoViewController(AppDelegate appDelegate) : base(UITableViewStyle.Grouped, null, true)
        {
            Root = new RootElement("Notification Content");
            appDelegate.MessageReceived += AppDelegate_MessageReceived;
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);


        }

        void AppDelegate_MessageReceived(object sender, UserInfoEventArgs e)
        {
          
                HandleNotificationMessage(e.UserInfo);
        }

        void HandleDataMessage(NSDictionary data)
        {
            var notificationSection = new Section("Data Message");

            foreach (var key in data.Keys)
                if (data[key] is NSDictionary || data[key] is NSArray)
                    notificationSection.Add(new StringElement(key.ToString(), "Multiple values"));
                else
                    notificationSection.Add(new StringElement(key.ToString(), data[key].ToString()));

            Root.Add(notificationSection);
        }

        void HandleNotificationMessage(NSDictionary notification)
        {
            var notificationSection = new Section();
            var apsDictionary = notification["notification"] as NSDictionary;

            string body;
                body = apsDictionary["body"].ToString();
           

            notificationSection.Add(new StringElement("Body", body));
            AddCustomData(notification, notificationSection);
            Root.Add(notificationSection);
        }

        void AddCustomData(NSDictionary userInfo, Section notificationSection)
        {
            foreach (var key in userInfo.Keys)
            {
                if (key.ToString() == "aps" ||
                    key.ToString().StartsWith("gcm", StringComparison.InvariantCulture) ||
                    key.ToString().StartsWith("google", StringComparison.InvariantCulture))
                    continue;

                notificationSection.Add(new StringElement(key.ToString(), userInfo[key].ToString()));
            }
        }
    }
    public class UserInfoEventArgs : EventArgs
    {
        public NSDictionary UserInfo { get; private set; }
        public MessageTypeNoti MessageTypenoti { get; private set; }

        public UserInfoEventArgs(NSDictionary userInfo, MessageTypeNoti messageTypenoti)
        {
            UserInfo = userInfo;
            MessageTypenoti = messageTypenoti;
        }
    }

    public enum MessageTypeNoti
    {
        Notification,
        Data
    }
}
