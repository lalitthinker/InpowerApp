using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Android.App;
using Android.Graphics;
using Android.Views;
using Android.Widget;

using InPowerApp.Model;
using InPowerApp.Repositories;
using Java.Lang;
using Refractored.Controls;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;

using Android.Support.V4.Util;
using Android.Support.V4.App;
using Object = Java.Lang.Object;
using PCL.Common;
using Square.Picasso;
using InPowerApp.Common;

namespace InPowerApp.ListAdapter
{
    public class ChatListAdapter : RecyclerView.Adapter, IFilterable
    {

        public Filter Filter { get; private set; }

        public List<ChatConversation> searchChat;
        public List<ChatConversation> originalChat;

        // Event handler for item clicks:
        public event EventHandler<ChatConversation> PrivateChatItemClick;
        public event EventHandler<ChatConversation> GroupChatItemClick;
        List<int> iconId = new List<int>();

        List<ChatListItem> consolidatedList;
        int SrNo;
        Bitmap defaultProfile;
        Context context;
        public ChatListAdapter(Context contextChat, List<ChatConversation> _chat)
        {

            originalChat = _chat.ToList();
            this.iconId = iconId;

            context = contextChat;
            Filter = new ChatFilter(this);

        }
        // Create a new photo CardView (invoked by the layout manager): 
        public override RecyclerView.ViewHolder
            OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            RecyclerView.ViewHolder viewHolder = null;
            LayoutInflater inflater = LayoutInflater.From(parent.Context);


            switch (viewType)
            {

                case ChatListItem.TYPE_PRIVATE_CHAT:
                    View v1 = inflater.Inflate(Resource.Layout.ChatListItem, parent, false);
                    viewHolder = new ChatAdaptereViewHolder(v1, OnClickPrivateChatItem);
                    break;

                case ChatListItem.TYPE_GROUP_CHAT:

                    View v2 = inflater.Inflate(Resource.Layout.ChatGroupsListItem, parent, false);
                    viewHolder = new ChatGroupViewHolder(v2, OnClickGroupChatItem);
                    break;
            }
            return viewHolder;

            //// Inflate the CardView for the photo:
            //var itemView = LayoutInflater.From(parent.Context).
            //            Inflate(Resource.Layout.ChatListItem, parent, false);

            //// Create a ViewHolder to find and hold these view references, and 
            //// register OnClick with the view holder:
            //ChatAdaptereViewHolder vh = new ChatAdaptereViewHolder(itemView, OnClick);
            //return vh;
        }

        void OnClickPrivateChatItem(int position)
        {

            if (PrivateChatItemClick != null)
                PrivateChatItemClick(this, originalChat[position]);
        }
        void OnClickGroupChatItem(int position)
        {
            if (GroupChatItemClick != null)
                GroupChatItemClick(this, originalChat[position]);
        }
        // Fill in the contents of the photo card (invoked by the layout manager):
        public override void
            OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            switch (originalChat[position].IsGroup)
            {
                case false:
                    ChatAdaptereViewHolder vh = holder as ChatAdaptereViewHolder;
                    var ContactUser = ContactRepository.GetContactbyUserId(Convert.ToInt64(originalChat[position].ContactId));
                    if (ContactUser != null)
                    {
                        vh.SenderName.Text = ContactUser.name;
                        if (Convert.ToDateTime(originalChat[position].LastMessageDate).ToLocalTime().Date == DateTime.UtcNow.ToLocalTime().Date)
                        {
                            vh.MsgTextTime.Text = Convert.ToDateTime(originalChat[position].LastMessageDate).ToLocalTime().ToString("hh:mm tt");
                        }
                        else
                        {
                            vh.MsgTextTime.Text = Convert.ToDateTime(originalChat[position].LastMessageDate).ToLocalTime().Date.ToString("MM/dd/yyyy");
                        }
                        vh.txtFeedMessage.Text = originalChat[position].LastMessage;

                        int count = ChatMessageRepository.getChatMessageUnRead(originalChat[position].ChatId);

                        if (count > 0)
                        {
                            vh.MsgTextTime.SetTextColor(Color.YellowGreen);
                            vh.txtMessageCount.Visibility = ViewStates.Visible;
                            vh.txtMessageCount.Text = count.ToString();
                        }
                        else
                        {
                            vh.txtMessageCount.Visibility = ViewStates.Invisible;
                        }

                        if ( !string.IsNullOrEmpty(ContactUser.contactPicUrl))
                        {
                            Picasso.With(context)
                            .Load(ContactUser.contactPicUrl)
                            .Transform(new CircleTransformation())
                            .Fit()
                            .Into(vh.imgMessagelogo);
                        }
                    }
                    break;
                case true:
                    ChatGroupViewHolder viewHolder = holder as ChatGroupViewHolder;

                    int countGroupUnread = GroupRepository.getGroupMessageUnRead((long)originalChat[position].GroupId);
                    if (countGroupUnread > 0)
                    {
                        viewHolder.txtTime.SetTextColor(Color.YellowGreen);
                        viewHolder.txtMessageCount.Visibility = ViewStates.Visible;
                        viewHolder.txtMessageCount.Text = countGroupUnread.ToString();
                    }
                    else
                    {
                        viewHolder.txtMessageCount.Visibility = ViewStates.Invisible;
                    }

                    var GroupDetails = GroupRepository.GetGroupByID(Convert.ToInt64(originalChat[position].GroupId));
                    if (GroupDetails != null)
                    {
                        viewHolder.lblChatGroupName.Text = GroupDetails.GroupName;
                        if (Convert.ToDateTime(originalChat[position].LastMessageDate).ToLocalTime().Date == DateTime.UtcNow.ToLocalTime().Date)
                        {
                            viewHolder.txtTime.Text = Convert.ToDateTime(originalChat[position].LastMessageDate).ToLocalTime().ToString("hh:mm tt");
                        }
                        else
                        {
                            viewHolder.txtTime.Text = Convert.ToDateTime(originalChat[position].LastMessageDate).ToLocalTime().Date.ToString("MM/dd/yyyy");
                        }
                        if (!string.IsNullOrEmpty(originalChat[position].LastMessage))
                            viewHolder.lblChatGroupLastMSG.Text = originalChat[position].SenderName + " : " + originalChat[position].LastMessage;
                        else
                        {
                            var GroupUser = ContactRepository.GetContactbyUserId(GroupDetails.OwnerId);
                            if (GroupUser != null)
                            {
                                viewHolder.lblChatGroupLastMSG.Text = "Created by " + GroupUser.screenName;
                            }
                            else if (GroupDetails.OwnerId == CommonHelper.GetUserId())
                            {
                                viewHolder.lblChatGroupLastMSG.Text = "Created by me";
                            }
                            else
                            {
                                viewHolder.lblChatGroupLastMSG.Text = "Created by Unknown";
                            }
                        }

                        if (GroupDetails.GroupPictureUrl != null)
                        {
                            Picasso.With(context)
                            .Load(GroupDetails.GroupPictureUrl)
                            .Transform(new CircleTransformation())
                            .Fit()
                            .Into(viewHolder.ivgroupdefault);
                        }
                    }
                    break;
            }
        }

        public override int GetItemViewType(int position)
        {
            if (originalChat[position].IsGroup)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public int getItemCount()
        {
            return originalChat != null ? originalChat.Count : 0;
        }

        // Return the number of photos available in the photo album:
        public override int ItemCount
        {
            get { return originalChat.Count; }
        }

        public void add(ChatConversation book)
        {
            originalChat.Add(book);
        }

        public void update(ChatConversation Conv)
        {
            if (originalChat.Count > 0)
            {
                var PreviousMessage = originalChat.Where(a => a.ChatId == Conv.ChatId).FirstOrDefault();
                if (PreviousMessage != null)
                {
                    originalChat.Remove(PreviousMessage);
                    originalChat.Insert(0, Conv);
                }
            }
        }
    }

    public class ChatAdaptereViewHolder : RecyclerView.ViewHolder
    {
        public TextView SenderName { get; set; }
        public TextView txtFeedMessage { get; set; }
        // public CircleImageView imgProfilePic { get; set; }
        public ImageView imgMessagelogo { get; set; }

        public TextView MsgTextTime { get; set; }
        public TextView txtMessageCount { get; set; }
        public Action<int> _listener;

        public ChatAdaptereViewHolder(View itemView, Action<int> listener)
            : base(itemView)
        {
            SenderName = itemView.FindViewById<TextView>(Resource.Id.txtSenderName);
            txtFeedMessage = itemView.FindViewById<TextView>(Resource.Id.txtSenderMessage);
            MsgTextTime = itemView.FindViewById<TextView>(Resource.Id.txtTime);
            imgMessagelogo = itemView.FindViewById<ImageView>(Resource.Id.imgMessagelogo);
            txtMessageCount = itemView.FindViewById<TextView>(Resource.Id.txtMessageCount);
            itemView.Click += (sender, e) => listener(base.LayoutPosition);
        }
        public void SetClickListener(Action<int> listener)
        {
            _listener = listener;
            ItemView.Click += HandleClick;
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            //if (ItemView != null)
            //{
            //    ItemView.Click -= HandleClick;
            //}
            //_listener = null;
        }

        void HandleClick(object sender, EventArgs e)
        {
            if (_listener != null)
            {
                _listener(base.AdapterPosition);
            }
        }
    }

    public class ChatGroupViewHolder : RecyclerView.ViewHolder
    {
        public TextView lblChatGroupName { get; set; }
        public TextView txtTime { get; set; }
        public ImageView ivgroupdefault { get; set; }
        public TextView lblChatGroupMembersCount { get; set; }
        public TextView lblChatGroupLastMSG { get; set; }
        public TextView txtMessageCount { get; set; }
        public Action<int> _listener;

        public ChatGroupViewHolder(View itemView, Action<int> listener)
            : base(itemView)
        {
            lblChatGroupName = itemView.FindViewById<TextView>(Resource.Id.lblChatGroupName);
            txtTime = itemView.FindViewById<TextView>(Resource.Id.txtTime);
            lblChatGroupLastMSG = itemView.FindViewById<TextView>(Resource.Id.lblChatGroupLastMSG);
            txtMessageCount = itemView.FindViewById<TextView>(Resource.Id.txtMessageCount);
            ivgroupdefault = itemView.FindViewById<ImageView>(Resource.Id.ivgroupdefault);
            itemView.Click += (sender, e) => listener(base.LayoutPosition);
        }
        public void SetClickListener(Action<int> listener)
        {
            _listener = listener;
            ItemView.Click += HandleClick;
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            //if (ItemView != null)
            //{
            //    ItemView.Click -= HandleClick;
            //}
            //_listener = null;
        }

        void HandleClick(object sender, EventArgs e)
        {
            if (_listener != null)
            {
                _listener(base.AdapterPosition);
            }
        }
    }

    public class ChatFilter : Filter
    {
        private readonly ChatListAdapter _adapter;
        public ChatFilter(ChatListAdapter adapter)
        {
            _adapter = adapter;
        }

        protected override FilterResults PerformFiltering(ICharSequence constraint)
        {
            var returnObj = new FilterResults();
            var results = new List<ChatConversation>();
            if (_adapter.searchChat == null)
                _adapter.searchChat = _adapter.originalChat;

            if (constraint == null) return returnObj;

            if (_adapter.searchChat != null && _adapter.searchChat.Any())
            {
                // Compare constraint to all names lowercased. 
                // It they are contained they are added to results.
                try
                {
                    results.AddRange(
                        _adapter.searchChat.Where(
                            chat => (
                          (ContactRepository.GetContactbyUserId(Convert.ToInt64(chat.ContactId)) != null) ?
                            ContactRepository.GetContactbyUserId(Convert.ToInt64(chat.ContactId)).name.ToLower().Contains(constraint.ToString()) : false)
                            || ((chat.LastMessage != null) ? chat.LastMessage.ToLower().Contains(constraint.ToString()) : false)));
                }
                catch (System.Exception ex)
                {


                }
                //try
                //{
                //    results.AddRange(
                //      _adapter.searchChat.Where(
                //          chat => (
                //        (GroupRepository.GetGroupByID(Convert.ToInt64(chat.GroupId)) != null) ?
                //          GroupRepository.GetGroupByID(Convert.ToInt64(chat.GroupId)).GroupName.ToLower().Contains(constraint.ToString()) : false)
                //          || ((chat.LastMessage != null) ? chat.LastMessage.ToLower().Contains(constraint.ToString()) : false)));
                //}
                //catch (System.Exception ex)
                //{


                //}
            }

            // Nasty piece of .NET to Java wrapping, be careful with this!
            returnObj.Values = FromArray(results.Select(r => r.ToJavaObject()).ToArray());
            returnObj.Count = results.Count;

            constraint.Dispose();

            return returnObj;
        }

        protected override void PublishResults(ICharSequence constraint, FilterResults results)
        {
            using (var values = results.Values)
                _adapter.originalChat = values.ToArray<Java.Lang.Object>()
                    .Select(r => r.ToNetObject<ChatConversation>()).ToList();

            _adapter.NotifyDataSetChanged();

            // Don't do this and see GREF counts rising
            constraint.Dispose();
            results.Dispose();
        }
    }

    public class GroupChatListItem : ChatListItem
    {

        private GroupModel groupChatModel;

        public GroupModel getGroup()
        {
            return groupChatModel;
        }

        public void setDate(GroupModel groupChatModel)
        {
            this.groupChatModel = groupChatModel;
        }
        public override int getType()
        {
            return TYPE_GROUP_CHAT;
        }
    }
    public abstract class ChatListItem
    {
        public const int TYPE_PRIVATE_CHAT = 0;
        public const int TYPE_GROUP_CHAT = 1;

        abstract public int getType();
    }
    public class PrivateChatListItem : ChatListItem
    {

        private ChatMessage privateChatModel;

        public ChatMessage getChatMessagearray()
        {
            return privateChatModel;
        }

        public void setChatMessagearray(ChatMessage privateChatModel)
        {
            this.privateChatModel = privateChatModel;
        }


        public override int getType()
        {
            return TYPE_PRIVATE_CHAT;
        }
    }

}
