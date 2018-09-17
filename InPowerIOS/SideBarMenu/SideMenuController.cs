using Foundation;
using System;
using UIKit;
using System.Collections.Generic;
using InPowerIOS.Common;
using InPowerIOS.Model;
using InPowerIOS.Repositories;
using SDWebImage;
using InPowerIOS.Book;
using InPowerIOS.Setting;
using InPowerIOS.Chats;

namespace InPowerIOS.SideBarMenu
{
    public partial class SideMenuController : BaseController
    {
        MenuListSource menuTableview { get; set; }
        public SideMenuController(IntPtr handle) : base(handle)
        {
        }

       
   
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
          

            profileImage.UserInteractionEnabled = true;
            var selectImageTapped = new UITapGestureRecognizer(() => { ShowUpdateUserProfileViewController(); });
            profileImage.AddGestureRecognizer(selectImageTapped);
     


            setProfileDetails();
            List<MenuListItem> _ObjMenuList = new List<MenuListItem>();

            MenuListItem _ChatsMenu = new MenuListItem();
            _ChatsMenu.Name = "Chats";
            _ChatsMenu.ImageUrl = "ic_chats24.png";
            _ObjMenuList.Add(_ChatsMenu);

            MenuListItem _ContactsMenu = new MenuListItem();
            _ContactsMenu.Name = "Contacts";
            _ContactsMenu.ImageUrl = "ic_contacts32.png";
            _ObjMenuList.Add(_ContactsMenu);

            MenuListItem _BookShelfMenu = new MenuListItem();
            _BookShelfMenu.Name = "Book Shelf";
            _BookShelfMenu.ImageUrl = "ic_bookshelf32.png";
            _ObjMenuList.Add(_BookShelfMenu);


            MenuListItem _SettingMenu = new MenuListItem();
            _SettingMenu.Name = "Setting";
            _SettingMenu.ImageUrl = "ic_settings32.png";
            _ObjMenuList.Add(_SettingMenu);

            MenuListItem _LogoutMenu = new MenuListItem();
            _LogoutMenu.Name = "Logout";
            _LogoutMenu.ImageUrl = "ic_logout32.png";
            _ObjMenuList.Add(_LogoutMenu);
          //  tbl_MenuList.RowHeight = 60

            menuTableview = new MenuListSource(_ObjMenuList);
            var mainchatscontroller = (MainScreenTabBarController)Storyboard.InstantiateViewController("MainScreenTabBarController");
            NavController.PushViewController(mainchatscontroller, true);
            mainchatscontroller.SelectedIndex = 0;

            menuTableview.RowSelectedEvent += (sender, e) =>
            {
                var Data = ((MenuListSource)sender).selectedItem;

                switch (Data.Name)
                {
                    case "Chats":
                        var Chatscontroller = (MainScreenTabBarController)Storyboard.InstantiateViewController("MainScreenTabBarController");
                        NavController.PushViewController(Chatscontroller, true);
                      
                        Chatscontroller.SelectedIndex = 0;
                        SidebarController.CloseMenu();
                        break;

                    case "Contacts":
                        var contactscontroller = (MainScreenTabBarController)Storyboard.InstantiateViewController("MainScreenTabBarController");
                        //  if (NavController.TopViewController as MainScreenTabBarController == null)
                        NavController.PushViewController(contactscontroller, true);
                        contactscontroller.SelectedIndex = 1;

                        SidebarController.CloseMenu();
                        break;
                    case "Book Shelf":
                        var BookShelfcontroller = (BooksTabBarController)Storyboard.InstantiateViewController("BooksTabBarController");
                        //  if (NavController.TopViewController as MainScreenTabBarController == null)
                        NavController.PushViewController(BookShelfcontroller, true);
                        BookShelfcontroller.SelectedIndex = 0;
                        SidebarController.CloseMenu();
                        break;
                    case "Setting":
                        var Settingcontroller = (SettingViewController)Storyboard.InstantiateViewController("SettingViewController");
                        NavController.PushViewController(Settingcontroller, true);
                        SidebarController.CloseMenu();
                        break;
                    case "Logout":
                        LogoutUser();
                        break;
                }
            };


           
            SideHeaderBackgroundView.BackgroundColor = ColorExtensions.NavigationColor();
            tbl_MenuList.Source = menuTableview;
            tbl_MenuList.ReloadData();

        }

        private void ShowUpdateUserProfileViewController()
        {
            InvokeOnMainThread(delegate
            {
                var updateUserProfileViewcontroller = (UpdateUserProfileViewController)Storyboard.InstantiateViewController("UpdateUserProfileViewController");
                NavController.PushViewController(updateUserProfileViewcontroller, true);
                SidebarController.CloseMenu();
            });
        }

        partial void UIButton7616_TouchUpInside(UIButton sender)
        {
         
        }

        private void LogoutUser()
        {
            if (CommonHelper.IsUserLoggedIn())
            {
                //clear previously saved user data 
                CommonHelper.ClearUserPreferences();
                //to logout from google
                //to show log in screen agaian
                (UIApplication.SharedApplication.Delegate as AppDelegate).GoToLoginScreen();
            }
        }


        private void setProfileDetails()
        {
            UserProfile loginUserDetails = UserProfileRepository.GetUserProfile((int)CommonHelper.GetUserId());
            CommonHelper.SetCircularImage(profileImage);
            userName.Text = loginUserDetails.FirstName + " " + loginUserDetails.LastName;
            userEmail.Text = loginUserDetails.Email;

            if (!string.IsNullOrEmpty(loginUserDetails.ProfileImageUrl))
            {
                profileImage.SetImage(new NSUrl(loginUserDetails.ProfileImageUrl), UIImage.FromBundle("default_profile.png"));
            }
            else
            {
                profileImage.Image = new UIImage("default_profile.png");
            }
        }
    }
}