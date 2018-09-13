using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Text;
using Android.Views;
using Android.Widget;
using InPowerApp.Common;
using InPowerApp.ListAdapter;
using InPowerApp.Model;
using InPowerApp.Repositories;
using InPowerApp.SignalR;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using PCL;
using PCL.Model;
using Refractored.Controls;
using Square.Picasso;
using static Android.Views.View;

namespace InPowerApp.Activities
{
    [Activity(Label = "GroupMessageActivity")]
    public class GroupMessageActivity : AppCompatActivity
    {
        LinearLayout ButtonLayout;
        bool loadList = true;
        PaginationModel paginationModel = new PaginationModel();
        SwipeRefreshLayout mySwipeRefreshLayout;
        RecyclerView mRecyclerView;
        RecyclerView.LayoutManager mLayoutManager;
        GroupMessageAdapter mAdapter;
        private Object thisLock = new Object();


        View Circleview;
      //  private CircleMenu circleMenu;
        Android.App.AlertDialog dialog = null;

        GroupMessageAdapter GroupchatMessageAdapter;
        GroupModel GroupObject;
        TextView txtChatSendMessage;

        Button btnSendButton, btnAttachment;

        ImageView btnCameraImage;
        List<AttachmentViewModel> lstAttachments;

        string AttachFilePath;
        string MediaType;
        public static readonly int PickImageId = 1000;
        protected static readonly int CAMERA_REQUEST = 1337;
        protected static readonly int PickvideoId = 1111;
        protected static readonly int videoRequest = 2222;

        Dictionary<DateTime, List<GroupMessage>> ListChatsCon;

        ChatSignalRServices _objChatSignalRService = new ChatSignalRServices();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                SetContentView(Resource.Layout.ChatGroupMessagelayout);


                var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.app_bar);
                SetSupportActionBar(toolbar);

                LinearLayout li_toolbarDetails = FindViewById<LinearLayout>(Resource.Id.li_toolbarDetails);
                CircleImageView IV_Grouplogo = FindViewById<CircleImageView>(Resource.Id.iv_customforGrouplogo);
                CircleImageView _IVarrow_back_white = FindViewById<CircleImageView>(Resource.Id.iv_arrow_back_white);
                TextView toolbar_title = FindViewById<TextView>(Resource.Id.toolbar_title);
                li_toolbarDetails.Click += Li_toolbarDetails_Click;
                IV_Grouplogo.Click += IV_Grouplogo_Click;
                toolbar_title.Click += Toolbar_title_Click;
                _IVarrow_back_white.Click += _IVarrow_back_white_Click;
                toolbar.Click += Toolbar_Click;
                  GroupObject = JsonConvert.DeserializeObject<GroupModel>(Intent.GetStringExtra("GroupObject"));
               
                if (GroupObject != null)
                {
                    toolbar_title.Text = GroupObject.GroupName;
                }

                if (!string.IsNullOrEmpty(GroupObject.GroupPictureUrl))
                {
                    Picasso.With(this)
                   .Load(GroupObject.GroupPictureUrl)
                  .Resize(100, 100)
                  .CenterCrop().Placeholder(Resource.Mipmap.grouplist)
                  .Into(IV_Grouplogo);
                }
                else
                {
                    IV_Grouplogo.Visibility = ViewStates.Invisible;
                }

                txtChatSendMessage = FindViewById<EditText>(Resource.Id.txtSendMessage);


                mySwipeRefreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.swiperefresh);
                var refreshListEvent = new SwipeRefreshLayoutGroupMessage(this);
                mySwipeRefreshLayout.SetOnRefreshListener(refreshListEvent);
                refreshListEvent.LoadMoreEvent += RefreshListEvent_LoadMoreEvent;

                mRecyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
                mLayoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                mRecyclerView.SetLayoutManager(mLayoutManager);

                btnAttachment = (Button)FindViewById(Resource.Id.btnextra_msg);
                btnSendButton = FindViewById<Button>(Resource.Id.btnsend_msg);
                btnCameraImage = FindViewById<ImageView>(Resource.Id.imgCamera_msg);
                ButtonLayout = FindViewById<LinearLayout>(Resource.Id.linearLayout3);
                txtChatSendMessage.TextChanged += TxtChatSendMessage_TextChanged;
                btnSendButton.Click += BtnSendButton_Click;
                btnAttachment.Click += BtnAttachment_Click;
                btnCameraImage.Click += BtnCameraImage_Click;

            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
            //loadGroupSignalR();
            //LoadServerMessagesUpto(DateTime.Now);
            //loadLocalMessage();

        }

        private void TxtChatSendMessage_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtChatSendMessage.Text == "")
            {
                ButtonLayout.Visibility = ViewStates.Gone;
            }
            else
            {
                ButtonLayout.Visibility = ViewStates.Visible;
            }
        }



        public void BtnCameraImage_Click(object sender, EventArgs e)
        {

            if (IsThereAnAppToTakePictures())
            {
                CommonHelper.CreateDirectoryForPictures();


            }
            TakeAPicture();
        }

       

        private void Toolbar_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(GroupDetailsActivity));
            intent.PutExtra("GroupObject", JsonConvert.SerializeObject(GroupObject));
            StartActivity(intent);
        }

        private void Toolbar_title_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(GroupDetailsActivity));
            intent.PutExtra("GroupObject", JsonConvert.SerializeObject(GroupObject));
            StartActivity(intent);
        }

        private void _IVarrow_back_white_Click(object sender, EventArgs e)
        {
            this.Finish();
        }

        private void IV_Grouplogo_Click(object sender, EventArgs e)
        {
            //Intent intent = new Intent(this, typeof(UserProfileDetailActivity));
            //intent.PutExtra("GroupObject", JsonConvert.SerializeObject(GroupObject));
            //StartActivity(intent);
        }

        private void Li_toolbarDetails_Click(object sender, EventArgs e)
        {
            //Intent intent = new Intent(this, typeof(UserProfileDetailActivity));
            //intent.PutExtra("GroupObject", JsonConvert.SerializeObject(GroupObject));
            //StartActivity(intent);
        }
        private void RefreshListEvent_LoadMoreEvent(object sender, bool loadCalled)
        {
            try
            {
                if (loadCalled)
                {
                    mySwipeRefreshLayout.Refreshing = false;
                    LoadLocalPreviousMessages();
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public void LoadLocalPreviousMessages()
        {
            try
            {
                loadList = false;
                paginationModel.SkipRecords += 30;

                int PreviousListCount = ListChatsCon.Values.Sum(list => list.Count);
                ListChatsCon = GroupRepository.GetGroupMessagesForPageIndex(paginationModel, GroupObject.GroupId);
                int CurrentListCount = ListChatsCon.Values.Sum(list => list.Count);
                if (ListChatsCon != null && ListChatsCon.Count > 0)
                {
                    mAdapter = new GroupMessageAdapter(this, ListChatsCon);
                    mRecyclerView.SetAdapter(mAdapter);
                    mAdapter.NotifyDataSetChanged();
                    mRecyclerView.ScrollToPosition(CurrentListCount - PreviousListCount - 2);
                    loadList = true;
                }

            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }
        private void BtnAttachment_Click(object sender, EventArgs e)
        {
            //var builder = new Android.App.AlertDialog.Builder(this);
            //Circleview = LayoutInflater.Inflate(Resource.Layout.ChatAttachmentLayoutPopup, null);

            //builder.SetView(Circleview);
            //circleMenu = (CircleMenu)Circleview.FindViewById(Resource.Id.circle_menu);

            //circleMenu.SetMainMenu(Color.ParseColor("#CDCDCD"), Resource.Mipmap.icon_menu, Resource.Mipmap.icon_cancel)
            //    .AddSubMenu(Color.ParseColor("#258CFF"), Resource.Mipmap.ic_action_picture)
            //    .AddSubMenu(Color.ParseColor("#30A400"), Resource.Mipmap.ic_action_camera)
            //    .AddSubMenu(Color.ParseColor("#FF4B32"), Resource.Mipmap.ic_action_video)
            //    .AddSubMenu(Color.ParseColor("#8A39FF"), Resource.Mipmap.ic_action_headphones)
            //     .AddSubMenu(Color.ParseColor("#FF6A00"), Resource.Mipmap.ic_action_place)
            //    .AddSubMenu(Color.ParseColor("#e2bf0b"), Resource.Mipmap.ic_action_chat)
            //    .SetOnMenuSelectedListener(this);
            //circleMenu.OpenMenu();

            //dialog = builder.Create();
            //dialog.Show();

            if (IsThereAnAppToTakePictures())
            {
                CommonHelper.CreateDirectoryForPictures();
            }
            Intent = new Intent();
            Intent.SetType("image/*");
            Intent.SetAction(Intent.ActionGetContent);
            StartActivityForResult(Intent.CreateChooser(Intent, "Select Picture"), PickImageId);


        }

        private async void loadGroupSignalR()
        {
            try
            {
                
                await _objChatSignalRService.Connect();
                _objChatSignalRService.OnGroupMessageReceived += _objChatSignalRService_OnGroupMessageReceived;
                _objChatSignalRService.OnGetGroupMessageUpdateStatusRecieved += _objChatSignalRService_OnGetGroupMessageUpdateStatusRecieved;
                _objChatSignalRService.OnGetReload += _objChatSignalRService_OnGetGroupReload;
                await _objChatSignalRService.ConnectUser("GroupChat");
                if (GroupObject != null)
                {
                    await _objChatSignalRService.Reload(GroupObject.GroupId.ToString());
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        private void _objChatSignalRService_OnGetGroupMessageUpdateStatusRecieved(object sender, GroupMessageStatusViewModel e)
        {
            try
            {
                if (e.GroupMessageId != 0)
                {
                    var savedMessages = GroupRepository.UpdateGroupMessageStatus(e);
                    RunOnUiThread(() =>
                      {
                          LoadLocalLatestMessages();
                      });
                }
            }
            catch (Java.Lang.Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }



        private void _objChatSignalRService_OnGetGroupReload(object sender, string e)
        {
            try
            {
                RunOnUiThread(() =>
                {
                    LoadServerMessagesUpto(DateTime.Now);

                });
            }
            catch (Exception eX)
            {
                Crashes.TrackError(eX);
            }

        }

        private async void _objChatSignalRService_OnGroupMessageReceived(object sender, GroupMessageResponseViewModel e)
        {
            try
            {
                if (e.MessageId != 0)
                {
                    var chatmessage = GroupRepository.CheckGroupChatMessage(e.MessageId, e.GroupId);
                    if (chatmessage == null)
                    {
                        GroupRepository.SaveGroupMessage(e,"Private");
                        RunOnUiThread(() =>
                        {
                            LoadLocalLatestMessages();
                        });
                        UpdateGroupChatMessage(e.SenderId.ToString(), e, "Private");

                    }
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.ToString(), ToastLength.Long).Show();
            }
        }

        public async void UpdateGroupChatMessage(string contactid, GroupMessageResponseViewModel model, string privatemsg)
        {
            try
            {
                GroupMessageStatusViewModel _model = new GroupMessageStatusViewModel();
                _model.GroupMessageId = model.MessageId;
                await _objChatSignalRService.SendGroupMessageUpdate(contactid, _model, privatemsg);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        //private async void LoadServerMessagesUpto(DateTime date)
        //{
        //    var unixTimeStamp = CommonHelper.DateTimeToUnixTimestamp(date.AddSeconds(-1));
        //    var result = await new GroupChatService().GetGroupMessagesUpto(GroupObject.GroupId, unixTimeStamp);
        //    if (result.Status == 1)
        //    {
        //        var GroupMessagelist = JsonConvert.DeserializeObject<List<GroupMessageResponseViewModel>>(result.Response.ToString());
        //        var savedItem = GroupRepository.SaveGroupMessageFromList(GroupMessagelist);
        //    }
        //}

        private async void LoadServerMessagesUpto(DateTime date)
        {
            var unixTimeStamp = CommonHelper.DateTimeToUnixTimestamp(date.AddSeconds(-1));
            try
            {
                //BTProgressHUD.Show("Loading messages..", maskType: ProgressHUD.MaskType.Black);
                await Task.Run(async () =>
                {
                    var result = await new GroupChatService().GetGroupMessagesUpto(GroupObject.GroupId, unixTimeStamp);
                    if (result.Status == 1)
                    {
                        var GroupMessagelist = JsonConvert.DeserializeObject<List<GroupMessageResponseViewModel>>(result.Response.ToString());

                        var savedMessages = GroupRepository.SaveGroupMessageFromList(GroupMessagelist);

                        RunOnUiThread(() =>
                        {
                            LoadLocalLatestMessages();
                        });
                    }
                });
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //private async void loadLocalMessage()
        //{
        //    var listValues = GroupRepository.GetGroupMessagesForPageIndex(GroupObject.GroupId);

        //    if (listValues != null)
        //    {
        //        GroupchatMessageAdapter = new GroupMessageAdapter(this,ListChatsCon);
        //        mRecyclerView.SetAdapter(GroupchatMessageAdapter);
        //        GroupchatMessageAdapter.NotifyDataSetChanged();
        //        mRecyclerView.ScrollToPosition(GroupchatMessageAdapter.ItemCount - 1);
        //    }
        //}

        public void LoadLocalLatestMessages()
        {
            try
            {
                loadList = false;

                if (GroupObject != null)
                {
                    ListChatsCon = GroupRepository.GetGroupMessagesForPageIndex(paginationModel, GroupObject.GroupId);
                  
                    if (ListChatsCon != null && ListChatsCon.Count > 0)
                    {
                        mAdapter = new GroupMessageAdapter(this, ListChatsCon);
                        mRecyclerView.SetAdapter(mAdapter);
                        mAdapter.NotifyDataSetChanged();
                        mRecyclerView.ScrollToPosition(mAdapter.ItemCount - 1);
                        loadList = true;
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }



        private async void BtnSendButton_Click(object sender, EventArgs e)
        {
            ButtonSendChatMessage();
            //if (string.IsNullOrEmpty(txtChatSendMessage.Text))
            //{
            //    Toast.MakeText(this, "Message Text Not Blank", ToastLength.Long).Show();

            //    return;
            //}
            //txtChatSendMessage.Text = txtChatSendMessage.Text.Trim(' ', '\n');


            //var GroupTempSaveMessage = new GroupMessageResponseViewModel
            //{
            //    GroupId = GroupObject.GroupId,
            //    Message = txtChatSendMessage.Text,
            //    Attachments = lstAttachments ?? new List<AttachmentViewModel>(),
            //    SenderId = CommonHelper.GetUserId(),
            //    MessageTime = DateTime.Now.ToUniversalTime()

            //};
            //var message = GroupRepository.SaveGroupMessage(GroupTempSaveMessage);
            //var GroupSend = new GroupMessageRequestViewModel
            //{
            //    GroupId = GroupObject.GroupId,
            //    Message = txtChatSendMessage.Text,
            //    Attachments = lstAttachments ?? new List<AttachmentViewModel>(),
            //    MobileDatabaseId = message.id

            //};

            //GroupchatMessageAdapter.add(message);
            ////mRecyclerView.SmoothScrollToPosition(mAdapter.ItemCount - 1);
            //mRecyclerView.ScrollToPosition(GroupchatMessageAdapter.ItemCount - 1);
            //GroupchatMessageAdapter.NotifyDataSetChanged();
            //txtChatSendMessage.Text = "";
            //try
            //{

            //    var result = await new GroupChatService().PostGroupMessageService(GroupSend);


            //    if (result.Status == 1)
            //    {
            //        var GroupMessagelist = JsonConvert.DeserializeObject<GroupMessageResponseViewModel>(result.Response.ToString());
            //        Console.WriteLine("posted successfully in grp");
            //        GroupRepository.UpdateMicronetLastMessage(GroupMessagelist.GroupId, GroupMessagelist.Message);


            //        var savedItem = GroupRepository.UpdateGroupMessage(GroupMessagelist);
            //        await _objChatSignalRService.SendGroup(GroupMessagelist, GroupMessagelist.GroupId);


            //        loadLocalMessage();
            //    }
            //    else
            //    {
            //        Console.WriteLine("post failed in grp");
            //    }
            //}
            //catch (Exception ex)
            //{


            //}
        }


        public async void ButtonSendChatMessage()
        {
            try
            {
                if (!string.IsNullOrEmpty(txtChatSendMessage.Text) || lstAttachments.Count > 0)
                {

                    txtChatSendMessage.Text = txtChatSendMessage.Text.Trim(' ', '\n');

                    var GroupTempSaveMessage = new GroupMessageResponseViewModel
                    {
                        GroupId = GroupObject.GroupId,
                        Message = txtChatSendMessage.Text,
                        Attachments = lstAttachments ?? new List<AttachmentViewModel>(),
                        SenderId = CommonHelper.GetUserId(),
                        MessageTime = DateTime.Now.ToUniversalTime()

                    };


                    var message = GroupRepository.SaveGroupMessage(GroupTempSaveMessage,"Private");
                    var GroupSend = new GroupMessageRequestViewModel
                    {
                        GroupId = GroupObject.GroupId,
                        Message = txtChatSendMessage.Text,
                        Attachments = lstAttachments ?? new List<AttachmentViewModel>(),
                        MobileDatabaseId = message.id

                    };

                    lstAttachments = new List<AttachmentViewModel>();
                    txtChatSendMessage.Text = "";
                    try
                    {
                        var result = await new GroupChatService().PostGroupMessageService(GroupSend);

                        if (result.Status == 1)
                        {
                            var GroupMessagelist = JsonConvert.DeserializeObject<GroupMessageResponseViewModel>(result.Response.ToString());
                            Console.WriteLine("posted successfully in grp");

                            var chatConversation = ChatConversationRepository.GetConversationbyGroupId(GroupMessagelist.GroupId);
                            ChatConversationRepository.UpdateChatLastMessage(chatConversation.id, GroupMessagelist.Message,GroupMessagelist.SenderProfileName);
                            var savedItem = GroupRepository.UpdateGroupMessage(GroupMessagelist);
                            await _objChatSignalRService.SendGroup(GroupMessagelist, GroupMessagelist.GroupId);

                            LoadLocalLatestMessages();
                        }
                        else
                        {
                            Console.WriteLine("post failed in grp");
                        }

                    }

                    catch (Exception ex)
                    {
                        Crashes.TrackError(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }

        }


        public override void OnBackPressed()
        {
            try
            {
                if (FragmentManager.BackStackEntryCount != 0)
                {
                    FragmentManager.PopBackStack();// fragmentManager.popBackStack();
                }
                else
                {

                    base.OnBackPressed();
                    this.Finish();
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
                Finish();

            return base.OnOptionsItemSelected(item);
        }

        protected async override void OnStop()
        {
            // await _objChatSignalRService.disconnectUser();
            base.OnStop();
        }

        //public void OnMenuSelected(int index)
        //{
        //    if (index == 0)
        //    {
        //        if (IsThereAnAppToTakePictures())
        //        {
        //            CommonHelper.CreateDirectoryForPictures();
        //        }
        //        Intent = new Intent();
        //        Intent.SetType("image/*");
        //        Intent.SetAction(Intent.ActionGetContent);
        //        StartActivityForResult(Intent.CreateChooser(Intent, "Select Picture"), PickImageId);
        //    }

        //    if (index == 1)
        //    {

        //        if (IsThereAnAppToTakePictures())
        //        {
        //            CommonHelper.CreateDirectoryForPictures();


        //        }
        //        TakeAPicture();
        //    }

        //    this.circleMenu.Dispose();
        //    Circleview.Dispose();
        //    dialog.Hide();
        //}



        private bool IsThereAnAppToTakePictures()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            IList<ResolveInfo> availableActivities = this.PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
            return availableActivities != null && availableActivities.Count > 0;
        }
        public void TakeAPicture()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            BitmapHelper._file = new Java.IO.File(BitmapHelper._dir, System.String.Format("Attach_{0}.jpg", Guid.NewGuid()));
            intent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(BitmapHelper._file));
            StartActivityForResult(intent, CAMERA_REQUEST);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            try
            {
                string filePath;
                base.OnActivityResult(requestCode, resultCode, data);
                if ((requestCode == PickImageId) && (resultCode == Result.Ok) && (data != null))
                {
                    if (data.Data != null)
                    {
                        Android.Net.Uri uri = data.Data;

                        filePath = CommonHelper.GetRealPathFromURI(this, data.Data);
                        int height = 600;
                        int width = 600;
                        BitmapHelper._file = new Java.IO.File(filePath);




                        BitmapHelper.bitmap = BitmapHelper._file.Path.LoadAndResizeBitmap(width, height);
                        try
                        {
                            using (var os = new System.IO.FileStream(System.IO.Path.Combine(BitmapHelper._dir.ToString(), String.Format("Attach_{0}.jpg", Guid.NewGuid())), System.IO.FileMode.CreateNew))
                            {
                                BitmapHelper.bitmap.Compress(Bitmap.CompressFormat.Jpeg, 95, os);
                                AttachFilePath = os.Name;
                            }
                        }
                        catch (Exception e)
                        {
                            Toast.MakeText(this, e.Message, ToastLength.Long).Show();
                        }

                        MediaType = "Photo";
                    }
                }
                if ((requestCode == CAMERA_REQUEST))
                {
                    Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
                    Android.Net.Uri contentUri = Android.Net.Uri.FromFile(BitmapHelper._file);
                    mediaScanIntent.SetData(contentUri);
                    // AttachImageView.SetImageURI(contentUri);
                    int height = 600;
                    int width = 600;

                    BitmapHelper.bitmap = BitmapHelper._file.Path.LoadAndResizeBitmap(width, height);
                    try
                    {
                        using (var os = new System.IO.FileStream(System.IO.Path.Combine(BitmapHelper._dir.ToString(), String.Format("Attach_{0}.jpg", Guid.NewGuid())), System.IO.FileMode.CreateNew))
                        {
                            BitmapHelper.bitmap.Compress(Bitmap.CompressFormat.Jpeg, 95, os);
                            AttachFilePath = os.Name;
                        }
                    }
                    catch (Exception e)
                    {
                        Toast.MakeText(this, e.Message, ToastLength.Long).Show();
                    }

                    MediaType = "Photo";
                }
                SendMedia(AttachFilePath, MediaType);
                //  
            }
            catch (Exception e)
            {

                Crashes.TrackError(e);
            }

        }
        public async void SendMedia(string filePath, string mediaType)
        {
            var mediaName = System.IO.Path.GetFileName(filePath); //AWSUploader.SetMediaName (mediaType);
            var url = "";
            try
            {
                // BTProgressHUD.Show("Processing media..", maskType: ProgressHUD.MaskType.Black);
                if (mediaType == "Photo")
                    await AWSUploader.AWSUploadImage(filePath, mediaName);
                else
                    await AWSUploader.AWSUploadAudioVideo(filePath, mediaName, mediaType);
                url = AWSUploader.GetMediaUrl(mediaType) + mediaName;



                try
                {
                    if (string.IsNullOrEmpty(url))
                    {
                        // AlertBox.Create("Upload", "File upload failed, Please check your internet connection", null);

                        return;
                    }
                    else
                    {
                        lstAttachments = new List<AttachmentViewModel>();
                        var attachment = new AttachmentViewModel();
                        attachment.Type = mediaType;
                        attachment.Url = url;
                        lstAttachments.Add(attachment);
                        ButtonSendChatMessage();
                    }
                }
                catch (Java.Lang.Exception e)
                {
                    Crashes.TrackError(e);
                }
            }
            catch (Java.Lang.Exception e)
            {

                Crashes.TrackError(e);
            }
        }



        protected async override void OnResume()
        {
            base.OnResume();
            await Task.Run(async () =>
            {
                Loadonresume();
            });
        }

        private void Loadonresume()
        {
            try
            {
                // Create your application here
                GroupObject = JsonConvert.DeserializeObject<GroupModel>(Intent.GetStringExtra("GroupObject"));
                SupportActionBar.Title = GroupObject.GroupName;
                if (InternetConnectivityModel.CheckConnection(this))
                {
                    loadGroupSignalR();
                }
                RunOnUiThread(() =>
                {
                    paginationModel.SkipRecords = 0;
                    paginationModel.TakeRecords = 30;
                    LoadLocalLatestMessages();
                });

                if (InternetConnectivityModel.CheckConnection(this))
                {
                    LoadServerMessagesUpto(DateTime.UtcNow);
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }
    }
}