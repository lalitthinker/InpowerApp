using System;
using System.Collections.Generic;
using CoreGraphics;
using Foundation;
using InPowerIOS.Models;
using InPowerIOS.NewChat;
using UIKit;

namespace InPowerIOS.Chats
{
    public class ChatSource : UITableViewSource
    {
        static readonly NSString IncomingCellId = new NSString("Incoming");
        static readonly NSString OutgoingCellId = new NSString("Outgoing");

        IList<Message> messages;

        readonly BubbleCell[] sizingCells;

        public ChatSource(IList<Message> messages)
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
            BubbleCell cell = null;
            Message msg = messages[indexPath.Row];

            cell = (BubbleCell)tableView.DequeueReusableCell(GetReuseId(msg.Type));
            cell.Message = msg;

            return cell;
            //bool isLeft = false;
            //Message msg = messages[indexPath.Row];
            //MessageType str = msg.Type;
            //if(str==MessageType.Incoming)
            //{
            //    isLeft = true;
            //}
            //var cell = tableView.DequeueReusableCell(isLeft ? BubbleCell.KeyLeft : BubbleCell.KeyRight) as BubbleCell;
            //if (cell == null)
            //    cell = new BubbleCell(isLeft);
            //cell.Update(msg.Text);
            //return cell;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
           // return BubbleCell.GetSizeForText(tableView, messages[indexPath.Row].Text).Height + BubbleCell.BubblePadding.Height;
            Message msg = messages[indexPath.Row];
            return CalculateHeightFor(msg, tableView);
        }

        public override nfloat EstimatedHeight(UITableView tableView, NSIndexPath indexPath)
        {
            Message msg = messages[indexPath.Row];
            return CalculateHeightFor(msg, tableView);
        }

        nfloat CalculateHeightFor(Message msg, UITableView tableView)
        {
            var index = (int)msg.Type;
            BubbleCell cell = sizingCells[index];
            if (cell == null)
                cell = sizingCells[index] = (BubbleCell)tableView.DequeueReusableCell(GetReuseId(msg.Type));

            cell.Message = msg;

            cell.SetNeedsLayout();
            cell.LayoutIfNeeded();
            CGSize size = cell.ContentView.SystemLayoutSizeFittingSize(UIView.UILayoutFittingCompressedSize);
            return NMath.Ceiling(size.Height) + 1;
        }

        NSString GetReuseId(MessageType msgType)
        {
            return msgType == MessageType.Incoming ? IncomingCellId : OutgoingCellId;
        }
    }
}