using System;
using System.Collections.Generic;
using Foundation;
using InPowerIOS.Model;
using InPowerIOS.NewChat;
using UIKit;
using static InPowerIOS.Chats.ChatViewContarollerSource;
using static InPowerIOS.Chats.GroupChatViewController;

namespace InPowerIOS.Chats
{
    public class GroupChatListSource : UITableViewSource
    {
        //static readonly NSString IncomingCellId = new NSString("Incoming");
        //static readonly NSString OutgoingCellId = new NSString("Outgoing");

        IList<ListItem> messages;

      
        public GroupChatListSource(IList<ListItem> messages)
        {
            if (messages == null)
                throw new ArgumentNullException(nameof(messages));

            this.messages = messages;
         
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return messages.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            //BubbleCell cell = null;
            //Message msg = messages[indexPath.Row];

            //cell = (BubbleCell)tableView.DequeueReusableCell(GetReuseId(msg.Type));
            //cell.Message = msg;

            //return cell;
            bool isLeft = false;
            ListItem msg = messages[indexPath.Row];
            List<GroupAttachment> AttachList=new List<GroupAttachment>() ;
            GroupMessage item = new GroupMessage();
            switch (msg.getType())
            {
                case 0:
                    {
                        //DateViewHolder dh = holder as DateViewHolder;
                        DateItem DateItem = (DateItem)msg;
                       
                        var cell = tableView.DequeueReusableCell("DateLabelCell") as DateLabelCell;
                        cell.UpdateCell(DateItem);
                     
                        return cell;
                       
                    }
                default :
                    GeneralGroupItem GeneralItem = (GeneralGroupItem)msg;
                    item = GeneralItem.getChatMessagearray();

                    Boolean isMe = item.SenderUserId == Common.CommonHelper.GetUserId();
                     AttachList = (item.MessageId != 0) ? Repositories.GroupRepository.GetGroupMessageAttachList(item.MessageId) : new List<GroupAttachment>();

                    if (isMe)
                    {
                        isLeft = false;

                        if (AttachList.Count > 0)
                        {
                            var cell = tableView.DequeueReusableCell(isLeft ? ChatBubbleWithAttachmentCell.KeyLeft : ChatBubbleWithAttachmentCell.KeyRight) as ChatBubbleWithAttachmentCell;
                            if (cell == null)
                                cell = new ChatBubbleWithAttachmentCell(isLeft);
                            cell.UpdateGroup(item);
                            return cell;
                        }
                        else
                        {
                            var cell = tableView.DequeueReusableCell(isLeft ? ChatBubbleCell.KeyLeft : ChatBubbleCell.KeyRight) as ChatBubbleCell;
                            if (cell == null)
                                cell = new ChatBubbleCell(isLeft);
                            cell.UpdateGroup(item);
                            return cell;
                        }
                    }
                    else
                    {
                        isLeft = true;
                        if (AttachList.Count > 0)
                        {
                            var cell = tableView.DequeueReusableCell(isLeft ? GroupChatBubbleWithAttachmentCell.KeyLeft : GroupChatBubbleWithAttachmentCell.KeyRight) as GroupChatBubbleWithAttachmentCell;
                            if (cell == null)
                                cell = new GroupChatBubbleWithAttachmentCell(isLeft);
                            cell.Update(item);
                            return cell;
                        }
                        else
                        {
                            var cell = tableView.DequeueReusableCell(isLeft ? GroupChatBubbleCell.KeyLeft : GroupChatBubbleCell.KeyRight) as GroupChatBubbleCell;
                            if (cell == null)
                                cell = new GroupChatBubbleCell(isLeft);
                            cell.Update(item);
                            return cell;
                        }
                    }
            }
           
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            ListItem msg = messages[indexPath.Row];
            if (msg.getType() == 1)
            {
                List<GroupAttachment> AttachList = new List<GroupAttachment>();

                GeneralGroupItem GeneralItem = (GeneralGroupItem)msg;
                var item = GeneralItem.getChatMessagearray();

                AttachList = (item.MessageId != 0) ? Repositories.GroupRepository.GetGroupMessageAttachList(item.MessageId) : new List<GroupAttachment>();
                Boolean isMe = item.SenderUserId == Common.CommonHelper.GetUserId();
                if (!isMe)
                {
                    if (AttachList.Count > 0)
                    {
                        return GroupChatBubbleWithAttachmentCell.GetHeight(tableView, item.MessageText, "10:11 PM").Height;
                    }
                    else
                    {
                        return GroupChatBubbleCell.GetHeight(tableView, item.MessageText, "10:11 PM").Height;

                    }
                }
                else
                {
                    if (AttachList.Count > 0)
                    {
                        return ChatBubbleWithAttachmentCell.GetHeight(tableView, item.MessageText, "10:11 PM").Height;
                    }
                    else
                    {
                        return ChatBubbleCell.GetHeight(tableView, item.MessageText, "10:11 PM").Height;

                    }
                }
            }
            else
            {
                return 32;
              
            }
        }


    }
}