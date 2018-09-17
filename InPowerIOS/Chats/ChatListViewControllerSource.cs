using System;
using System.Collections.Generic;
using Foundation;
using InPowerIOS.Model;
using PCL.Model;
using UIKit;
using System.Linq;
using InPowerIOS.Repositories;
using Microsoft.AppCenter.Crashes;

namespace InPowerIOS.Chats
{
    public class ChatListViewControllerSource : UITableViewSource
    {
        private bool isFetching;
        private int pageIndex;
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public List<ChatConversation> tableItems { get; set; }
        private List<ChatConversation> searchItems = new List<ChatConversation>();  
       
        UIViewController uiNewView;

      
        private UISearchDisplayController search;

        public ChatListViewControllerSource(List<ChatConversation> items, UIViewController uiNewView)         {             this.tableItems = items;             this.searchItems = items;             this.uiNewView = uiNewView; 
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return searchItems.Count;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (searchItems[indexPath.Row].IsGroup == false)
            {
                var ChatCon = searchItems[indexPath.Row];
                var ContactView = new ContactViewModel { ContactId = (long)ChatCon.ContactId, ChatId = ChatCon.ChatId, ProfileImageUrl = "", IsBlock = ChatCon.IsBlock, ChatConvId = ChatCon.id };
                var chatViewContaroller = (PrivateChatListViewController)uiNewView.Storyboard.InstantiateViewController("PrivateChatListViewController");

           //     var chatViewContaroller = (ChatViewContaroller)uiNewView.Storyboard.InstantiateViewController("ChatViewContaroller");
                chatViewContaroller.contactViewModel = ContactView;
                uiNewView.NavigationController.PushViewController(chatViewContaroller, true);
            }
            else
            {
                var ChatCon = searchItems[indexPath.Row];
                var GroupModel = GroupRepository.GetGroupByID(Convert.ToInt64(ChatCon.GroupId));
                var groupchatViewContaroller = (GroupChatListViewController)uiNewView.Storyboard.InstantiateViewController("GroupChatListViewController");
                groupchatViewContaroller.GroupObject = GroupModel;
                uiNewView.NavigationController.PushViewController(groupchatViewContaroller, true);
            }
        }


        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell("ChatListTableViewCell") as ChatListTableViewCell;
            cell.UpdateCell(searchItems[indexPath.Row]);
            return cell;
        }

        public void PerformSearch(string searchText)  
        {  
            var results = new List<ChatConversation>();
            searchText = searchText.ToLower();  
          
            try
            {
                results.AddRange(
                    tableItems.Where(
                        chat => (
                      (ContactRepository.GetContactbyUserId(Convert.ToInt64(chat.ContactId)) != null) ?
                            ContactRepository.GetContactbyUserId(Convert.ToInt64(chat.ContactId)).name.ToLower().Contains(searchText) : false)
                        || ((chat.LastMessage != null) ? chat.LastMessage.ToLower().Contains(searchText) : false)));
            }
            catch (System.Exception ex)
            {
                Crashes.TrackError(ex);

            }
            try
            {
                results.AddRange(
                    tableItems.Where(
                      chat => (
                    (GroupRepository.GetGroupByID(Convert.ToInt64(chat.GroupId)) != null) ?
                            GroupRepository.GetGroupByID(Convert.ToInt64(chat.GroupId)).GroupName.ToLower().Contains(searchText) : false)
                        || ((chat.LastMessage != null) ? chat.LastMessage.ToLower().Contains(searchText) : false)));
            }
            catch (System.Exception ex)
            {

                Crashes.TrackError(ex);
            }
            this.searchItems = results;
        } 


        public void UpdateChat(ChatConversation chatConversation)
        {
            var newchatConversation  = this.searchItems.Where(a => a.ChatId == chatConversation.ChatId).FirstOrDefault();
            int index = this.searchItems.IndexOf(newchatConversation);
            this.searchItems[index] = chatConversation;
        }

        public void AddChat(ChatConversation chatConversation)
        {
            this.searchItems.Add(chatConversation);
        }
    }
}