using System;
using System.Collections.Generic;
using Foundation;
using InPowerIOS.Model;
using InPowerIOS.Repositories;
using InPowerIOS.SideBarMenu;
using PCL.Model;
using UIKit;
using System.Linq;

namespace InPowerIOS.Chats
{
    public class ContactListViewControllerSource: UITableViewSource
    {
        public List<Contact> searchContact;
        public List<Contact> originalContact;
        UIViewController uiNewView;
        public ContactListViewControllerSource(List<Contact> items, UIViewController uiView)
        {
            this.originalContact = items;
            this.uiNewView = uiView;
            this.searchContact = items;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell("ContactListTableViewCell") as ContactListTableViewCell;
            cell.UpdateCell(searchContact[indexPath.Row], indexPath.Row);
         
            return cell;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }


        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return searchContact != null ? searchContact.Count : 0;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if(indexPath.Row == 0)
            {
                var createGroupViewController = (CreateGroupViewController)uiNewView.Storyboard.InstantiateViewController("CreateGroupViewController");
                uiNewView.NavigationController.PushViewController(createGroupViewController, true);
            }
            else
            {
                var ChatCon = searchContact[indexPath.Row];
        
                var ContactUser = ContactRepository.GetContactbyUserId(Convert.ToInt64(ChatCon.contactId));
                var ContactView = new ContactViewModel { ContactId = (long)ChatCon.contactId, ProfileImageUrl = ContactUser.contactPicUrl };

                var chatViewContaroller = (PrivateChatListViewController)uiNewView.Storyboard.InstantiateViewController("PrivateChatListViewController");

               // var chatViewContaroller = (NewChatViewController)uiNewView.Storyboard.InstantiateViewController("NewChatViewController");
                chatViewContaroller.contactViewModel = ContactView;
                uiNewView.NavigationController.PushViewController(chatViewContaroller, true);

            }
        }

        internal void PerformSearch(string searchText)
        {
            var results = new List<Contact>();
            searchText = searchText.ToLower();  

            if (originalContact != null)
            {
                // Compare constraint to all names lowercased. 
                // It they are contained they are added to results.
                results.Insert(0, null);
                results.AddRange(
                    originalContact.Where(
                        contact => contact!=null? (((contact.name != null) ? contact.name.ToLower().Contains(searchText) : false) || ((contact.screenName != null) ? contact.screenName.ToLower().Contains(searchText) : false)):false));
            }

            this.searchContact = results;
        }
    }
}
