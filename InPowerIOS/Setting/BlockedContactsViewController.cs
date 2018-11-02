using Foundation;
using System;
using UIKit;
using System.Threading.Tasks;
using InPowerIOS.Repositories;
using PCL.Service;
using InPowerApp.Model;
using System.Collections.Generic;
using InPowerIOS.Model;

namespace InPowerIOS.Setting
{
    public partial class BlockedContactsViewController : UIViewController
    {
        public BlockedContactsViewController (IntPtr handle) : base (handle)
        {
        }
        List<ChatConversation> BlockedContactList = new List<ChatConversation>();
        private BlockedContactsViewControllerSource blockedContactsViewControllerSource;
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
          
            Title = "Blocked Contacts";
            LoadBlockedContactListAsync();
        }


        private async Task LoadBlockedContactListAsync()
        {
            try
            {
                BlockedContactList = ChatConversationRepository.GetAllBlockList();

                if (BlockedContactList != null && BlockedContactList.Count > 0)
                {
                    tblBlockedUserList.TableFooterView = new UIView();

                    blockedContactsViewControllerSource = new BlockedContactsViewControllerSource(BlockedContactList);

                    tblBlockedUserList.Source = blockedContactsViewControllerSource;
                    tblBlockedUserList.RowHeight = 50;
                    tblBlockedUserList.ReloadData();
                }
            }
            catch (Exception ex)
            {
              
                new UIAlertView("Blocked Contacts", ex.ToString(), null, "OK", null).Show();
            }
        }

    }
}