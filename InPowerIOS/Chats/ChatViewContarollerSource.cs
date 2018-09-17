using System;
using System.Collections.Generic;
using Foundation;
using InPowerIOS.Model;
using UIKit;

namespace InPowerIOS.Chats
{
    public class ChatViewContarollerSource: UITableViewSource
    {
        public Dictionary<DateTime, List<ChatMessage>> ChatConverstions { get; set; }
       
        UIViewController uiNewView;
   
        List<ListItem> consolidatedList;
        public ChatViewContarollerSource(Dictionary<DateTime, List<ChatMessage>> items, UIViewController uiView)
        {
            this.ChatConverstions = items;
            this.uiNewView = uiView;

            consolidatedList = new List<ListItem>();

            foreach (var itemm in ChatConverstions)
            {
                DateItem dateItem = new DateItem();
                dateItem.setDate(itemm.Key.ToShortDateString());
                consolidatedList.Add(dateItem);

                foreach (var general in itemm.Value)
                {
                    GeneralItem generalItem = new GeneralItem();
                    generalItem.setChatMessagearray(general);
                    consolidatedList.Add(generalItem);
                }
            }

        }


        public class DateItem : ListItem
        {

            private string date;

            public string getDate()
            {
                return date;
            }

            public void setDate(string date)
            {
                this.date = date;
            }


            public override int getType()
            {
                return TYPE_DATE;
            }
        }
        public abstract class ListItem
        {

            public const int TYPE_DATE = 0;
            public const int TYPE_GENERAL = 1;

            abstract public int getType();
        }
        public class GeneralItem : ListItem
        {

            private ChatMessage ChatMessageArray;

            public ChatMessage getChatMessagearray()
            {
                return ChatMessageArray;
            }

            public void setChatMessagearray(ChatMessage ChatMessageArray)
            {
                this.ChatMessageArray = ChatMessageArray;
            }


            public override int getType()
            {
                return TYPE_GENERAL;
            }
        }



        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return consolidatedList != null ? consolidatedList.Count : 0;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            tableView.DeselectRow(indexPath,false);
        }

        public int GetItemViewType(int position)
        {
            return consolidatedList[position].getType();
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell("ChatTableViewCell") as ChatTableViewCell;
            cell.UpdateCell(consolidatedList[indexPath.Row], indexPath.Row);
            return cell;
        }



    }
}