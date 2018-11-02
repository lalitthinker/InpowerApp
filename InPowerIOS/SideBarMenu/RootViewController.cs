using System;
using System.Drawing;
using UIKit;
using Foundation;
using CoreGraphics;
using SidebarNavigation;
using InPowerIOS.Chats;

namespace InPowerIOS.SideBarMenu
{
    public partial class RootViewController : UIViewController
    {
        private UIStoryboard _storyboard;
        // the sidebar controller for the app

        public SidebarNavigation.SidebarController SidebarController { get; private set; }

        // the navigation controller
        public NavController NavController { get; private set; }

        // the storyboard
        public override UIStoryboard Storyboard
        {
            get
            {
                if (_storyboard == null)
                    _storyboard = UIStoryboard.FromName("Main", null);
                return _storyboard;
            }
        }

        public RootViewController(IntPtr handle) : base(handle)
        {

        }


        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var introController = (MainScreenTabBarController)Storyboard.InstantiateViewController("MainScreenTabBarController");
            var menuController = (SideMenuController)Storyboard.InstantiateViewController("SideMenuController");

            // create a slideout navigation controller with the top navigation controller and the menu view controller
            NavController = new NavController();

            NavController.PushViewController(introController, false);
            SidebarController = new SidebarNavigation.SidebarController(this, NavController, menuController);
            SidebarController.MenuWidth = 220;
            SidebarController.ReopenOnRotate = false;
            SidebarController.MenuLocation = MenuLocations.Left;
           
        }
    }
}


