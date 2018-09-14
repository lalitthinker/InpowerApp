using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.OS;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using InPowerApp.Activities;
using InPowerApp.Common;
using InPowerApp.ListAdapter;
using InPowerApp.Model;
using InPowerApp.Repositories;
using Newtonsoft.Json;
using PCL.Model;
using PCL.Service;
using Java.Interop;
using Android.Support.V7.Widget;
using InPowerApp.SignalR;
using Plugin.Connectivity;
using Android.Support.Design.Widget;
using System.Threading.Tasks;
using static Android.Widget.AdapterView;
using Microsoft.AppCenter.Crashes;

namespace InPowerApp.Fragments
{
    public class ChatsFragment : Android.Support.V4.App.Fragment
    {

        RecyclerView mRecyclerView;
        RecyclerView.LayoutManager mLayoutManager;

        public int UserId;
        private List<ChatConversation> Chats;
        private ChatListAdapter _adapter;
        private Android.Support.V7.Widget.SearchView _searchView;
        ChatSignalRServices _objChatSignalRService = new ChatSignalRServices();
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            UserId = CommonHelper.GetUserId();
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
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

    
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.Chats_Fragment, container, false);
            try
            {
                HasOptionsMenu = true;
                mRecyclerView = (RecyclerView)view.FindViewById(Resource.Id.ChatList);
                mLayoutManager = new LinearLayoutManager(mRecyclerView.Context, LinearLayoutManager.Vertical, false);
                mRecyclerView.SetLayoutManager(mLayoutManager);
                Chats = new List<ChatConversation>();
                //FloatingActionButton fabAddBook = view.FindViewById<FloatingActionButton>(Resource.Id.fabAddChat);
                //fabAddBook.Click += FabAddChat_Click;


            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
            return view;
        }

        private void FabAddChat_Click(object sender, EventArgs e)
        {
            //Intent intent = new Intent(this.Activity, typeof(CreateGroupActivity));
            //StartActivity(intent);
        }

        internal static void LogUnhandledException(Exception exception)
        {
            try
            {
              
                Android.Util.Log.Error("Crash Report", exception.Message);
                Crashes.TrackError(exception);
            }
            catch
            {
               
            }
        }



        private async void loadSignalR()
        {
            try
            {
                await _objChatSignalRService.Connect();
                _objChatSignalRService.OnGetUpdateStatusRecieved += _objChatSignalRService_OnGetUpdateStatus;
                _objChatSignalRService.OnGroupMessageReceived += _objChatSignalRService_OnGroupMessageReceived;
                _objChatSignalRService.OnGetGroupMessageUpdateStatusRecieved += _objChatSignalRService_OnGetGroupMessageUpdateStatusRecieved;
                _objChatSignalRService.OnGetGroupCreateStatusRecieved += _objChatSignalRService_OnGetGroupCreateStatusRecieved;
                _objChatSignalRService.OnMessageReceived += _objChatSignalRService_OnMessageReceived;

                await _objChatSignalRService.ConnectUser("ChatList");

            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private void _objChatSignalRService_OnGetGroupMessageUpdateStatusRecieved(object sender, GroupMessageStatusViewModel e)
        {
            try
            {
                if (e.GroupMessageId != 0)
                {
                    var savedMessages = GroupRepository.UpdateGroupMessageStatus(e);
                    Activity.RunOnUiThread(() =>
                    {
                        LoadLocalChats();
                    });
                }
            }
            catch (Java.Lang.Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private void _objChatSignalRService_OnGetGroupCreateStatusRecieved(object sender, GroupResponseViewModel e)
        {
            try
            {
                if (e.GroupId != 0)
                {
                    GroupRepository.SaveGroupCreated(e);
                    ChatModel cm = new ChatModel();
                    cm.ChatId = e.ChatModel.ChatId;
                    cm.GroupId = e.ChatModel.GroupId;
                    cm.Message = e.ChatModel.Message;
                    cm.LastActiveTime = e.ChatModel.LastActiveTime;
                    ChatConversation chatConversation = ChatConversationRepository.SaveGroupConverstionNewFromServer(cm);
                    if (chatConversation != null)
                    {
                        Activity.RunOnUiThread(() =>
                        {
                            MessageReceived(chatConversation, "NewChat");
                        });
                    }
                }
            }
            catch (Java.Lang.Exception ex)
            {
                Crashes.TrackError(ex);
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
                         GroupRepository.SaveGroupMessage(e, "ChatList");
                        var chatConversation = ChatConversationRepository.GetConversationbyGroupId(e.GroupId);
                        if (e.Attachments.Count > 0)
                        {
                            e.Message = (string.IsNullOrEmpty(e.Message)) ? e.Attachments[e.Attachments.Count - 1].Type : e.Message;
                        }
                        chatConversation = ChatConversationRepository.UpdateChatLastMessage(chatConversation.id, e.Message, e.SenderProfileName);
                        if (chatConversation != null)
                        {
                            Activity.RunOnUiThread(() =>
                            {
                                MessageReceived(chatConversation, "OldChat");
                            });
                        }
                        //else
                        //{
                        //    Activity.RunOnUiThread(() =>
                        //    {
                        //        //chatConversation = new ChatConversation();
                        //        //chatConversation.ChatId = e.ChatId;
                        //        //chatConversation.ContactId = e.ContactId;
                        //        //chatConversation.CreatedDate = e.MessageTime;
                        //        //chatConversation.
                        //        //MessageReceived(chatConversation, "NewChat");
                        //    });
                        //}
                        GroupMessageStatusViewModel _model = new GroupMessageStatusViewModel();
                        _model.GroupMessageId = e.MessageId;
                        await _objChatSignalRService.SendGroupMessageUpdate(e.SenderId.ToString(), _model, "ChatList");
                    }
                }
            }
            catch (Java.Lang.Exception ex)
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
                    var chatmessage = ChatMessageRepository.CheckMessage(e.ChatMessageId);
                    if (chatmessage != null)
                    {
                        var savedMessages = ChatMessageRepository.updateChatMessage(e);
                        var chatConversation = ChatConversationRepository.GetConversationIdbyChatId(savedMessages.ChatId);
                        if (chatConversation != null)
                        {
                            Activity.RunOnUiThread(() =>
                            {
                                MessageReceived(chatConversation, "OldChat");
                            });
                        }
                    }
                }
            }
            catch (Java.Lang.Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }


        private async void _objChatSignalRService_OnMessageReceived(object sender, ChatMessageViewModel e)
        {
            try
            {
                if (e.ChatMessageId != 0)
                {
                    var chatmessage = ChatMessageRepository.CheckMessage(e.ChatMessageId);
                    if (chatmessage == null)
                    {
                        var savedMessages = ChatMessageRepository.SaveChatMessage(e, e.ChatId);
                        var chatConversation = ChatConversationRepository.GetConversationIdbyChatId(savedMessages.ChatId);
                        if (e.Attachments.Count > 0)
                        {
                            savedMessages.MessageText = (string.IsNullOrEmpty(savedMessages.MessageText))? e.Attachments[e.Attachments.Count - 1].Type : savedMessages.MessageText;
                        }
                        chatConversation = ChatConversationRepository.UpdateChatLastMessage(chatConversation.id, savedMessages.MessageText, "");
                        if (chatConversation != null)
                        {
                            Activity.RunOnUiThread(() =>
                            {
                                MessageReceived(chatConversation, "OldChat");
                            });
                        }
                       
                        await _objChatSignalRService.SendUpdate(chatConversation.ContactId.ToString(), e, "ChatList");
                    }
                }
            }
            catch (Java.Lang.Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }



        private void _adapter_ItemClick(object sender, ChatConversation e)
        {
            try
            {
                var ChatCon = e;
                if (ChatCon != null)
                {
                    var ContactUser = ContactRepository.GetContactbyUserId(Convert.ToInt64(ChatCon.ContactId));
                   
                    var ContactView = new ContactViewModel { ContactId = (long)ChatCon.ContactId, ChatId = ChatCon.ChatId, ProfileImageUrl = ContactUser.contactPicUrl,IsBlock=e.IsBlock ,ChatConvId=e.id};
                    if (ContactView != null)
                    {
                        var intent = new Intent(Context, typeof(PrivateMessageActivity));
                        intent.AddFlags(ActivityFlags.SingleTop);
                        intent.PutExtra("ContactObject", JsonConvert.SerializeObject(ContactView));
                        StartActivity(intent);
                    }
                }
            }
            catch (Exception ex)
            {
                Dictionary<string, string> dicVar = new Dictionary<string, string>();
                dicVar.Add("Index", e.ToString());
                dicVar.Add("Chat Count", Chats.Count.ToString());
                Crashes.TrackError(ex, dicVar);
            }
        }

        private async void LoaderAsyncfromserver()
        {
            try
            {
                await Task.Run(async () =>
                {
                    var result = await new ChatService().getChatAll(new UserRequestViewModel { UserId = CommonHelper.GetUserId() });

                    if (result.Status == 1)
                    {
                        var chatlist = JsonConvert.DeserializeObject<List<ChatModel>>(result.Response.ToString());
                        Console.Write(result.Status);
                        ChatConversationRepository.SaveChatConversationFromServer(chatlist);
                        Activity.RunOnUiThread(() =>
                        {
                            LoadLocalChats();
                        });
                    }
                });
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        private void LoadLocalChats()
        {
            try
            {

                Chats = ChatConversationRepository.GetAllChat();
                if (Chats.Count > 0)
                {
                    _adapter = new ChatListAdapter(this.Context, Chats);
                    _adapter.PrivateChatItemClick += _adapter_ItemClick;
                    _adapter.GroupChatItemClick += _adapter_GroupChatItemClick;
                    mRecyclerView.SetAdapter(_adapter);
                    _adapter.NotifyDataSetChanged();
                }
            }
            catch(Exception ex)
            { Crashes.TrackError(ex); }
        }

        private void MessageReceived(ChatConversation chatConversation, string ProcessType)
        {
            try
            {
                if (ProcessType == "OldChat")
                {
                    _adapter.update(chatConversation);
                    _adapter.NotifyDataSetChanged();
                }
                else
                {
                    _adapter.add(chatConversation);
                    _adapter.NotifyDataSetChanged();
                }
            }
            catch(Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private void _adapter_GroupChatItemClick(object sender, ChatConversation e)
        {
            try
            {
                var ChatCon = e;
                if (ChatCon != null)
                {
                    if (_objChatSignalRService.disconnectUser().IsCompleted)
                    {
                        var GroupModel = GroupRepository.GetGroupByID(Convert.ToInt64(ChatCon.GroupId));
                        //   var ContactView = new ContactViewModel { ContactId = (long)ChatCon.ContactId, ChatId = ChatCon.ChatId, ProfileImageUrl = ContactUser.contactPicUrl };
                        if (GroupModel != null)
                        {
                            var intent = new Intent(Context, typeof(GroupMessageActivity));
                            intent.PutExtra("GroupObject", JsonConvert.SerializeObject(GroupModel));
                            StartActivity(intent);
                        }
                    }
                }
            }
            catch (Exception ex)
            { Crashes.TrackError(ex); }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item != null)
            {
                if (item.ItemId == Resource.Id.action_ChatSearch)
                {

                }
            }

            return base.OnOptionsItemSelected(item);
        }


        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            // menu.Clear();
            inflater.Inflate(Resource.Menu.action_chat, menu);
            base.OnCreateOptionsMenu(menu, inflater);

            var searchItems = menu.FindItem(Resource.Id.action_ChatSearch);

            searchItems.SetVisible(true);

            //MenuItemCompat.SetOnActionExpandListener(searchItems, new SearchViewExpandListener((IFilter)mAdapter));

            var searchItem = MenuItemCompat.GetActionView(searchItems);
            _searchView = searchItem.JavaCast<Android.Support.V7.Widget.SearchView>();
            if(_adapter!=null)
            {

                _searchView.QueryTextChange += (s, e) => _adapter.Filter.InvokeFilter(e.NewText.ToLower());

                _searchView.QueryTextSubmit += (s, e) =>
                {
                    // Handle enter/search button on keyboard here
                    Toast.MakeText(this.Context, "Searched for: " + e.Query, ToastLength.Short).Show();
                    e.Handled = true;
                };

            }
           
            //  MenuItemCompat.SetOnActionExpandListener(searchItems, new SearchViewExpandListener(_adapter));
        }

        //      
        public async override void OnResume()
        {
            base.OnResume();
            await Task.Run(async () =>
            {
                Loadonresume();
            });
        }

        public async override void OnStop()
        {
            await _objChatSignalRService.disconnectUser();
            base.OnStop();
        }
        private async void Loadonresume()
        {
            if (IsVisible == true)
            {
                if (InternetConnectivityModel.CheckConnection(this.Context))
                {
                    loadSignalR();
                }

                Activity.RunOnUiThread(() =>
                {
                    LoadLocalChats();
                });

                if (InternetConnectivityModel.CheckConnection(this.Context))
                {
                    LoaderAsyncfromserver();
                }
            }
        }
    }
}