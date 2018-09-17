using Foundation;
using System;
using UIKit;
using InPowerIOS.Model;
using System.Collections.Generic;
using static InPowerIOS.Chats.ChatViewContarollerSource;
using InPowerIOS.NewChat;
using InPowerIOS.Chats;
using PCL.Model;
using InPowerIOS.SignalR;
using InPowerIOS.Repositories;
using InPowerIOS.Models;
using CoreGraphics;
using System.Drawing;
using InPowerApp.Model;
using InPowerIOS.Common;
using System.Threading.Tasks;
using PCL.Service;
using Newtonsoft.Json;
using BigTed;
using PCL;
using System.Linq;
using static InPowerIOS.Chats.GroupChatViewController;

namespace InPowerIOS
{
    public partial class GroupChatListViewController : UIViewController
    {
   
    
        ChatSignalRServices _objChatSignalRService = new ChatSignalRServices();
        PaginationModel paginationModel = new PaginationModel();
        public GroupModel GroupObject { get; set; }
        string ContactName = "";
        bool loadList = true;
        Dictionary<DateTime, List<GroupMessage>> ListChatsCon;

        List<AttachmentViewModel> lstAttachments;

        GroupChatListSource chatSource;

        List<ListItem> consolidatedList;

        UIImagePickerController imagePicker;
        UIImage PhotoCapture;
        string documentsDirectory, filePath;
        string mediaType = "Photo";
        UIButton cameraButton;
        bool useRefreshControl = false;  
        UIRefreshControl RefreshControl;  

        public GroupChatListViewController(IntPtr handle) : base(handle)
        {
        }

        public override  void ViewDidLoad()
        {
            base.ViewDidLoad();

            Xamarin.IQKeyboardManager.SharedManager.EnableAutoToolbar = false;
            Xamarin.IQKeyboardManager.SharedManager.ShouldResignOnTouchOutside = true;
            Xamarin.IQKeyboardManager.SharedManager.ShouldToolbarUsesTextFieldTintColor = true;
            Xamarin.IQKeyboardManager.SharedManager.KeyboardDistanceFromTextField = 000f;
   
            ScrollToBottom(false);

            imagePicker = new UIImagePickerController();
            imagePicker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
            imagePicker.MediaTypes = UIImagePickerController.AvailableMediaTypes(UIImagePickerControllerSourceType.PhotoLibrary);
            imagePicker.FinishedPickingMedia += Handle_FinishedPickingMedia;
            imagePicker.Canceled += Handle_Canceled;

            getGroupDetails();
           
            Loadonresume();
            AddRefreshControl();

            var g = new UITapGestureRecognizer(() => View.EndEditing(false));
            g.CancelsTouchesInView = true;;
            View.AddGestureRecognizer(g);
        }
        public override void ViewWillAppear(bool animated)
        {
            //base.ViewWillAppear(animated);
            //Task.Run(() =>
            //{
            //    Loadonresume();
            //});
        }
        private void Loadonresume()
        {
            try
            {
             
                if (InternetConnectivityModel.CheckConnection())
                {
                    loadGroupSignalR();
                }
               
                if (InternetConnectivityModel.CheckConnection())
                {
                    LoadServerMessagesUpto(DateTime.UtcNow);
                }
                else
                {
                    InvokeOnMainThread(() =>
                    {
                        paginationModel.SkipRecords = 0;
                        paginationModel.TakeRecords = 30;
                        LoadLocalLatestMessages();
                    });

                }
            }
            catch (Exception e)
            {
               // Crashes.TrackError(e);
            }
        }
        protected async void Handle_FinishedPickingMedia(object sender, UIImagePickerMediaPickedEventArgs e)
        {
            // determine what was selected, video or image
            bool isImage = false;
            switch (e.Info[UIImagePickerController.MediaType].ToString())
            {
                case "public.image":
                    Console.WriteLine("Image selected");
                    isImage = true;
                    break;
                case "public.video":
                    Console.WriteLine("Video selected");
                    break;
            }

            // get common info (shared between images and video)

            var referenceURL = (NSUrl)e.Info.ValueForKey(new NSString("UIImagePickerControllerImageURL"));


            //  stringTaskCompletionSource.SetResult(url.Path);
            // NSUrl referenceURL = e.Info[new NSString("UIImagePickerControllerReferenceURL")] as NSUrl;
            if (referenceURL != null)
                Console.WriteLine("Url:" + referenceURL.Path.ToString());

            // if it was an image, get the other image info
            if (isImage)
            {

                // get the original image
                string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                UIImage originalImage = e.Info[UIImagePickerController.OriginalImage] as UIImage;
                NSData data = originalImage.AsJPEG();
                filePath = referenceURL.AbsoluteString;

                if (originalImage != null)
                {
                    // do something with the image
                    Console.WriteLine("got the original image");
                    // ivUserProfilePic.Image = originalImage; // display
                }


                // This bit of code saves to the application's Documents directory, doesn't save metadata

                documentsDirectory = Environment.GetFolderPath
                                              (Environment.SpecialFolder.Personal);
                string imagename = "myAttachmentTemp" + DateTime.UtcNow.ToString("ddMMyyyyhhmmss") + ".jpg";
                filePath = System.IO.Path.Combine(documentsDirectory, imagename);
                NSData imgData = originalImage.AsJPEG();
                NSError err = null;

                if (imgData.Save(filePath, false, out err))
                {
                    Console.WriteLine("saved as " + filePath);
                }
                else
                {
                    Console.WriteLine("NOT saved as" + filePath + " because" + err.LocalizedDescription);
                }
                SendMedia(filePath, "Photo");

            }
            else
            { // if it's a video
              // get video url
                NSUrl mediaURL = e.Info[UIImagePickerController.MediaURL] as NSUrl;
                if (mediaURL != null)
                {
                    Console.WriteLine(mediaURL.ToString());
                }
            }
            // dismiss the picker
            imagePicker.DismissModalViewController(true);
        }

        void Handle_Canceled(object sender, EventArgs e)
        {
            imagePicker.DismissModalViewController(true);
        }

        public async void LoadLocalLatestMessages()
        {
            try
            {
                loadList = false;

                if (GroupObject != null)
                {
                    ListChatsCon = GroupRepository.GetGroupMessagesForPageIndex(paginationModel, GroupObject.GroupId);

                    if (ListChatsCon != null && ListChatsCon.Count > 0)
                    {
                        consolidatedList = new List<ListItem>();

                        foreach (var itemm in ListChatsCon)
                        {
                            DateItem dateItem = new DateItem();
                            dateItem.setDate(itemm.Key.ToShortDateString());
                            consolidatedList.Add(dateItem);

                            foreach (var general in itemm.Value)
                            {
                                GeneralGroupItem generalItem = new GeneralGroupItem();
                                generalItem.setChatMessagearray(general);
                                consolidatedList.Add(generalItem);
                            }
                        }


                        chatSource = new GroupChatListSource(consolidatedList);
                        tblChatList.Source = chatSource;
                      
                       
                        tblChatList.Add(RefreshControl);
                        tblChatList.ReloadData();
                        ScrollToBottom(true);
                    }
                }
            }
            catch (Exception e)
            {
                //Crashes.TrackError(e);
            }
        }

        async Task RefreshAsync()  
{  
    // only activate the refresh control if the feature is available  
    if (useRefreshControl)  
        RefreshControl.BeginRefreshing();  
  
    if (useRefreshControl)  
        RefreshControl.EndRefreshing();

            LoadLocalPreviousMessages();
}  
 
#region * iOS Specific Code  
// This method will add the UIRefreshControl to the table view if  
// it is available, ie, we are running on iOS 6+  
void AddRefreshControl()  
{  
    if (UIDevice.CurrentDevice.CheckSystemVersion(6, 0))  
    {  
        // the refresh control is available, let's add it  
        RefreshControl = new UIRefreshControl();  
        RefreshControl.ValueChanged += async (sender, e) =>  
        {  
            await RefreshAsync();  
        };  
        useRefreshControl = true;  
    }  
}
#endregion


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
                    //mAdapter = new GroupMessageAdapter(this, ListChatsCon);
                    //mRecyclerView.SetAdapter(mAdapter);
                    //mAdapter.NotifyDataSetChanged();
                
                }
                if (ListChatsCon != null && ListChatsCon.Count > 0)
                {
                    consolidatedList = new List<ListItem>();

                    foreach (var itemm in ListChatsCon)
                    {
                        DateItem dateItem = new DateItem();
                        dateItem.setDate(itemm.Key.ToShortDateString());
                        consolidatedList.Add(dateItem);

                        foreach (var general in itemm.Value)
                        {
                            GeneralGroupItem generalItem = new GeneralGroupItem();
                            generalItem.setChatMessagearray(general);
                            consolidatedList.Add(generalItem);
                        }
                    }


                    chatSource = new GroupChatListSource(consolidatedList);
                    tblChatList.Source = chatSource;
                   
                    tblChatList.ReloadData();
                    tblChatList.ScrollToNearestSelected(UITableViewScrollPosition.Middle,true);
                    loadList = true;
                }

            }
            catch (Exception e)
            {
              //  Crashes.TrackError(e);
            }
        }

       
        public class PostChatResponseViewModel
        {
            public ChatModel Chat { get; set; }
            public ChatMessageViewModel ChatMessage { get; set; }
        }

        partial void BtnSend_TouchUpInside(UIButton sender)
        {
           

            if (!string.IsNullOrEmpty(filePath))
            {
                SendMedia(filePath, "Photo");
            }
            else
            {
                ButtonSendChatMessage();
            }
            // txt_TextMessage.Text = "";
        }

        public async void SendMedia(string filePath, string mediaType)
        {
            BTProgressHUD.Show("Please Wait", maskType: ProgressHUD.MaskType.Black);
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
                        BTProgressHUD.Dismiss();
                        ButtonSendChatMessage();
                    }
                }
                catch (Exception e)
                {
                    // Crashes.TrackError(e);
                }
            }
            catch (Exception e)
            {

                // Crashes.TrackError(e);
            }

        }

        partial void BtnAttachment_TouchUpInside(UIButton sender)
        {
            ImageAttacted();
        }

        private void ImageAttacted()
        {
            UIAlertController _ImageSelection = new UIAlertController();
            _ImageSelection.AddAction(UIAlertAction.Create("Camera", UIAlertActionStyle.Default, (action) => SelectImageFromCamera()));
            _ImageSelection.AddAction(UIAlertAction.Create("Photo Library", UIAlertActionStyle.Default, (action) => SelectImageFromLibrary()));
            _ImageSelection.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, (action) => cancelUploadImage()));

            UIPopoverPresentationController presentationPopover = _ImageSelection.PopoverPresentationController;
            if (presentationPopover != null)
            {
                presentationPopover.SourceView = this.View;
                presentationPopover.PermittedArrowDirections = UIPopoverArrowDirection.Up;
            }
            // Display the pop up for options for selecting image
            this.PresentViewController(_ImageSelection, true, null);
        }

        private async void SelectImageFromCamera()
        {

            #region FromCamera      
            Camera.TakePicture(this, (obj) =>
            {


                PhotoCapture = new UIImage();
                PhotoCapture = obj.ValueForKey(new NSString("UIImagePickerControllerOriginalImage")) as UIImage;
                // ivUserProfilePic.Image = PhotoCapture;

                // This bit of code saves to the application's Documents directory, doesn't save metadata

                documentsDirectory = Environment.GetFolderPath
                                              (Environment.SpecialFolder.Personal);
                string imagename = "myAttachmentTemp" + DateTime.UtcNow.ToString("ddMMyyyyhhmmss") + ".jpg";
                filePath = System.IO.Path.Combine(documentsDirectory, imagename);
                NSData imgData = PhotoCapture.AsJPEG();
                NSError err = null;

                if (imgData.Save(filePath, false, out err))
                {
                    Console.WriteLine("saved as " + filePath);
                }
                else
                {
                    Console.WriteLine("NOT saved as" + filePath + " because" + err.LocalizedDescription);
                }
                SendMedia(filePath, "Photo");

            });
            #endregion
        }

        private void SelectImageFromLibrary()
        {
            #region FromLibrary
            //for selecting image from library
            NavigationController.PresentModalViewController(imagePicker, true);
            #endregion
        }

        private void cancelUploadImage()
        {

        }

        public async void ButtonSendChatMessage()
        {
            try
            {
                if (!string.IsNullOrEmpty(txt_TextMessage.Text) || lstAttachments.Count > 0)
                {

                    txt_TextMessage.Text = txt_TextMessage.Text.Trim(' ', '\n');

                    var GroupTempSaveMessage = new GroupMessageResponseViewModel
                    {
                        GroupId = GroupObject.GroupId,
                        Message = txt_TextMessage.Text,
                        Attachments = lstAttachments ?? new List<AttachmentViewModel>(),
                        SenderId = CommonHelper.GetUserId(),
                        MessageTime = DateTime.Now.ToUniversalTime()

                    };


                    var message = GroupRepository.SaveGroupMessage(GroupTempSaveMessage, "Private");
                    var GroupSend = new GroupMessageRequestViewModel
                    {
                        GroupId = GroupObject.GroupId,
                        Message = txt_TextMessage.Text,
                        Attachments = lstAttachments ?? new List<AttachmentViewModel>(),
                        MobileDatabaseId = message.id

                    };

                    lstAttachments = new List<AttachmentViewModel>();
                    txt_TextMessage.Text = "";
                    try
                    {
                        var result = await new GroupChatService().PostGroupMessageService(GroupSend);

                        if (result.Status == 1)
                        {
                            var GroupMessagelist = JsonConvert.DeserializeObject<GroupMessageResponseViewModel>(result.Response.ToString());
                            Console.WriteLine("posted successfully in grp");

                            var chatConversationResponce = ChatConversationRepository.GetConversationbyGroupId(GroupMessagelist.GroupId);
                            if (chatConversationResponce.success)
                            {
                                ChatConversationRepository.UpdateChatLastMessage(chatConversationResponce.chatConversation.id, GroupMessagelist.Message, GroupMessagelist.SenderProfileName);
                            }
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
                        //Crashes.TrackError(ex);
                    }
                }
            }
            catch (Exception ex)
            {
               // Crashes.TrackError(ex);
            }

        }

        public void ScrollToBottom(bool animated)
        {
            if (tblChatList.NumberOfSections() == 0)
                return;

            var items = (int)tblChatList.NumberOfRowsInSection(0);
            if (items == 0)
                return;

            var finalRow = (int)NMath.Max(0, tblChatList.NumberOfRowsInSection(0) - 1);
            NSIndexPath finalIndexPath = NSIndexPath.FromRowSection(finalRow, 0);
            tblChatList.ScrollToRow(finalIndexPath, UITableViewScrollPosition.Bottom, animated);
        }


        public void getGroupDetails()
        {
            try
            {
            if (GroupObject != null)
            {
                ContactName = GroupRepository.GetGroupByID(GroupObject.GroupId).GroupName;
              
                Title = ContactName;


                var titleView = new UILabel(new CGRect(0, 0, 100, 60));
                titleView.Text = ContactName;
                titleView.TextColor = UIColor.White;

                NavigationItem.TitleView = titleView;

            }
        }
            catch (Exception e)
            {
                //Crashes.TrackError(e);
            }
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
              //  Crashes.TrackError(e);
            }
        }


        private void _objChatSignalRService_OnGetGroupMessageUpdateStatusRecieved(object sender, GroupMessageStatusViewModel e)
        {
            try
            {
                if (e.GroupMessageId != 0)
                {
                    var savedMessages = GroupRepository.UpdateGroupMessageStatus(e);
                    InvokeOnMainThread(() =>
                    {
                        LoadLocalLatestMessages();
                    });
                }
            }
            catch (Exception ex)
            {
              
            }
        }



        private void _objChatSignalRService_OnGetGroupReload(object sender, string e)
        {
            try
            {
                InvokeOnMainThread(() =>
                {
                    LoadServerMessagesUpto(DateTime.Now);

                });
            }
            catch (Exception eX)
            {
               
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
                        GroupRepository.SaveGroupMessage(e, "Private");
                        InvokeOnMainThread(() =>
                        {
                            LoadLocalLatestMessages();
                        });
                        UpdateGroupChatMessage(e.SenderId.ToString(), e, "Private");

                    }
                }
            }
            catch (Exception ex)
            {
                //Toast.MakeText(this, ex.ToString(), ToastLength.Long).Show();
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
               
            }
        }

    

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

                        InvokeOnMainThread(() =>
                        {
                            paginationModel.SkipRecords = 0;
                            paginationModel.TakeRecords = 30;
                            LoadLocalLatestMessages();
                        });

                    }
                });
            }
            catch (Exception e)
            {
               // Crashes.TrackError(e);
            }
        }

    


    }
}