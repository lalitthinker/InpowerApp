using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using InPowerApp.Activities;
using InPowerApp.Common;
using InPowerApp.Model;
using Java.Util;
using Square.Picasso;

namespace InPowerApp.ListAdapter
{
    public class GroupMessageAdapter : RecyclerView.Adapter
    {
        // Event handler for item clicks:
        public event EventHandler<int> ItemClick;

        // Underlying data set (a photo album):
        public List<GroupMessage> rvmGroupMessage;

        List<ListItem> consolidatedList;
        Activity context;
        // Load the adapter with the data set (photo album) at construction time:
        public GroupMessageAdapter(Activity ContextActy, Dictionary<DateTime, List<GroupMessage>> GroupMessage)
        {
            //  rvmGroupMessage = GroupMessage.ToList();

            consolidatedList = new List<ListItem>();

            foreach (var item in GroupMessage)
            {
                DateItem dateItem = new DateItem();
                dateItem.setDate(item.Key.ToShortDateString());
                consolidatedList.Add(dateItem);

                foreach (var general in item.Value)
                {
                    GeneralGroupItem generalItem = new GeneralGroupItem();
                    generalItem.setChatMessagearray(general);
                    consolidatedList.Add(generalItem);
                }
            }
            context = ContextActy;
        }
        public override int GetItemViewType(int position)
        {
            return consolidatedList[position].getType();
        }
        public int getItemCount()
        {
            return consolidatedList != null ? consolidatedList.Count : 0;
        }


        public override RecyclerView.ViewHolder
            OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            // Inflate the CardView for the photo:
            //////View itemView = LayoutInflater.From(parent.Context).
            //////            Inflate(Resource.Layout.List_item_private_chat_message_send_recv, parent, false);

            //////GroupMessageHolder vh = new GroupMessageHolder(itemView, OnClick,);
            //////return vh;
            RecyclerView.ViewHolder viewHolder = null;
            LayoutInflater inflater = LayoutInflater.From(parent.Context);


            switch (viewType)
            {

                case ListItem.TYPE_GENERAL:
                    View v1 = inflater.Inflate(Resource.Layout.List_item_group_chat_message_send_recv, parent, false);
                    viewHolder = new GroupMessageViewHolder(v1, OnClick, consolidatedList);
                    break;

                case ListItem.TYPE_DATE:

                    View v2 = inflater.Inflate(Resource.Layout.List_Item_Date_privateMsg, parent, false);
                    viewHolder = new DateViewHolder(v2, consolidatedList);
                    break;
            }

            return viewHolder;
        }

        public override void
        OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            GroupMessageViewHolder vh = holder as GroupMessageViewHolder;
            switch (consolidatedList[position].getType())
            {
                case 1:
                    {

                        GeneralGroupItem GeneralItem = (GeneralGroupItem)consolidatedList[position];

                        var item = GeneralItem.getChatMessagearray();
                        //Boolean isMe = item.ContactId != Common.CommonHelper.GetUserId();
                        var AttachList = (item.MessageId != 0) ? Repositories.GroupRepository.GetGroupMessageAttachList(item.MessageId) : new List<GroupAttachment>();
                        //    var item = rvmGroupMessage[position];
                        Boolean ishide = false;
                        if (position > 0)
                        {
                            if (consolidatedList[position - 1].getType() == 1)
                            {
                                GeneralGroupItem GeneralItemPrevious = (GeneralGroupItem)consolidatedList[position - 1];
                                string oldName = GeneralItemPrevious.getChatMessagearray().senderName;
                                string newName = item.senderName;
                                ishide = oldName == newName;
                            }
                        }


                        Boolean isMe = item.SenderUserId == Common.CommonHelper.GetUserId();

                        if (isMe)
                        {
                            vh.tv_messageRight.Text = item.MessageText;
                            vh.message_timeRight.Text = item.MessageTime.ToLocalTime().ToString("hh:mm tt");

                            if (AttachList.Count > 0)
                            {
                                CommonHelper.SetImageOnUIImageView(vh.iv_AttachImage, AttachList.FirstOrDefault().url, context, 600, 600);

                                vh.iv_AttachImage.SetOnClickListener(new iv_AttachImageClikLitenerGroupImage(item, context));
                                vh.ll_LinearLayoutForImageAttachRight.Visibility = ViewStates.Visible;
                            }
                            else
                            {
                                vh.ll_LinearLayoutForImageAttachRight.Visibility = ViewStates.Gone;
                            }

                            var Status = (item.MessageId != 0) ? Repositories.GroupRepository.GetGroupMessageOverallStatusbyid(item.MessageId) : new GroupMessageStatus();
                            if (Status.IsRead)
                            {
                                vh.iv_StatusRight.SetImageResource(Resource.Drawable.message_got_read_receipt_from_target);
                            }

                            else if (Status.IsRecieved)
                            {
                                vh.iv_StatusRight.SetImageResource(Resource.Drawable.message_got_receipt_from_target);
                            }

                            else if (Status.IsSend)
                            {
                                vh.iv_StatusRight.SetImageResource(Resource.Drawable.message_got_receipt_from_server);
                            }
                            else
                            {
                                vh.iv_StatusRight.SetImageResource(Resource.Drawable.pending);
                            }

                            vh.ll_LinearLayoutRight.Visibility = ViewStates.Visible;
                            vh.ll_LinearLayoutLeft.Visibility = ViewStates.Gone;
                        }
                        else
                        {
                            vh.tv_messageLeft.Text = item.MessageText;
                            vh.message_timeLeft.Text = item.MessageTime.ToLocalTime().ToString("hh:mm tt");
                            if (ishide)
                            {
                                vh.tv_SenderNameLeft.Visibility = ViewStates.Gone;
                            }
                            else
                            {
                                vh.tv_SenderNameLeft.Visibility = ViewStates.Visible;
                                vh.tv_SenderNameLeft.Text = item.senderName;
                            }
                            if (AttachList.Count > 0)
                            {
                                CommonHelper.SetImageOnUIImageView(vh.iv_AttachImageLeft, AttachList.FirstOrDefault().url, context, 600, 600);
                                vh.iv_AttachImageLeft.SetOnClickListener(new iv_AttachImageLeftClikLitenerGroupImage(item, context));
                                vh.ll_LinearLayoutForImageAttachLeft.Visibility = ViewStates.Visible;
                            }
                            else
                            {
                                vh.ll_LinearLayoutForImageAttachLeft.Visibility = ViewStates.Gone;
                            }

                            vh.ll_LinearLayoutLeft.Visibility = ViewStates.Visible;
                            vh.ll_LinearLayoutRight.Visibility = ViewStates.Gone;
                        }
                        break;
                    }
                case 0:
                    {
                        DateViewHolder dh = holder as DateViewHolder;
                        DateItem DateItem = (DateItem)consolidatedList[position];
                        var datetimedata = Convert.ToDateTime(DateItem.getDate()).ToLocalTime().Date;

                        if (datetimedata.Date == DateTime.Now.Date)
                        {
                            dh.txt_Date_message.Text = "Today";
                        }
                        else if (datetimedata.Date == DateTime.Now.Date.AddDays(-1))
                        {
                            dh.txt_Date_message.Text = "Yesterday";
                        }
                        else
                        {
                            dh.txt_Date_message.Text = datetimedata.ToString("MMM dd, yyyy");
                        }
                        break;
                    }
            }
        }

        // Return the number of photos available in the photo album:
        public override int ItemCount
        {
            get { return consolidatedList.Count; }
        }

        // Raise an event when the item-click takes place:
        void OnClick(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }

        public void add(ChatMessage GroupMessage)
        {


            GeneralItem generalItem = new GeneralItem();
            generalItem.setChatMessagearray(GroupMessage);
            consolidatedList.Add(generalItem);


        }



    }


    //public class DateViewHolder : RecyclerView.ViewHolder
    //{
    //    public TextView txt_Date_message { get; set; }


    //    List<ListItem> consolidatedList { get; set; }
    //    public DateViewHolder(View itemView, List<ListItem> consolidatedList)
    //        : base(itemView)
    //    {
    //        this.consolidatedList = consolidatedList;
    //        txt_Date_message = itemView.FindViewById<TextView>(Resource.Id.txt_Date_message);

    //    }

    //}

    internal class iv_AttachImageClikLitenerGroupImage : Java.Lang.Object, View.IOnClickListener
    {
        private GroupMessage item;
        private Activity context;

        public iv_AttachImageClikLitenerGroupImage(GroupMessage item, Activity context)
        {
            this.item = item;
            this.context = context;
        }

        public void OnClick(View v)
        {
            var AttachList = (item.MessageId != 0) ? Repositories.ChatAttachmentRepository.GetChatAttachList(item.MessageId) : new List<ChatAttachment>();

            if (AttachList.Count > 0)
            {
                Intent intent = new Intent();
                intent.SetAction(Intent.ActionView);

                var fileAndpath = new Java.IO.File(
                 Android.OS.Environment.GetExternalStoragePublicDirectory(
                     Android.OS.Environment.DirectoryPictures), System.IO.Path.Combine("Inpower", System.IO.Path.GetFileName(AttachList.FirstOrDefault().url)));
                Android.Net.Uri uri = Android.Net.Uri.FromFile(fileAndpath);
                uri = Android.Net.Uri.Parse(uri.EncodedPath.ToString());
                intent.SetDataAndType(uri, "image/*");
                context.StartActivity(intent);
            }
            else
            {

                Toast.MakeText(context, "Picture Not Available", ToastLength.Long).Show();
            }
        }
    }

    internal class iv_AttachImageLeftClikLitenerGroupImage : Java.Lang.Object, View.IOnClickListener
    {
        private GroupMessage item;
        private Activity context;

        public iv_AttachImageLeftClikLitenerGroupImage(GroupMessage item, Activity context)
        {
            this.item = item;
            this.context = context;
        }

        public void OnClick(View v)
        {

            var AttachList = (item.MessageId != 0) ? Repositories.ChatAttachmentRepository.GetChatAttachList(item.MessageId) : new List<ChatAttachment>();

            if (AttachList.Count > 0)
            {

                Intent intent = new Intent();
                intent.SetAction(Intent.ActionView);

                var fileAndpath = new Java.IO.File(
                 Android.OS.Environment.GetExternalStoragePublicDirectory(
                     Android.OS.Environment.DirectoryPictures), System.IO.Path.Combine("Inpower", System.IO.Path.GetFileName(AttachList.FirstOrDefault().url)));
                Android.Net.Uri uri = Android.Net.Uri.FromFile(fileAndpath);
                uri = Android.Net.Uri.Parse(uri.EncodedPath.ToString());
                intent.SetDataAndType(uri, "image/*");
                context.StartActivity(intent);
            }

        }
    }

    public class GroupMessageViewHolder : RecyclerView.ViewHolder
    {
        //public CardView cv_CardViewGroupLeft { get; private set; }
        public LinearLayout ll_LinearLayoutLeft { get; private set; }
        public LinearLayout ll_LinearLayoutForImageAttachLeft { get; private set; }
        public TextView tv_SenderNameLeft { get; private set; }
        public TextView tv_messageLeft { get; private set; }
        public TextView message_timeLeft { get; private set; }
        public ImageView imageViewarrow_bgLeft { get; private set; }
        List<ListItem> consolidatedList { get; set; }
        //public CardView cv_CardViewGroupRight { get; private set; }

        public LinearLayout ll_LinearLayoutForImageAttachRight { get; private set; }
        public LinearLayout ll_LinearLayoutRight { get; private set; }

        public TextView tv_messageRight { get; private set; }
        public TextView message_timeRight { get; private set; }
        public ImageView imageViewarrow_bgRight { get; private set; }
        public ImageView iv_StatusRight { get; private set; }
        public ImageView iv_AttachImage { get; private set; }
        public ImageView iv_AttachImageLeft { get; private set; }
        // Get references to the views defined in the CardView layout.
        public GroupMessageViewHolder(View itemView, Action<int> listener, List<ListItem> consolidatedList)
            : base(itemView)
        {
            this.consolidatedList = consolidatedList;
            ll_LinearLayoutLeft = itemView.FindViewById<LinearLayout>(Resource.Id.ll_LinearLayoutLeft);
            ll_LinearLayoutForImageAttachLeft = itemView.FindViewById<LinearLayout>(Resource.Id.ll_LinearLayoutForImageAttachLeft);
            tv_SenderNameLeft = itemView.FindViewById<TextView>(Resource.Id.tv_SenderNameLeft);
            tv_messageLeft = itemView.FindViewById<TextView>(Resource.Id.tv_messageLeft);
            message_timeLeft = itemView.FindViewById<TextView>(Resource.Id.message_timeLeft);
            imageViewarrow_bgLeft = itemView.FindViewById<ImageView>(Resource.Id.imageViewarrow_bgLeft);
            iv_AttachImage = itemView.FindViewById<ImageView>(Resource.Id.iv_AttachImage);
            //cv_CardViewGroupRight = itemView.FindViewById<CardView>(Resource.Id.cv_CardViewGroupRight);
            ll_LinearLayoutRight = itemView.FindViewById<LinearLayout>(Resource.Id.ll_LinearLayoutRight);
            ll_LinearLayoutForImageAttachRight = itemView.FindViewById<LinearLayout>(Resource.Id.ll_LinearLayoutForImageAttachRight);
            tv_messageRight = itemView.FindViewById<TextView>(Resource.Id.tv_messageRight);
            message_timeRight = itemView.FindViewById<TextView>(Resource.Id.message_timeRight);
            imageViewarrow_bgRight = itemView.FindViewById<ImageView>(Resource.Id.imageViewarrow_bgRight);
            iv_StatusRight = itemView.FindViewById<ImageView>(Resource.Id.iv_StatusRight);
            iv_AttachImageLeft = itemView.FindViewById<ImageView>(Resource.Id.iv_AttachImageLeft);
        }
    }

    public class GeneralGroupItem : ListItem
    {

        private GroupMessage GroupChatMessageArray;

        public GroupMessage getChatMessagearray()
        {
            return GroupChatMessageArray;
        }

        public void setChatMessagearray(GroupMessage GroupChatMessageArray)
        {
            this.GroupChatMessageArray = GroupChatMessageArray;
        }


        public override int getType()
        {
            return TYPE_GENERAL;
        }
    }

    internal class SwipeRefreshLayoutGroupMessage : Java.Lang.Object, SwipeRefreshLayout.IOnRefreshListener
    {
        private GroupMessageActivity groupMessageActivity;
        public delegate void LoadMoreEventHandler(object sender, bool loadCalled);
        public event LoadMoreEventHandler LoadMoreEvent;

        public SwipeRefreshLayoutGroupMessage(GroupMessageActivity groupMessageActivity)
        {
            this.groupMessageActivity = groupMessageActivity;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (this.Handle == IntPtr.Zero)
                return;

        }

        public void OnRefresh()
        {
            LoadMoreEvent(this, true);
        }
    }

}