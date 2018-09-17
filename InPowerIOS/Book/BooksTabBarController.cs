using Foundation;
using System;
using UIKit;
using InPowerIOS.SideBarMenu;

namespace InPowerIOS.Book
{
    public partial class BooksTabBarController : UITabBarController
    {
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

        public BooksTabBarController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
          
            //this will add the left side menu image to click to show sidebar
            NavigationItem.SetLeftBarButtonItem(
            new UIBarButtonItem(UIImage.FromBundle("threelines")
                , UIBarButtonItemStyle.Plain
                , (sender, args) => {
                    SidebarController.ToggleMenu();
                }), true);
        }
    }
}