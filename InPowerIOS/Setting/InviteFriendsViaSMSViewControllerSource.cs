using System;
using System.Collections.Generic;
using Foundation;
using InPowerIOS.Models;
using UIKit;

namespace InPowerIOS.Setting
{
    public class InviteFriendsViaSMSViewControllerSource: UITableViewSource
    {
        public event EventHandler<int> BtnInviteClick;
        public List<PhoneContactIOSModel> originalContact;
        int currentRowIndex = 0;
        UIViewController uIViewController;
        public InviteFriendsViaSMSViewControllerSource(List<PhoneContactIOSModel> items, UIViewController uIViewControllerParent)
        {
            this.uIViewController = uIViewControllerParent;
            this.originalContact = items;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            currentRowIndex = indexPath.Row;
            var cell = tableView.DequeueReusableCell("InviteFriendsViaSMSTableViewCell") as InviteFriendsViaSMSTableViewCell;
            cell.UpdateCell(originalContact[currentRowIndex], currentRowIndex, this.uIViewController);
            return cell;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }


        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return originalContact != null ? originalContact.Count : 0;
        }

    }
}
