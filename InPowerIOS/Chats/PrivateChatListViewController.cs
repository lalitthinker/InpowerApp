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
using Microsoft.AppCenter.Crashes;
using System.Linq;

namespace InPowerIOS
{
    public partial class PrivateChatListViewController : UIViewController
    {
        private float scroll_amount = 0.0f;    // amount to scroll 
        private float bottom = 0.0f;           // bottom point
        private float offset = 45.0f;          // extra offset
        private bool moveViewUp = false;

        public ChatConversation chatConversation { get; set; }
        ChatSignalRServices _objChatSignalRService = new ChatSignalRServices();
        PaginationModel paginationModel = new PaginationModel();
        public ContactViewModel contactViewModel { get; set; }
        long contactId;
        string ContactName = "";
        bool loadList = true;
        Dictionary<DateTime, List<ChatMessage>> ListChatsCon;

        private ChatListViewControllerSource chatViewContarollerSource;

        List<AttachmentViewModel> lstAttachments;

        NSObject willShowToken;
        NSObject willHideToken;
        Message message;
        List<Message> messages;
        ChatListSource chatSource;

      
        UIToolbar toolbar;

        NSLayoutConstraint toolbarBottomConstraint;
        NSLayoutConstraint toolbarHeightConstraint;
        int notifCount = 0;

        List<ListItem> consolidatedList;

        UIImagePickerController imagePicker;
        UIImage PhotoCapture;
        string ProfileImageURL;
        string documentsDirectory, filePath;
        string mediaType = "Photo";
        UIButton cameraButton;
        bool useRefreshControl = false;  
        UIRefreshControl RefreshControl;  

        public PrivateChatListViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
             
            ScrollToBottom(false);

            imagePicker = new UIImagePickerController();
            imagePicker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
            imagePicker.MediaTypes = UIImagePickerController.AvailableMediaTypes(UIImagePickerControllerSourceType.PhotoLibrary);
            imagePicker.FinishedPickingMedia += Handle_FinishedPickingMedia;
            imagePicker.Canceled += Handle_Canceled;

            getUserDetails();
            AddRefreshControl();
            var g = new UITapGestureRecognizer(() => View.EndEditing(false));
            g.CancelsTouchesInView = true;
            View.AddGestureRecognizer(g);

            //LoadLocalLatestMessages();
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

        public override void ViewWillAppear(bool animated)
        {
            //base.ViewWillAppear(animated);
            //Task.Run(() =>
            //{
            //    Loadonresume();
            //});
            Loadonresume();

        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
         
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


    
        private void Loadonresume()
        {
            try
            {
                ContactName = ContactRepository.GetContactbyUserId(Convert.ToInt64(contactViewModel.ContactId)).name;
                contactId = contactViewModel.ContactId;
                if (InternetConnectivityModel.CheckConnection())
                {
                    loadSignalR();
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
                Crashes.TrackError(e);
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
                    messages = new List<Message>();
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
                                GeneralItem generalItem = new GeneralItem();
                                generalItem.setChatMessagearray(general);
                                consolidatedList.Add(generalItem);
                            }
                        }
                        chatSource = new ChatListSource(consolidatedList);
                        tblChatList.Source = chatSource;
                        tblChatList.Add(RefreshControl);
                        tblChatList.ReloadData();
                        ScrollToBottom(true);
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }



        private async void LoadServerMessagesUpto(DateTime date)
        {
            var unixTimeStamp = CommonHelper.DateTimeToUnixTimestamp(date.AddSeconds(-1));
            try
            {
               // BTProgressHUD.Show("Loading messages..", maskType: ProgressHUD.MaskType.Black);
                await Task.Run(async () =>
                {
                    var result = await new ChatService().GetChatMessagesUptoId(contactViewModel.ContactId, unixTimeStamp);
                    if (result.Status == 1)
                    {
                        var chatMessagelist = JsonConvert.DeserializeObject<List<ChatMessageViewModel>>(result.Response.ToString());

                        var savedMessages = ChatMessageRepository.SaveChatMessages(chatMessagelist, chatConversation.ChatId);

                        InvokeOnMainThread(() =>
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

        partial void BtnSend_TouchUpInside(UIButton sender)
        {
            var sMessageText = txt_TextMessage.Text;
           
            if (string.IsNullOrWhiteSpace(sMessageText))
                return;

           

            if (!string.IsNullOrEmpty(filePath))
            {
                SendMedia(filePath, "Photo");
            }
            else
            {
                ButtonSendChatMessage(sMessageText);
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
                        ButtonSendChatMessage(txt_TextMessage.Text);
                    }
                }
                catch (Exception e)
                {
                    Crashes.TrackError(e);
                }
            }
            catch (Exception e)
            {

                Crashes.TrackError(e);
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
                SendMedia(filePath,"Photo");
               
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

        public async void ButtonSendChatMessage(string sMessageText)
        {
            try
            {

                if (!string.IsNullOrEmpty(sMessageText) || lstAttachments.Count > 0)
                {

                    var chatModel = new ChatMessageViewModel
                    {
                        Attachments = lstAttachments ?? new List<AttachmentViewModel>(),
                        ContactId = contactId,
                        Message = sMessageText.Trim(' ', '\n'),
                        IsRead = false,
                        IsRecieved = false,
                        IsSend = false,
                        MessageTime = DateTime.Now.ToUniversalTime()
                    };


                   var  chatConversationResponse = ChatConversationRepository.GetConversationbyContactId(contactId);
                    if (chatConversationResponse.success)
                    {
                        if (chatConversationResponse.chatConversation != null)
                        {
                            var message = ChatMessageRepository.SaveChatMessage(chatModel, chatConversationResponse.chatConversation.ChatId);
                            chatModel.MobiledatabaseId = message.id;
                            chatModel.ChatId = chatConversationResponse.chatConversation.ChatId;
                            chatModel.IsRead = false;
                            chatModel.IsRecieved = false;
                            chatModel.IsSend = false;
                            //var msg = new Message
                            //{
                            //    Type = MessageType.Outgoing,
                            //    Text = sMessageText.Trim()
                            //};

                            //DateItem dateItem = new DateItem();
                            //dateItem.setDate(chatModel.MessageTime.ToShortDateString());
                            //if(!consolidatedList.Contains(dateItem))
                            //{
                            //    consolidatedList.Add(dateItem);
                            //}

                            //    GeneralItem generalItem = new GeneralItem();
                            //generalItem.setChatMessagearray(message);
                            //    consolidatedList.Add(generalItem);


                            //tblChatList.InsertRows(new NSIndexPath[] { NSIndexPath.FromRowSection(messages.Count - 1, 0) }, UITableViewRowAnimation.None);

                            //ScrollToBottom(true);
                        }
                    }

                   

                    lstAttachments = new List<AttachmentViewModel>();
                    try
                    {
                        var result = await new ChatService().PostChat(chatModel);

                        if (result.Status == 2 || result.Status == 1)
                        {
                            var ChatResponse = JsonConvert.DeserializeObject<PostChatResponseViewModel>(result.Response.ToString());

                            if (chatConversationResponse.chatConversation == null)
                            {
                                ChatConversationRepository.SaveConverstionNewFromServer(ChatResponse.Chat);
                                chatConversationResponse = ChatConversationRepository.GetConversationbyContactId(contactId);
                                if (chatConversationResponse.success)
                                {
                                    ChatConversationRepository.UpdateChatLastMessage(chatConversationResponse.chatConversation.id, ChatResponse.ChatMessage.Message, "");

                                    var savedMessages = ChatMessageRepository.SaveChatMessage(ChatResponse.ChatMessage, chatConversationResponse.chatConversation.ChatId);
                                }
                                LoadLocalLatestMessages();
                                Console.WriteLine("CHAT POSTED : " + result);
                            }
                            else
                            {
                                
                                 chatConversationResponse = ChatConversationRepository.GetConversationbyContactId(contactId);
                                if (chatConversationResponse.success)
                                {
                                    ChatConversationRepository.UpdateChatLastMessage(chatConversationResponse.chatConversation.id, ChatResponse.ChatMessage.Message, "");

                                    var savedMessages = ChatMessageRepository.updateChatMessage(ChatResponse.ChatMessage);
                                    LoadLocalLatestMessages();
                                    await _objChatSignalRService.Send(chatConversationResponse.chatConversation.ContactId.ToString(), ChatResponse.ChatMessage);
                                }
                            }
                        }


                    }

                    catch (Exception ex)
                    {
                        Crashes.TrackError(ex);
                    }
                }

                txt_TextMessage.Text = "";
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
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


        public void getUserDetails()
        {
          var  chatConversationResponce = ChatConversationRepository.GetConversationbyContactId(contactViewModel.ContactId);
            if(chatConversationResponce.success)
            {
                chatConversation = chatConversationResponce.chatConversation;
            }
                if (contactViewModel != null)
                {
                    ContactName = ContactRepository.GetContactbyUserId(contactViewModel.ContactId).name;
                    contactId = contactViewModel.ContactId;
                    Title = ContactName;


                    var titleView = new UILabel(new CGRect(0, 0, 100, 60));
                    titleView.Text = ContactName;
                    titleView.TextColor = UIColor.White;
                    var ShowUserProfileViewController = new UITapGestureRecognizer(() =>
                    {
                        var viewController = (UserProfileViewController)Storyboard.InstantiateViewController("UserProfileViewController");
                        viewController.contactViewModel = contactViewModel;
                        NavigationController.PushViewController(viewController, true);
                    });
                    titleView.UserInteractionEnabled = true;
                    titleView.AddGestureRecognizer(ShowUserProfileViewController);
                    NavigationItem.TitleView = titleView;

                }

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
                if (contactViewModel != null)
                {
                    await _objChatSignalRService.Reload(contactViewModel.ContactId.ToString());
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
                InvokeOnMainThread(() =>
                {
                    LoadServerMessagesUpto(DateTime.Now);
                });
            }
            catch (Exception eX)
            {
                Crashes.TrackError(eX);
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
                            if (contactViewModel.ChatId == e.ChatId)
                            {
                                e.IsRead = true;
                            }
                            var savedMessages = ChatMessageRepository.SaveChatMessage(e, e.ChatId);
                            InvokeOnMainThread(() =>
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

                            InvokeOnMainThread(() =>
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

        async Task RefreshAsync()  
        {  
            // only activate the refresh control if the feature is available  
            if (useRefreshControl)  
                RefreshControl.BeginRefreshing();  
  
            if (useRefreshControl)  
                RefreshControl.EndRefreshing();

            LoadLocalPreviousMessages();
        }  

        public void LoadLocalPreviousMessages()
        {
            try
            {
                loadList = false;
                paginationModel.SkipRecords += 30;

                int PreviousListCount = ListChatsCon.Values.Sum(list => list.Count);
                ListChatsCon = ChatMessageRepository.GetChatMessagesForPageIndex(paginationModel, chatConversation.ChatId);
                int CurrentListCount = ListChatsCon.Values.Sum(list => list.Count);
               
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
                            GeneralItem generalItem = new GeneralItem();
                            generalItem.setChatMessagearray(general);
                            consolidatedList.Add(generalItem);
                        }
                    }


                    chatSource = new ChatListSource(consolidatedList);
                    tblChatList.Source = chatSource;
                    tblChatList.ReloadData();
                    ScrollToBottom(true);
                }

            }
            catch (Exception e)
            {
                //  Crashes.TrackError(e);
            }
        }




    }
}