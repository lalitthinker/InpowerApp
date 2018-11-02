using System;


using UIKit;
using Foundation;
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

            AppDelegate.ShowMessage("Hey!", "To see this sample in action, send a notification from Firebase Console", this);
        }

        void AppDelegate_MessageReceived(object sender, UserInfoEventArgs e)
        {
            if (e.MessageType == MessageType.Data)
                HandleDataMessage(e.UserInfo);
            else
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
            var apsDictionary = notification["aps"] as NSDictionary;

            string body;
            if (apsDictionary["alert"] is NSDictionary alertDictionary)
            {
                notificationSection.Caption = alertDictionary["title"].ToString();
                body = alertDictionary["body"].ToString();
            }
            else
            {
                notificationSection.Caption = "«No Notification Title»";
                body = apsDictionary["alert"].ToString();
            }

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
}
