using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using InPowerApp.Common;
using InPowerApp.Model;
using InPowerApp.Repositories;
using PCL.Model;
using PCL.Service;

namespace InPowerApp.ListAdapter
{
    class BlockedContactListAdapter : RecyclerView.Adapter
    {
        Activity context;
        public event EventHandler<int> ListReload;

        public List<ChatConversation> searchContacts;
        public List<ChatConversation> originalContacts;
        Activity Contextt;


        public BlockedContactListAdapter(List<ChatConversation> contact, Activity Context)
        {
            originalContacts = contact;
            this.Contextt = Context;
        }

        // Create a new photo CardView (invoked by the layout manager): 
        public override RecyclerView.ViewHolder
            OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            // Inflate the CardView for the photo:
            View itemView = LayoutInflater.From(parent.Context).
                        Inflate(Resource.Layout.BlockedContactListItemLayout, parent, false);

            BlockedContactListHolder vh = new BlockedContactListHolder(itemView, OnClick);

         

            return vh;
        }

        public override void
            OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            BlockedContactListHolder vh = holder as BlockedContactListHolder;
            var item = originalContacts[position];
            if (item != null)
            {
                var BlockListContact = Repositories.ContactRepository.GetContactbyUserId(Convert.ToInt32(item.ContactId));


                if (BlockListContact != null)
                {
                   vh.txtSenderName.Text = BlockListContact.screenName;
                 
                }
                if(BlockListContact != null && BlockListContact.contactPicUrl=="")
                {

                    CommonHelper.SetImageOnUIImageView(vh.contactPic, BlockListContact.contactPicUrl, Contextt, 400, 400);
                }
            }

            //ChatConversation RemoveContactModel = originalContacts[position];
            // var removeContactListEvent = new BlockedContactListListner(ChatConversation, Contextt);
            //  vh.AddContactButton.SetOnClickListener(addContactListEvent);
            // removeContactListEvent.ListReload += RefreshList;
        
            vh.removeContactButton.SetOnClickListener(new removeContactButtonClickListener(item, Contextt));
        }

        public void RefreshList(object o, int contactId)
        {
            if (contactId != 0)
            {
                this.ListReload(this, contactId);
            }
        }



        // Return the number of photos available in the photo album:
        public override int ItemCount
        {
            get { return originalContacts.Count; }
        }


        // Raise an event when the item-click takes place:
        void OnClick(int position)
        {
            //if (ItemClick != null)
            //    ItemClick(this, position);
        }

        //public void add(GroupMember Contact)
        //{
        //    originalContacts.Add(Contact);
        //}

        //public void remove(GroupMember Contact)
        //{
        //    originalContacts.Remove(Contact);
        //}
    }

  
    public class BlockedContactListHolder : RecyclerView.ViewHolder
    {
        public TextView txtSenderName { get; set; }
        public ImageView contactPic { get; set; }
         public Button removeContactButton { get; set; }
        public EventHandler<int> AddClick { get; set; }
        public Action<int> _listener;

        public BlockedContactListHolder(View itemView, Action<int> listener)
            : base(itemView)
        {
            txtSenderName = itemView.FindViewById<TextView>(Resource.Id.txtSenderName);
            contactPic = itemView.FindViewById<ImageView>(Resource.Id.contactPic);
            removeContactButton = itemView.FindViewById<Button>(Resource.Id.imgMessagelogo);
        }



        public void SetClickListener(Action<int> listener)
        {
            _listener = listener;
            // ItemView.Click += HandleClick;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (ItemView != null)
            {
                //  ItemView.Click -= HandleClick;
            }
            _listener = null;
        }

        void HandleClick(object sender, int e)
        {
            if (_listener != null)
            {
                _listener(base.AdapterPosition);
            }
        }
    }

    public class removeContactButtonClickListener : Java.Lang.Object, View.IOnClickListener
    {
        private ChatConversation item;
        private Activity context;

        public removeContactButtonClickListener(ChatConversation item, Activity context)
        {
            this.item = item;

            this.context = context;
        }

        public void OnClick(View v)
        {
            Android.App.AlertDialog.Builder dialog = new Android.App.AlertDialog.Builder(context);
            Android.App.AlertDialog alert = dialog.Create();
            alert.SetTitle("UnBlock");
            alert.SetMessage("Do you want to UnBlock this user");
            alert.SetButton("OK", (c, ev) =>
            {

                RemoveFromBlackListAsync(Convert.ToInt64(item.ContactId));
            });
            alert.SetButton2("CANCEL", (c, ev) =>
            {
                alert.Dismiss();
            });
            alert.Show();
        }

        private async void RemoveFromBlackListAsync(long id)
        {
          
                CommonHelper.ShowProgressBar(context);

                var model = new userdetails
                {
                    BlockUserID = id
                };

                var result = await new SettingService().PostUnBlockUserInterest(model);
               if (result.Status == 1)
                {
                   
                    ChatConversationRepository.UpdateUnBlock(id);
                    CommonHelper.DismissProgressBar(context);
                    Toast.MakeText(context, result.Message, ToastLength.Long).Show();
                }
            else
            {
                CommonHelper.DismissProgressBar(context);
                Toast.MakeText(context, result.Message, ToastLength.Long).Show();
            }
            CommonHelper.DismissProgressBar(context);
        }
        }
    }



