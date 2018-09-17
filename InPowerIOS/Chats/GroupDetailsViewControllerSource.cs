using System;
using System.Collections.Generic;
using Foundation;
using InPowerIOS.Model;
using UIKit;

namespace InPowerIOS.Chats
{
    public class GroupDetailsViewControllerSource: UITableViewSource
    {
        public List<GroupMember> searchContact;
        public List<GroupMember> originalContact;
        UIViewController uiNewView;
        public GroupDetailsViewControllerSource(List<GroupMember> items, UIViewController uiView)
        {
            this.originalContact = items;
            this.uiNewView = uiView;
        }


            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell("GroupMemberListTableViewCell") as GroupMemberListTableViewCell;
            cell.UpdateCell(originalContact[indexPath.Row], indexPath.Row);

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

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
           
        }

    }
}