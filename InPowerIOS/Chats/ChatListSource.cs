using System;
using System;
using System.Collections.Generic;
using CoreGraphics;
using Foundation;
using InPowerIOS.Model;
using InPowerIOS.Models;
using InPowerIOS.NewChat;
using UIKit;
using static InPowerIOS.Chats.ChatViewContarollerSource;

namespace InPowerIOS.Chats
{
    public class ChatListSource : UITableViewSource
    {
        //static readonly NSString IncomingCellId = new NSString("Incoming");
        //static readonly NSString OutgoingCellId = new NSString("Outgoing");

        IList<ListItem> messages;

        readonly ChatBubbleCell[] sizingCells;

        public ChatListSource(IList<ListItem> messages)
        {
            if (messages == null)
                throw new ArgumentNullException(nameof(messages));

            this.messages = messages;
            // sizingCells = new BubbleCell[2];
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
            List<ChatAttachment> AttachList=new List<ChatAttachment>() ;
            ChatMessage item = new ChatMessage();
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
                    GeneralItem GeneralItem = (GeneralItem)msg;
                    item = GeneralItem.getChatMessagearray();

                    Boolean isMe = item.ContactId != Common.CommonHelper.GetUserId();
                    AttachList = (item.ChatMessageId != 0) ? Repositories.ChatAttachmentRepository.GetChatAttachList(item.ChatMessageId) : new List<ChatAttachment>();

                    if (isMe)
                    {
                        isLeft = false;


                    }
                    else
                    {
                        isLeft = true;
                    }

                    if (AttachList.Count > 0)
                    {
                        var cell = tableView.DequeueReusableCell(isLeft ? ChatBubbleWithAttachmentCell.KeyLeft : ChatBubbleWithAttachmentCell.KeyRight) as ChatBubbleWithAttachmentCell;
                        if (cell == null)
                            cell = new ChatBubbleWithAttachmentCell(isLeft);
                        cell.Update(item);
                        return cell;
                    }
                    else
                    {
                        var cell = tableView.DequeueReusableCell(isLeft ? ChatBubbleCell.KeyLeft : ChatBubbleCell.KeyRight) as ChatBubbleCell;
                        if (cell == null)
                            cell = new ChatBubbleCell(isLeft);
                        cell.Update(item);
                        return cell;
                    }

                 
            }
           
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            ListItem msg = messages[indexPath.Row];
            if (msg.getType() == 1)
            {
                List<ChatAttachment> AttachList = new List<ChatAttachment>();

                GeneralItem GeneralItem = (GeneralItem)msg;

                var item = GeneralItem.getChatMessagearray();
                AttachList = (item.ChatMessageId != 0) ? Repositories.ChatAttachmentRepository.GetChatAttachList(item.ChatMessageId) : new List<ChatAttachment>();
                if (AttachList.Count > 0)
                {
                    return ChatBubbleWithAttachmentCell.GetHeight(tableView, item.MessageText, "10:11 PM").Height;
                }
                else
                {
                    return ChatBubbleCell.GetHeight(tableView, item.MessageText, "10:11 PM").Height;

                }
            }
            else
            {
                return 32;
              
            }// return ChatBubbleCell.GetSizeForText(tableView, messages[indexPath.Row].Text).Height + ChatBubbleCell.BubblePadding.Height;
            //Message msg = messages[indexPath.Row];
            //return CalculateHeightFor(msg, tableView);
        }

        //public override nfloat EstimatedHeight(UITableView tableView, NSIndexPath indexPath)
        //{
        //    Message msg = messages[indexPath.Row];
        //    return CalculateHeightFor(msg, tableView);
        //}

        //nfloat CalculateHeightFor(Message msg, UITableView tableView)
        //{
        //    var index = (int)msg.Type;
        //    BubbleCell cell = sizingCells[index];
        //    if (cell == null)
        //        cell = sizingCells[index] = (BubbleCell)tableView.DequeueReusableCell(GetReuseId(msg.Type));

        //    cell.Message = msg;

        //    cell.SetNeedsLayout();
        //    cell.LayoutIfNeeded();
        //    CGSize size = cell.ContentView.SystemLayoutSizeFittingSize(UIView.UILayoutFittingCompressedSize);
        //    return NMath.Ceiling(size.Height) + 1;
        //}

        //NSString GetReuseId(MessageType msgType)
        //{
        //    return msgType == MessageType.Incoming ? IncomingCellId : OutgoingCellId;
        //}
    }
}