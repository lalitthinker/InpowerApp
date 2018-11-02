using Foundation;
using System;
using UIKit;
using SidebarNavigation;
using System.Collections.Generic;
using CoreGraphics;
using InPowerIOS.Common;
using InPowerIOS.SideBarMenu;

namespace InPowerIOS.Chats
{
    public partial class MainScreenTabBarController : UITabBarController
    {
        UISearchBar sampleSearchBar;
        UIBarButtonItem RightSearchButton;

        protected SidebarNavigation.SidebarController SidebarController
        {
            get
            {
                return (UIApplication.SharedApplication.Delegate as AppDelegate).RootViewController.SidebarController;
            }
        }

        // provide access to the navigation controller to all inheriting controllers
        protected NavController NavController
        {
            get
            {
                return (UIApplication.SharedApplication.Delegate as AppDelegate).RootViewController.NavController;
            }
        }

        public MainScreenTabBarController(IntPtr handle) : base(handle)
        {

        }



        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
          
            Title = "InPower";
            //this will add the left side menu image to click to show sidebar
            NavigationItem.SetLeftBarButtonItem(
            new UIBarButtonItem(UIImage.FromBundle("threelines")
                , UIBarButtonItemStyle.Plain
                , (sender, args) => {
                    SidebarController.ToggleMenu();
                }), true);

       
          
        }

        //this event will handle the clicks of the menu item
      

        private void LogoutUser()
        {
            if (CommonHelper.IsUserLoggedIn())
            {
                //clear previously saved user data 
                CommonHelper.ClearUserPreferences();
                //to logout from google
            
                //to show log in screen agaian
              //  (UIApplication.SharedApplication.Delegate as AppDelegate).GoToLoginScreen();
            }
        }

        void SampleSearchBar_CancelButtonClicked(object sender, EventArgs e)
        {

            NavigationItem.Title = "InPower";
        }

        void SampleSearchBar_CancelButtonClicked1(object sender, EventArgs e)
        {
            sampleSearchBar.Hidden = true;

            NavigationItem.RightBarButtonItem = RightSearchButton;


        }

        void Search_OnEditingStopped(object sender, EventArgs e)
        {

        }

        //not in use but default method of pop over interface


        //right side menu pop up with logout and setting menu items



        // not in use right now but this calls when the view get refreshed
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

        }







    }

 
}