using System;
using Facebook.CoreKit;
using Firebase.CloudMessaging;
using Firebase.Core;
using Foundation;
using InPowerIOS.Common;
using InPowerIOS.Login;
using InPowerIOS.Models;
using InPowerIOS.Repositories;
using InPowerIOS.SideBarMenu;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using PCL.Common;
using UIKit;
using UserNotifications;

namespace InPowerIOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate, IUNUserNotificationCenterDelegate, IMessagingDelegate
    {
        int count_notofication = 0;
        // class-level declarations

        private bool isAuthenticated = false;

        public event EventHandler<UserInfoEventArgs> MessageReceived;

      
        public UIStoryboard MainStoryboard

        {
            get { return UIStoryboard.FromName("Main", NSBundle.MainBundle); }
        }
        public RootViewController RootViewController { get { return Window.RootViewController as RootViewController; } }
        //public override UIWindow Window
        //{
        //  get;
        //  set;
        //}

        public void SetRootViewController(UIViewController rootViewController, bool animate)
        {
            if (animate)
            {
                var transitionType = UIViewAnimationOptions.TransitionFlipFromRight;

                Window.RootViewController = rootViewController;
                UIView.Transition(Window, 0.5, transitionType,
                                  () => Window.RootViewController = rootViewController,
                                  null);
            }
            else
            {
                Window.RootViewController = rootViewController;
            }
        }

        // class-level declarations

        public override UIWindow Window
        {
            get;
            set;
        }

      

        public void GoToLoginScreen()
        {
            var loginViewController = GetViewController(MainStoryboard, "LoginInpowerViewController") as LoginInpowerViewController;
            loginViewController.OnLoginSuccess += LoginViewController_OnLoginSuccess;
            SetRootViewController(loginViewController, false);
        }

        public UIViewController GetViewController(UIStoryboard storyboard, string viewControllerName)
        {
            return storyboard.InstantiateViewController(viewControllerName);
        }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            Profile.EnableUpdatesOnAccessTokenChange(true);


            application.SetStatusBarStyle(UIStatusBarStyle.LightContent, true);

            Settings.AppID = SocialConstants.fbClientId;
            Settings.DisplayName = SocialConstants.appName;

          
            DBInitializer.CreateDatabase();

            // Override point for customization after application launch.
            // If not required for your application you can safely delete this method

            if (CommonHelper.IsUserLoggedIn())
            {
                //Window = new UIWindow(UIScreen.MainScreen.Bounds);

                //// If you have defined a root view controller, set it here:
                //Window.RootViewController = new RootViewController();

                //// make the window visible
                //Window.MakeKeyAndVisible();
                GlobalConstant.AccessToken = CommonHelper.GetAccessToken();
                var tabBarController = GetViewController(MainStoryboard, "RootViewController");
                SetRootViewController(tabBarController, false);
            }
            else
            {
                //User needs to log in, so show the Login View Controlller
                var loginViewController = GetViewController(MainStoryboard, "LoginInpowerViewController") as LoginInpowerViewController;
                loginViewController.OnLoginSuccess += LoginViewController_OnLoginSuccess;
                SetRootViewController(loginViewController, false);
            }
            // Override point for customization after application launch.
            // If not required for your application you can safely delete this method
           
          
            AppCenter.Start("890484fd-ac71-4775-9baf-fbe55397bbc6", typeof(Analytics), typeof(Crashes));

            App.Configure();

            
          
            // Register your app for remote notifications.
            if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
            {
                // For iOS 10 display notification (sent via APNS)
                UNUserNotificationCenter.Current.Delegate = this;

                var authOptions = UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound;
                UNUserNotificationCenter.Current.RequestAuthorization(authOptions, (granted, error) => {
                    Console.WriteLine(granted);
                });

            }
            else
            {


                //// iOS 9 or before
                //var allNotificationTypes = UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound;
                //var settings = UIUserNotificationSettings.GetSettingsForTypes(allNotificationTypes, null);
                //UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
            }
            // iOS 9 or before
            var allNotificationTypes = UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound;
            var settings = UIUserNotificationSettings.GetSettingsForTypes(allNotificationTypes, null);
            UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
            UIApplication.SharedApplication.RegisterForRemoteNotifications();

            Messaging.SharedInstance.Delegate = this;

            // To connect with FCM. FCM manages the connection, closing it
            // when your app goes into the background and reopening it 
            // whenever the app is foregrounded.
            Messaging.SharedInstance.ShouldEstablishDirectChannel = true;


            var token = Messaging.SharedInstance.FcmToken;
            UIApplication.SharedApplication.ApplicationIconBadgeNumber = -1;
            count_notofication = 0;
            return true;
        }
      

        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {    // Handle Notification messages in the background and foreground.
            // Handle Data messages for iOS 9 and below.

            // If you are receiving a notification message while your app is in the background,
            // this callback will not be fired till the user taps on the notification launching the application.
            // TODO: Handle data of notification

            // With swizzling disabled you must let Messaging know about the message, for Analytics
            //  Messaging.SharedInstance.AppDidReceiveMessage (userInfo);

            //HandleMessage(userInfo);
            count_notofication++;
            updateBadge();
            // Print full message.
            LogInformation(nameof(DidReceiveRemoteNotification), userInfo);

            //  completionHandler(UIBackgroundFetchResult.NewData);
        }
        [Export("messaging:didReceiveMessage:")]
        public void DidReceiveMessage(Messaging messaging, RemoteMessage remoteMessage)
        {
            count_notofication++;
            updateBadge();
            //HandleMessage(userInfo);
            // Handle Data messages for iOS 10 and above.
            // HandleMessage(remoteMessage.AppData);
           // UIApplication.SharedApplication.ApplicationIconBadgeNumber = 10;
            LogInformation(nameof(DidReceiveMessage), remoteMessage.AppData);
        }

        void updateBadge()
        {
            UIApplication.SharedApplication.ApplicationIconBadgeNumber = count_notofication;
        }

        void HandleMessage(NSDictionary message)
        {
            var AvAlert = new UIAlertView(message.ValueForKey(new NSString("title")).ToString(), message.ValueForKey(new NSString("body")).ToString(), null, "OK", null);
            AvAlert.Show();

            UIApplication.SharedApplication.ApplicationIconBadgeNumber = count_notofication;
            //var content = new UNMutableNotificationContent();
            //content.Title = "Notification Title";
            //content.Subtitle = "Notification Subtitle";
            //content.Body = "This is the message body of the notification.";
            //content.Badge = 1;

            //var trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(5, false);

            //var requestID = "NewCategory";
            //var request = UNNotificationRequest.FromIdentifier(requestID, content, trigger);

            //UNUserNotificationCenter.Current.AddNotificationRequest(request, (err) =>
            //{
            //    if (err != null)
            //    {
            //        // Do something with error...
            //    }
            //});
            //var AvAlert = new UIAlertView("App", "notification", null, "OK", null);
            //AvAlert.Show();

        }
            
      
        void LogInformation(string methodName, object information) => Console.WriteLine($"\nMethod name: {methodName}\nInformation: {information}");

        void LoginViewController_OnLoginSuccess(object sender, EventArgs e)
        {
            //Window = new UIWindow(UIScreen.MainScreen.Bounds);

            //// If you have defined a root view controller, set it here:
            //Window.RootViewController = new RootViewController();

            //// make the window visible
            //Window.MakeKeyAndVisible();
            count_notofication = 0;
            var tabBarController = GetViewController(MainStoryboard, "RootViewController");
            SetRootViewController(tabBarController, true);
        }

        public override void OnResignActivation(UIApplication application)
        {
            // Invoked when the application is about to move from active to inactive state.
            // This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) 
            // or when the user quits the application and it begins the transition to the background state.
            // Games should use this method to pause the game.
        }

        public override void DidEnterBackground(UIApplication application)
        {
            // Use this method to release shared resources, save user data, invalidate timers and store the application state.
            // If your application supports background exection this method is called instead of WillTerminate when the user quits.
        }

        public override void WillEnterForeground(UIApplication application)
        {
            // Called as part of the transiton from background to active state.
            // Here you can undo many of the changes made on entering the background.
        }

        public override void OnActivated(UIApplication application)
        {
            // Restart any tasks that were paused (or not yet started) while the application was inactive. 
            // If the application was previously in the background, optionally refresh the user interface.
        }

        public override void WillTerminate(UIApplication application)
        {
            // Called when the application is about to terminate. Save data, if needed. See also DidEnterBackground.
        }
      
    }
}

