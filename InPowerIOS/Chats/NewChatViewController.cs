using Foundation;
using System;
using UIKit;
using CoreGraphics;
using InPowerIOS.NewChat;
using InPowerIOS.Models;
using System.Collections.Generic;
using InPowerIOS.Model;
using InPowerIOS.SignalR;
using PCL.Model;
using InPowerIOS.Chats;
using System.Threading.Tasks;
using InPowerIOS.Repositories;
using InPowerIOS.Common;
using PCL.Service;
using Newtonsoft.Json;
using InPowerApp.Model;
using static InPowerIOS.Chats.ChatViewContarollerSource;
using System.Drawing;
using Microsoft.AppCenter.Crashes;

namespace InPowerIOS.Chats
{
    [Register("NewChatViewController")]
    public partial class NewChatViewController : UIViewController
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

        private ChatViewContarollerSource chatViewContarollerSource;

        List<AttachmentViewModel> lstAttachments;




        NSObject willShowToken;
        NSObject willHideToken;
        Message message;
        List<Message> messages;
        ChatSource chatSource;

        UITableView tableView;
        UIToolbar toolbar;

        NSLayoutConstraint toolbarBottomConstraint;
        NSLayoutConstraint toolbarHeightConstraint;
        int notifCount = 0;

        ChatInputView chatInputView;

        UIButton SendButton
        {
            get
            {
                return chatInputView.SendButton;
            }
        }

        UITextView TextView
        {
            get
            {
                return chatInputView.TextView;
            }
        }

        bool IsInputToolbarHasReachedMaximumHeight
        {
            get
            {
                return toolbar.Frame.GetMinY() == TopLayoutGuide.Length;
            }
        }

        public NewChatViewController(IntPtr handle)
            : base (handle)
        {
            this.contactViewModel = contactViewModel;

        }

        #region Life cycle

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
         
            getUserDetails();
            paginationModel.TakeRecords = 30;
            paginationModel.SkipRecords = 0;
            SetUpTableView();
            SetUpToolbar();

            SendButton.TouchUpInside += OnSendClicked;
            TextView.Started += OnTextViewStarted;
            TextView.Changed += OnTextChanged;


            // Keyboard popup
            NSNotificationCenter.DefaultCenter.AddObserver
            (UIKeyboard.DidShowNotification, KeyBoardUpNotification);

            // Keyboard Down
            NSNotificationCenter.DefaultCenter.AddObserver
            (UIKeyboard.WillHideNotification, KeyBoardDownNotification);
           // TextView.BecomeFirstResponder();

            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
        
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            willShowToken = UIKeyboard.Notifications.ObserveWillShow(KeyboardWillShowHandler);
            willHideToken = UIKeyboard.Notifications.ObserveWillHide(KeyboardWillHideHandler);

            UpdateTableInsets();
            UpdateButtonState();
            ScrollToBottom(false);
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            AddObservers();



            Task.Run(() =>
            {
                Loadonresume();
            });
        }

        #endregion

        #region Initialization

        void SetUpTableView()
        {
            tableView = new UITableView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                AllowsSelection = false,
                SeparatorStyle = UITableViewCellSeparatorStyle.None
            };
            tableView.RegisterClassForCellReuse(typeof(IncomingCell), IncomingCell.CellId);
            tableView.RegisterClassForCellReuse(typeof(OutgoingCell), OutgoingCell.CellId);
            View.AddSubview(tableView);

            if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
            {   // iPhone X layout
                var safeGuide = View.SafeAreaLayoutGuide;
                tableView.TopAnchor.ConstraintEqualTo(safeGuide.TopAnchor).Active = true;
                tableView.LeadingAnchor.ConstraintEqualTo(safeGuide.LeadingAnchor).Active = true;
                tableView.TrailingAnchor.ConstraintEqualTo(safeGuide.TrailingAnchor).Active = true;
                tableView.BottomAnchor.ConstraintEqualTo(safeGuide.BottomAnchor, -44).Active = true;
            }
            else
            {
                var pinLeft = NSLayoutConstraint.Create(tableView, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, View, NSLayoutAttribute.Leading, 1f, 0f);
                View.AddConstraint(pinLeft);

                var pinRight = NSLayoutConstraint.Create(tableView, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, View, NSLayoutAttribute.Trailing, 1f, 0f);
                View.AddConstraint(pinRight);

                var pinTop = NSLayoutConstraint.Create(tableView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, TopLayoutGuide, NSLayoutAttribute.Bottom, 1f, 0f);
                View.AddConstraint(pinTop);

                var pinBottom = NSLayoutConstraint.Create(tableView, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, View, NSLayoutAttribute.Bottom, 1f, 0f);
                View.AddConstraint(pinBottom);
            }

            LoadLocalLatestMessages();
        }

        void SetUpToolbar()
        {
            toolbar = new UIToolbar
            {
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            chatInputView = new ChatInputView
            {
                TranslatesAutoresizingMaskIntoConstraints = false
            };

            toolbar.LayoutIfNeeded();
            View.AddSubview(toolbar);


            if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
            {   // iPhone X layout
                var safeGuide = View.SafeAreaLayoutGuide;
                toolbar.HeightAnchor.ConstraintEqualTo(44).Active = true;
                toolbar.LeadingAnchor.ConstraintEqualTo(safeGuide.LeadingAnchor).Active = true;
                toolbar.TrailingAnchor.ConstraintEqualTo(safeGuide.TrailingAnchor).Active = true;
                toolbarBottomConstraint = toolbar.BottomAnchor.ConstraintEqualTo(safeGuide.BottomAnchor);
                toolbarBottomConstraint.Active = true;
            }
            else
            {
                var pinLeft = NSLayoutConstraint.Create(toolbar, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, View, NSLayoutAttribute.Leading, 1f, 0f);
                View.AddConstraint(pinLeft);

                var pinRight = NSLayoutConstraint.Create(toolbar, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, View, NSLayoutAttribute.Trailing, 1f, 0f);
                View.AddConstraint(pinRight);

                toolbarBottomConstraint = NSLayoutConstraint.Create(View, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, toolbar, NSLayoutAttribute.Bottom, 1f, 0f);
                View.AddConstraint(toolbarBottomConstraint);

                toolbarHeightConstraint = NSLayoutConstraint.Create(toolbar, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 0f, 44f);
                View.AddConstraint(toolbarHeightConstraint);
            }

            toolbar.AddSubview(chatInputView);

            var c1 = NSLayoutConstraint.FromVisualFormat("H:|[chat_container_view]|",
                0,
                "chat_container_view", chatInputView
            );
            var c2 = NSLayoutConstraint.FromVisualFormat("V:|[chat_container_view]|",
                0,
                "chat_container_view", chatInputView
            );
            toolbar.AddConstraints(c1);
            toolbar.AddConstraints(c2);
        }

        #endregion

        void AddObservers()
        {
            TextView.AddObserver(this, "contentSize", NSKeyValueObservingOptions.OldNew, IntPtr.Zero);
        }

        public override void ObserveValue(NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
        {
            if (keyPath == "contentSize")
                OnSizeChanged(new NSObservedChange(change));
            else
                base.ObserveValue(keyPath, ofObject, change, context);
        }

        void OnSizeChanged(NSObservedChange change)
        {
            CGSize oldValue = ((NSValue)change.OldValue).CGSizeValue;
            CGSize newValue = ((NSValue)change.NewValue).CGSizeValue;

            var dy = newValue.Height - oldValue.Height;
            AdjustInputToolbarOnTextViewSizeChanged(dy);
        }

        void AdjustInputToolbarOnTextViewSizeChanged(nfloat dy)
        {
            bool isIncreasing = dy > 0;
            if (IsInputToolbarHasReachedMaximumHeight && isIncreasing)
            {
                // TODO: scroll to bottom
                return;
            }

            nfloat oldY = toolbar.Frame.GetMinY();
            nfloat newY = oldY - dy;
            if (newY < TopLayoutGuide.Length)
                dy = oldY - TopLayoutGuide.Length;

            AdjustInputToolbar(dy);
        }

        void AdjustInputToolbar(nfloat change)
        {
            toolbarHeightConstraint.Constant += change;

            if (toolbarHeightConstraint.Constant < ChatInputView.ToolbarMinHeight)
                toolbarHeightConstraint.Constant = ChatInputView.ToolbarMinHeight;

            View.SetNeedsUpdateConstraints();
            View.LayoutIfNeeded();
        }

        void KeyboardWillShowHandler(object sender, UIKeyboardEventArgs e)
        {
            UpdateButtomLayoutConstraint(e);
        }

        void KeyboardWillHideHandler(object sender, UIKeyboardEventArgs e)
        {
            notifCount = 0;
            SetToolbarContstraint(0);
        }

        void UpdateButtomLayoutConstraint(UIKeyboardEventArgs e)
        {
            UIViewAnimationCurve curve = e.AnimationCurve;
            if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
            {
                UIView.Animate(e.AnimationDuration, 0, ConvertToAnimationOptions(e.AnimationCurve), () =>
                {
                    nfloat offsetFromBottom = toolbar.Frame.GetMaxY() - e.FrameEnd.GetMinY();
                    offsetFromBottom = NMath.Max(0, offsetFromBottom);
                    if (++notifCount >= 2) { SetToolbarContstraint(-offsetFromBottom); }
                }, null);
            }
            else
            {
                UIView.Animate(e.AnimationDuration, 0, ConvertToAnimationOptions(e.AnimationCurve), () =>
                {
                    nfloat offsetFromBottom = tableView.Frame.GetMaxY() - e.FrameEnd.GetMinY();
                    offsetFromBottom = NMath.Max(0, offsetFromBottom);
                    SetToolbarContstraint(offsetFromBottom);
                }, null);
            }
        }

        void SetToolbarContstraint(nfloat constant)
        {
            toolbarBottomConstraint.Constant = constant;
            View.SetNeedsUpdateConstraints();
            View.LayoutIfNeeded();

            UpdateTableInsets();
        }

        void UpdateTableInsets()
        {
            nfloat bottom = tableView.Frame.GetMaxY() - toolbar.Frame.GetMinY();
            var insets = new UIEdgeInsets(0f, 0f, bottom, 0f);
            tableView.ContentInset = insets;
            tableView.ScrollIndicatorInsets = insets;
        }

        UIViewAnimationOptions ConvertToAnimationOptions(UIViewAnimationCurve curve)
        {
            // Looks like a hack. But it is correct.
            // UIViewAnimationCurve and UIViewAnimationOptions are shifted by 16 bits
            // http://stackoverflow.com/questions/18870447/how-to-use-the-default-ios7-uianimation-curve/18873820#18873820
            return (UIViewAnimationOptions)((int)curve << 16);
        }

        public async void ButtonSendChatMessage(string sMessageText)
        {
            try
            {
                if (!string.IsNullOrEmpty(sMessageText))
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


                    var chatConversationResponse = ChatConversationRepository.GetConversationbyContactId(contactId);
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
                            var msg = new Message
                            {
                                Type = MessageType.Outgoing,
                                Text = sMessageText.Trim()
                            };

                            messages.Add(msg);
                            tableView.InsertRows(new NSIndexPath[] { NSIndexPath.FromRowSection(messages.Count - 1, 0) }, UITableViewRowAnimation.None);
                            ScrollToBottom(true);
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
                                if(chatConversationResponse.success)
                                {
                                    ChatConversationRepository.UpdateChatLastMessage(chatConversationResponse.chatConversation.id, ChatResponse.ChatMessage.Message, "");
                                }
                                var savedMessages = ChatMessageRepository.updateChatMessage(ChatResponse.ChatMessage);
                                LoadLocalLatestMessages();
                                await _objChatSignalRService.Send(chatConversationResponse.chatConversation.ContactId.ToString(), ChatResponse.ChatMessage);

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


        void OnSendClicked(object sender, EventArgs e)
        {
            var sMessageText = TextView.Text;
            TextView.Text = string.Empty; // this will not generate change text event
            UpdateButtonState();

            if (string.IsNullOrWhiteSpace(sMessageText))
                return;

            ButtonSendChatMessage(sMessageText);

          
        }

        void OnTextViewStarted(object sender, EventArgs e)
        {
            ScrollToBottom(true);
        }

        void OnTextChanged(object sender, EventArgs e)
        {
            UpdateButtonState();
        }

        void UpdateButtonState()
        {
            SendButton.Enabled = !string.IsNullOrWhiteSpace(TextView.Text);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            willShowToken.Dispose();
            willHideToken.Dispose();
        }

        void ScrollToBottom(bool animated)
        {
            if (tableView.NumberOfSections() == 0)
                return;

            var items = (int)tableView.NumberOfRowsInSection(0);
            if (items == 0)
                return;

            var finalRow = (int)NMath.Max(0, tableView.NumberOfRowsInSection(0) - 1);
            NSIndexPath finalIndexPath = NSIndexPath.FromRowSection(finalRow, 0);
            tableView.ScrollToRow(finalIndexPath, UITableViewScrollPosition.Top, animated);
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

                InvokeOnMainThread(() =>
                {
                    paginationModel.SkipRecords = 0;
                    paginationModel.TakeRecords = 30;
                    LoadLocalLatestMessages();
                });

                if (InternetConnectivityModel.CheckConnection())
                {
                    LoadServerMessagesUpto(DateTime.UtcNow);
                }
            }
            catch (Exception e)
            {
                //Crashes.TrackError(e);
            }
        }
        List<ListItem> consolidatedList;
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

                    

                        foreach(var ChatConverstions in consolidatedList)
                        {
                            message = new Message();
                            switch (ChatConverstions.getType())
                            {
                                case 1:
                                    {
                                        GeneralItem GeneralItem = (GeneralItem)ChatConverstions;
                                        var item = GeneralItem.getChatMessagearray();
                                        Boolean isMe = item.ContactId != Common.CommonHelper.GetUserId();
                                        var AttachList = (item.ChatMessageId != 0) ? Repositories.ChatAttachmentRepository.GetChatAttachList(item.ChatMessageId) : new List<ChatAttachment>();

                                        if (isMe)
                                        {
                                            message.Text = item.MessageText;
                                            message.Type = MessageType.Outgoing;
                                        }

                                        else
                                        {
                                            message.Text = item.MessageText;
                                            message.Type = MessageType.Incoming;
                                        }

                                        break;
                                    }
                                case 0:
                                    {
                                        break;
                                    }
                            }

                            messages.Add(message);
                        }
                  
                        chatSource = new ChatSource(messages);
                        tableView.Source = chatSource;
                        tableView.ReloadData();
                        ScrollToBottom(true);
                    }
                }
            }
            catch (Exception e)
            {
                //Crashes.TrackError(e);
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
                //Crashes.TrackError(e);
            }
        }
        public class PostChatResponseViewModel
        {
            public ChatModel Chat { get; set; }
            public ChatMessageViewModel ChatMessage { get; set; }
        }


        public void getUserDetails()
        {
            //var chatConversationResponse = ChatConversationRepository.GetConversationbyContactId(contactViewModel.ContactId);
            //if(chatConversationResponse.success)
            //{
                
            //}

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

            //if (contactViewModel.ProfileImageUrl != null && contactViewModel.ProfileImageUrl != "")
            //{

            //    Picasso.With(this)
            //   .Load(ContactObject.ProfileImageUrl)
            //  .Resize(100, 100)
            //  .CenterCrop().Placeholder(Resource.Drawable.default_profile)
            //  .Into(IV_Userlogo);
            //}
            //else
            //{
            //    IV_Userlogo.SetBackgroundResource(Resource.Drawable.default_profile);
            //}

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
                //Crashes.TrackError(e);
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
                //Crashes.TrackError(eX);
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
                //Android.Util.Log.Error("Crash Report", exception.Message);
                //Crashes.TrackError(exception);
            }
            catch
            {
                // just suppress any error logging exceptions
            }
        }

        private void KeyBoardUpNotification(NSNotification notification)
        {
            // get the keyboard size
            var r = UIKeyboard.FrameBeginFromNotification(notification);
            //  RectangleF r = UIKeyboard.BoundsFromNotification(notification);

            // Find what opened the keyboard
            //foreach (UIView view in this.View.Subviews)
            //{
            //  if (view.IsFirstResponder)
            //      activeview = view;
            //}

            // Bottom of the controller = initial position + height + offset      
            //bottom = (((float)((float)(this.View.Frame.Y) + this.View.Frame.Height)));

         
            bottom = ((float)(this.View.Frame.Height) + offset);

            // Calculate how far we need to scroll
            scroll_amount = ((float)(r.Height - (View.Frame.Size.Height - bottom)));

            // Perform the scrolling
            if (scroll_amount > 0)
            {
                moveViewUp = true;
                ScrollTheView(moveViewUp);
            }
            else
            {
                moveViewUp = false;
            }

        }

        private void KeyBoardDownNotification(NSNotification notification)
        {
            if (moveViewUp) { ScrollTheView(false); }
        }

        private void ScrollTheView(bool move)
        {

            // scroll the view up or down
            UIView.BeginAnimations(string.Empty, System.IntPtr.Zero);
            UIView.SetAnimationDuration(0.3);

            RectangleF frame = (System.Drawing.RectangleF)View.Frame;

            if (move)
            {
                frame.Y -= scroll_amount;
            }
            else
            {
                frame.Y += scroll_amount;
                scroll_amount = 0;
            }

            View.Frame = frame;
            UIView.CommitAnimations();
        }
    }
}

