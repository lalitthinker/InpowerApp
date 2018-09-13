using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Support.V4.App;
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
using PCL.Model;
using PCL.Service;
using Refractored.Controls;
using Square.Picasso;

namespace InPowerApp.Activities
{
    [Activity(Label = "PrivateMessageActivity")]
    public class PrivateMessageActivity : AppCompatActivity
    {

        LinearLayout ButtonLayout;
        bool loadList = true;
        PaginationModel paginationModel = new PaginationModel();
        SwipeRefreshLayout mySwipeRefreshLayout;
        RecyclerView mRecyclerView;
        RecyclerView.LayoutManager mLayoutManager;
        PrivateMessgeAdapter mAdapter;
        public List<ChatMessage> mPrivateMessge;
        View Circleview;
        Android.App.AlertDialog dialog = null;
        ContactViewModel ContactObject;
        EditText txtChatSendMessage;
        long contactId;
        string ContactName = "";
        //ListView ChatMessage;
        List<AttachmentViewModel> lstAttachments;
        Button btnSendButton, btnAttachment;
        ImageView imgCamera_msg;
        public ChatConversation chatConversation { get; set; }
        protected const int REQUEST_LOCATION = 0x1;
        string AttachFilePath;
        ImageView AttachImageView;
        string MediaType;
        public static readonly int PickImageId = 1000;
        protected static readonly int CAMERA_REQUEST = 1337;
        protected static readonly int PickvideoId = 1111;
        protected static readonly int videoRequest = 2222;
        //ListView msgListView;
        Dictionary<DateTime, List<ChatMessage>> ListChatsCon;
        ChatSignalRServices _objChatSignalRService = new ChatSignalRServices();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
                TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;

                SetContentView(Resource.Layout.ChatMessagelayout);
                var requiredPermissions = new String[]
         {

                    Manifest.Permission.Internet,
                     Manifest.Permission.WriteExternalStorage,
                      Manifest.Permission.ReadExternalStorage,
                      Manifest.Permission.Camera,
                    Manifest.Permission.ReadContacts
         };
                ActivityCompat.RequestPermissions(this, requiredPermissions, REQUEST_LOCATION);

                var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.app_bar);
                SetSupportActionBar(toolbar);
                //SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                //SupportActionBar.SetHomeButtonEnabled(true);
                //SupportActionBar.SetDisplayUseLogoEnabled(true);
                ////SupportActionBar.SetIcon(Resource.Drawable.logo512_512);
                LinearLayout li_toolbarDetails = FindViewById<LinearLayout>(Resource.Id.li_toolbarDetails);
                CircleImageView IV_Userlogo = FindViewById<CircleImageView>(Resource.Id.iv_customforUserlogo);
                CircleImageView _IVarrow_back_white = FindViewById<CircleImageView>(Resource.Id.iv_arrow_back_white);
                TextView toolbar_title = FindViewById<TextView>(Resource.Id.toolbar_title);
                li_toolbarDetails.Click += Li_toolbarDetails_Click;
                IV_Userlogo.Click += IV_Userlogo_Click;
                toolbar_title.Click += Toolbar_title_Click;
                _IVarrow_back_white.Click += _IVarrow_back_white_Click;

              
              


                // Create your application here
                ContactObject = JsonConvert.DeserializeObject<ContactViewModel>(Intent.GetStringExtra("ContactObject"));
                if (ContactObject != null)
                {
                    ContactName = ContactRepository.GetContactbyUserId(ContactObject.ContactId).name;
                    contactId = ContactObject.ContactId;
                    toolbar_title.Text = ContactName;
                }

                if (ContactObject.ProfileImageUrl != null && ContactObject.ProfileImageUrl != "")
                {

                    Picasso.With(this)
                   .Load(ContactObject.ProfileImageUrl)
                  .Resize(100, 100)
                  .CenterCrop().Placeholder(Resource.Drawable.default_profile)
                  .Into(IV_Userlogo);
                }
                else
                {
                    IV_Userlogo.SetBackgroundResource(Resource.Drawable.default_profile);
                }

                txtChatSendMessage = FindViewById<EditText>(Resource.Id.txtSendMessage);
                btnAttachment = (Button)FindViewById(Resource.Id.btnextra_msg);
                btnSendButton = FindViewById<Button>(Resource.Id.btnsend_msg);
                ButtonLayout = FindViewById<LinearLayout>(Resource.Id.linearLayout3); 
                txtChatSendMessage.TextChanged += TxtChatSendMessage_TextChanged;

                mySwipeRefreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.swiperefresh);
                var refreshListEvent = new SwipeRefreshLayoutPrivateMessage(this);
                mySwipeRefreshLayout.SetOnRefreshListener(refreshListEvent);
                refreshListEvent.LoadMoreEvent += RefreshListEvent_LoadMoreEvent;

                mRecyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
                if (mRecyclerView != null)
                {
                    mRecyclerView.HasFixedSize = true;
                    var layoutManager = new LinearLayoutManager(this);
                    mRecyclerView.SetLayoutManager(layoutManager);
                }

                imgCamera_msg = FindViewById<ImageView>(Resource.Id.imgCamera_msg);

                chatConversation = ChatConversationRepository.GetConversationbyContactId(ContactObject.ContactId);

                btnSendButton.Click += btnSendButton_Click;
                btnAttachment.Click += BtnAttachment_Click;
                imgCamera_msg.Click += ImgCamera_msg_Click;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
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

        public void CloseMenu()
        {
            try
            {
                dialog.Hide();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        private static void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs unobservedTaskExceptionEventArgs)
        {
            var newExc = new Exception("TaskSchedulerOnUnobservedTaskException", unobservedTaskExceptionEventArgs.Exception);
            LogUnhandledException(newExc);
        }

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            var newExc = new Exception("CurrentDomainOnUnhandledException", unhandledExceptionEventArgs.ExceptionObject as Exception);
            LogUnhandledException(newExc);
        }

        internal static void LogUnhandledException(Exception exception)
        {
            try
            {
                // Log to Android Device Logging.
                Android.Util.Log.Error("Crash Report", exception.Message);
                Crashes.TrackError(exception);
            }
            catch
            {
                // just suppress any error logging exceptions
            }
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

        private void Toolbar_title_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(UserProfileDetailActivity));

            intent.PutExtra("ContactObject", JsonConvert.SerializeObject(ContactObject));
            intent.PutExtra("chatConversation", JsonConvert.SerializeObject(chatConversation));
            StartActivity(intent);
        }

        private void _IVarrow_back_white_Click(object sender, EventArgs e)
        {
            this.Finish();
        }

        private void IV_Userlogo_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(UserProfileDetailActivity));

            intent.PutExtra("ContactObject", JsonConvert.SerializeObject(ContactObject));
            intent.PutExtra("chatConversation", JsonConvert.SerializeObject(chatConversation));
            StartActivity(intent);
        }

        private void Li_toolbarDetails_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(UserProfileDetailActivity));

            intent.PutExtra("ContactObject", JsonConvert.SerializeObject(ContactObject));
            intent.PutExtra("chatConversation", JsonConvert.SerializeObject(chatConversation));
            StartActivity(intent);
        }

        private void ImgCamera_msg_Click(object sender, EventArgs e)
        {
            if (IsThereAnAppToTakePictures())
            {
                CommonHelper.CreateDirectoryForPictures();


            }
            TakeAPicture();
        }

        private void BtnAttachment_Click(object sender, EventArgs e)
        {

            if (IsThereAnAppToTakePictures())
            {
                CommonHelper.CreateDirectoryForPictures();
            }
            Intent = new Intent();
            Intent.SetType("image/*");
            Intent.SetAction(Intent.ActionGetContent);
            StartActivityForResult(Intent.CreateChooser(Intent, "Select Picture"), PickImageId);


        }



        private async void loadSignalR()
        {
            try
            {
                await _objChatSignalRService.Connect();
                _objChatSignalRService.OnMessageReceived += _objChatSignalRService_OnMessageReceived;
                _objChatSignalRService.OnGetUpdateStatusRecieved += _objChatSignalRService_OnGetUpdateStatus;
                _objChatSignalRService.OnGetReload += _objChatSignalRService_OnGetReload;
                await _objChatSignalRService.ConnectUser("PrivateChat");
                if (chatConversation != null)
                {
                        await _objChatSignalRService.Reload(chatConversation.ContactId.ToString());
                }



            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }


        }

        private void _objChatSignalRService_OnGetReload(object sender, string e)
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

        private void _objChatSignalRService_OnGetUpdateStatus(object sender, ChatMessageViewModel e)
        {
            try
            {
                if (e.ChatMessageId != 0)
                {
                    lock (thisLock)
                    {
                        var chatmessage = ChatMessageRepository.CheckMessage(e.ChatMessageId);
                        if (chatmessage != null)
                        {
                            var savedMessages = ChatMessageRepository.updateChatMessage(e);

                            RunOnUiThread(() =>
                            {
                                LoadLocalLatestMessages();

                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private Object thisLock = new Object();
        private async void _objChatSignalRService_OnMessageReceived(object sender, ChatMessageViewModel e)
        {
            try
            {
                if (e.ChatMessageId != 0)
                {
                    lock (thisLock)
                    {
                        var chatmessage = ChatMessageRepository.CheckMessage(e.ChatMessageId);
                        if (chatmessage == null)
                        {
                            if (chatConversation.ChatId == e.ChatId)
                            {
                                e.IsRead = true;
                            }
                            var savedMessages = ChatMessageRepository.SaveChatMessage(e, e.ChatId);
                            RunOnUiThread(() =>
                            {
                                LoadLocalLatestMessages();
                            });

                            UpdateChatMessage(chatConversation.ContactId.ToString(), e, "Private");
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        public async void UpdateChatMessage(string contactid, ChatMessageViewModel model, string privatemsg)
        {
            try
            {
                await _objChatSignalRService.SendUpdate(chatConversation.ContactId.ToString(), model, privatemsg);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        public void LoadLocalLatestMessages()
        {
            try
            {
                loadList = false;

                if (chatConversation != null)
                {
                    ListChatsCon = ChatMessageRepository.GetChatMessagesForPageIndex(paginationModel, chatConversation.ChatId);

                    if (ListChatsCon != null && ListChatsCon.Count > 0)
                    {
                        mAdapter = new PrivateMessgeAdapter(this, ListChatsCon);
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

        public void LoadLocalPreviousMessages()
        {
            try
            {
                loadList = false;
                paginationModel.SkipRecords += 30;
                if (chatConversation != null)
                {
                    int PreviousListCount = ListChatsCon.Values.Sum(list => list.Count);
                    ListChatsCon = ChatMessageRepository.GetChatMessagesForPageIndex(paginationModel, chatConversation.ChatId);
                    int CurrentListCount = ListChatsCon.Values.Sum(list => list.Count);
                    if (ListChatsCon != null && ListChatsCon.Count > 0)
                    {
                        mAdapter = new PrivateMessgeAdapter(this, ListChatsCon);
                        mRecyclerView.SetAdapter(mAdapter);
                        mAdapter.NotifyDataSetChanged();
                        mRecyclerView.ScrollToPosition(CurrentListCount - PreviousListCount - 2);
                        loadList = true;
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }
        private void btnSendButton_Click(object sender, EventArgs e)
        {
            ButtonSendChatMessage();
        }

        private async void LoadServerMessagesUpto(DateTime date)
        {
            var unixTimeStamp = CommonHelper.DateTimeToUnixTimestamp(date.AddSeconds(-1));
            try
            {
                //BTProgressHUD.Show("Loading messages..", maskType: ProgressHUD.MaskType.Black);
                await Task.Run(async () =>
                {
                    var result = await new ChatService().GetChatMessagesUptoId(ContactObject.ContactId, unixTimeStamp);
                    if (result.Status == 1)
                    {
                        var chatMessagelist = JsonConvert.DeserializeObject<List<ChatMessageViewModel>>(result.Response.ToString());

                        var savedMessages = ChatMessageRepository.SaveChatMessages(chatMessagelist, chatConversation.ChatId);
                       
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
        public class PostChatResponseViewModel
        {
            public ChatModel Chat { get; set; }
            public ChatMessageViewModel ChatMessage { get; set; }
        }

        public async void ButtonSendChatMessage()
        {
            try
            {
                if (!string.IsNullOrEmpty(txtChatSendMessage.Text) || lstAttachments.Count > 0)
                {

                    txtChatSendMessage.Text = txtChatSendMessage.Text.Trim(' ', '\n');

                    var chatModel = new ChatMessageViewModel
                    {
                        Attachments = lstAttachments ?? new List<AttachmentViewModel>(),
                        ContactId = contactId,
                        Message = txtChatSendMessage.Text,
                        IsRead = false,
                        IsRecieved = false,
                        IsSend = false,
                        MessageTime = DateTime.Now.ToUniversalTime()
                    };


                    chatConversation = ChatConversationRepository.GetConversationbyContactId(contactId);
                    if (chatConversation != null)
                    {
                        var message = ChatMessageRepository.SaveChatMessage(chatModel, chatConversation.ChatId);
                        chatModel.MobiledatabaseId = message.id;
                        chatModel.ChatId = chatConversation.ChatId;
                        chatModel.IsRead = false;
                        chatModel.IsRecieved = false;
                        chatModel.IsSend = false;
                        mAdapter.add(message);
                        mAdapter.NotifyDataSetChanged();
                        mRecyclerView.ScrollToPosition(mAdapter.ItemCount - 1);
                        txtChatSendMessage.Text = "";
                    }
                    lstAttachments = new List<AttachmentViewModel>();
                    txtChatSendMessage.Text = "";
                    try
                    {
                        var result = await new ChatService().PostChat(chatModel);

                        if (result.Status == 2 || result.Status == 1)
                        {
                            var ChatResponse = JsonConvert.DeserializeObject<PostChatResponseViewModel>(result.Response.ToString());

                            if (chatConversation == null)
                            {
                                ChatConversationRepository.SaveConverstionNewFromServer(ChatResponse.Chat);
                                chatConversation = ChatConversationRepository.GetConversationbyContactId(contactId);

                                ChatConversationRepository.UpdateChatLastMessage(chatConversation.id, ChatResponse.ChatMessage.Message,"");

                                var savedMessages = ChatMessageRepository.SaveChatMessage(ChatResponse.ChatMessage, chatConversation.ChatId);

                                LoadLocalLatestMessages();
                                txtChatSendMessage.Text = "";
                                Console.WriteLine("CHAT POSTED : " + result);
                            }
                            else
                            {
                                chatConversation = ChatConversationRepository.GetConversationbyContactId(contactId);

                                ChatConversationRepository.UpdateChatLastMessage(chatConversation.id, ChatResponse.ChatMessage.Message,"");

                                var savedMessages = ChatMessageRepository.updateChatMessage(ChatResponse.ChatMessage);
                                LoadLocalLatestMessages();
                                await _objChatSignalRService.Send(chatConversation.ContactId.ToString(), ChatResponse.ChatMessage);

                            }
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
                this.Finish();

            return base.OnOptionsItemSelected(item);
        }



        private bool IsThereAnAppToTakePictures()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            IList<ResolveInfo> availableActivities = this.PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
            return availableActivities != null && availableActivities.Count > 0;
        }
        private void TakeAPicture()
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

        protected async override void OnStop()
        {
           // await _objChatSignalRService.disconnectUser();
            base.OnStop();
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
                ContactObject = JsonConvert.DeserializeObject<ContactViewModel>(Intent.GetStringExtra("ContactObject"));
                ContactName = ContactRepository.GetContactbyUserId(Convert.ToInt64(ContactObject.ContactId)).name;
                contactId = ContactObject.ContactId;
                if (InternetConnectivityModel.CheckConnection(this))
                {
                    loadSignalR();
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