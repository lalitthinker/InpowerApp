using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InPowerIOS.Common;
using InPowerApp.Model;
using PCL.Model;
using SQLite;
using InPowerIOS.Model;
using Microsoft.AppCenter.Crashes;
using InPowerIOS.Models;
using PCL.Common;

namespace InPowerIOS.Repositories
{
    public class ChatConversationRepository
    {
     
        public static void SaveChatConversationFromServer(List<ChatModel> chats)
        {
            if (GlobalConstant.ChatInsertStart)
            {
                var db = new SQLiteConnection(CommonConstant.DBPath);
                //ChatConversation mdchatconversation = null;
                try
                {
                    GlobalConstant.ChatInsertStart = false;
                    foreach (var chat in chats)
                    {
                        if (chat.IsGroup == false)
                        {
                            var conversationResponce = GetConversationbyId(chat.ChatId, (long)chat.ContactId);

                            if (conversationResponce.success)
                            {
                                if (conversationResponce.chatConversation == null)
                                {
                                    conversationResponce.chatConversation = new ChatConversation();
                                    conversationResponce.chatConversation.LastMessage = chat.Message;
                                    conversationResponce.chatConversation.ChatId = chat.ChatId;
                                    conversationResponce.chatConversation.ContactId = chat.ContactId;
                                    conversationResponce.chatConversation.LastMessageDate = TimeZone.CurrentTimeZone.ToLocalTime(Convert.ToDateTime(chat.LastActiveTime));
                                    conversationResponce.chatConversation.OwnerId = CommonHelper.GetUserId();  //set my owner id from preference
                                    conversationResponce.chatConversation.UserId = CommonHelper.GetUserId();
                                    //var contact = new Contact
                                    //{
                                    //    name = chat.ContactProfile.FirstName + chat.ContactProfile.LastName,
                                    //    contactId = chat.ContactProfile.UserId,
                                    //    contactPicUrl = chat.ContactProfile.ProfileImageUrl,
                                    //    Aboutme = chat.ContactProfile.AboutMe,
                                    //    city = chat.ContactProfile.City,
                                    //    state = chat.ContactProfile.State,
                                    //    email = chat.email,
                                    //    screenName = chat.ContactProfile.FirstName + " " + chat.ContactProfile.LastName,
                                    //    UserId = CommonHelper.GetUserId()
                                    //};
                                    //ContactRepository.SaveorUpdateContact(contact);
                                    db.Insert(conversationResponce.chatConversation);
                                }
                                else
                                {
                                    conversationResponce.chatConversation.LastMessage = chat.Message;
                                    conversationResponce.chatConversation.SenderName = chat.SenderName;
                                    conversationResponce.chatConversation.LastMessageDate = TimeZone.CurrentTimeZone.ToLocalTime(Convert.ToDateTime(chat.LastActiveTime));
                                    conversationResponce.chatConversation.OwnerId = CommonHelper.GetUserId(); //set my owner id from preference
                                    conversationResponce.chatConversation.UserId = CommonHelper.GetUserId();
                                    //var contact = new Contact
                                    //{
                                    //    name = chat.ContactProfile.FirstName + " " + chat.ContactProfile.LastName,
                                    //    contactId = chat.ContactProfile.UserId,
                                    //    contactPicUrl = chat.ContactProfile.ProfileImageUrl,
                                    //    Aboutme = chat.ContactProfile.AboutMe,
                                    //    city = chat.ContactProfile.City,
                                    //    state = chat.ContactProfile.State,
                                    //    email = chat.email,
                                    //    screenName = chat.ContactProfile.FirstName + " " + chat.ContactProfile.LastName,
                                    //    UserId = CommonHelper.GetUserId()
                                    //};
                                    //ContactRepository.SaveorUpdateContact(contact);
                                    db.Update(conversationResponce.chatConversation);
                                }
                            }
                        }
                        else
                        {
                            var conversationResponce = GetConversationbyGroupId((long)chat.GroupId);
                            if (conversationResponce.success)
                            {
                                if (conversationResponce.chatConversation == null)
                                {
                                    conversationResponce.chatConversation = new ChatConversation();
                                    conversationResponce.chatConversation.LastMessage = chat.Message;
                                    conversationResponce.chatConversation.SenderName = chat.SenderName;
                                    conversationResponce.chatConversation.ChatId = chat.ChatId;
                                    conversationResponce.chatConversation.GroupId = chat.GroupId;
                                    conversationResponce.chatConversation.IsGroup = chat.IsGroup;
                                    conversationResponce.chatConversation.LastMessageDate = TimeZone.CurrentTimeZone.ToLocalTime(Convert.ToDateTime((DateTime)chat.LastActiveTime));
                                    conversationResponce.chatConversation.OwnerId = CommonHelper.GetUserId();  //set my owner id from preference
                                    conversationResponce.chatConversation.UserId = CommonHelper.GetUserId();
                                    var group = new GroupResponseViewModel
                                    {
                                        Name = chat.GroupDetails.GroupName,
                                        Description = chat.GroupDetails.GroupDescription,
                                        PictureUrl = chat.GroupDetails.GroupPictureUrl,
                                        InterestId = chat.GroupDetails.InterestId,
                                        GroupId = chat.GroupDetails.GroupId,
                                        MemberCount = chat.GroupDetails.MemberCount,
                                        // m = chat.GroupDetails.UserId,
                                        Visibility = chat.GroupDetails.Visibility,
                                        Message = chat.GroupDetails.LastMessage,
                                        MessageTime = TimeZone.CurrentTimeZone.ToLocalTime(Convert.ToDateTime((DateTime)chat.LastActiveTime)),
                                        UserId = chat.GroupDetails.UserId
                                    };
                                    GroupRepository.SaveGroupCreated(group);
                                    db.Insert(conversationResponce.chatConversation);
                                    foreach (var member in chat.GroupMember)
                                    {

                                        GroupRepository.SaveorUpdateGroupMember(member, group.GroupId);

                                    }

                                }
                                else
                                {
                                    conversationResponce.chatConversation.LastMessage = chat.Message;
                                    conversationResponce.chatConversation.SenderName = chat.SenderName;
                                    conversationResponce.chatConversation.LastMessageDate = TimeZone.CurrentTimeZone.ToLocalTime(Convert.ToDateTime(chat.LastActiveTime));
                                    conversationResponce.chatConversation.OwnerId = CommonHelper.GetUserId(); //set my owner id from preference
                                    conversationResponce.chatConversation.UserId = CommonHelper.GetUserId();
                                    conversationResponce.chatConversation.GroupId = chat.GroupId;
                                    conversationResponce.chatConversation.IsGroup = chat.IsGroup;
                                    var group = new GroupResponseViewModel
                                    {
                                        Name = chat.GroupDetails.GroupName,
                                        Description = chat.GroupDetails.GroupDescription,
                                        PictureUrl = chat.GroupDetails.GroupPictureUrl,
                                        InterestId = chat.GroupDetails.InterestId,
                                        GroupId = chat.GroupDetails.GroupId,
                                        MemberCount = chat.GroupDetails.MemberCount,
                                        // m = chat.GroupDetails.UserId,
                                        Visibility = chat.GroupDetails.Visibility,
                                        Message = chat.Message,
                                        MessageTime = (DateTime)chat.LastActiveTime,
                                        UserId = chat.GroupDetails.UserId
                                    };
                                    GroupRepository.SaveGroupCreated(group);
                                    db.Update(conversationResponce.chatConversation);


                                }
                            }
                        }
                        db.Commit();
                    }
                    GlobalConstant.ChatInsertStart = true;
                }
                catch (Exception ex)
                {
                    GlobalConstant.ChatInsertStart = true;
                    Crashes.TrackError(ex);
                    Console.WriteLine(ex.Message, ex);
                }
                db.Close();
               
            }

        }
        public static ChatConversation GetConversationIdbyChatId(long chatid)
        {
            ChatConversation chatConv = null;
            var db = new SQLiteConnection(CommonConstant.DBPath);
            try
            {
                var res =
                    db.Query<ChatConversation>("select * from 'ChatConversation' where chatid=" + chatid + "  and UserId=" + CommonHelper.GetUserId()).ToList();

                var ch = res.FirstOrDefault();
                chatConv = ch;
                //Console.WriteLine("Chat Conversation Id => "+a.ChatId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
            }
            db.Close();
            return chatConv;
        }
        //public static ChatConversation GetConversationbyContactId(long contactId)
        //{
        //    ChatConversation chatConv = null;
        //    var db = new SQLiteConnection(CommonConstant.DBPath);
        //    try
        //    {
        //        chatConv =
        //            db.Query<ChatConversation>("select * from 'ChatConversation' where ContactId=" + contactId + " and UserId=" + CommonHelper.GetUserId())
        //                .FirstOrDefault();
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message, ex);
        //    }
        //    db.Close();

        //    return chatConv;
        //}

        public static ChatConversationSaveResponceViewModel GetConversationbyContactId(long contactId)
        {
            var chatConvResponce = new ChatConversationSaveResponceViewModel();
            var db = new SQLiteConnection(CommonConstant.DBPath);
            chatConvResponce.success = true;
            try
            {
                //chatConv = db.Query<ChatConversation> ("select * from 'ChatConversation' where ChatId=" + Chatid + " Order By ID Desc").FirstOrDefault ();
                chatConvResponce.chatConversation =
                                    db.Query<ChatConversation>("select * from 'ChatConversation' where ContactId=" + contactId + " and UserId=" + CommonHelper.GetUserId())
                        .ToList()
                        .FirstOrDefault();
            }
            catch (Exception ex)
            {
                chatConvResponce.success = false;
                Crashes.TrackError(ex);
                Console.WriteLine(ex.Message, ex);
            }
            db.Close();
            return chatConvResponce;
        }
        //public static ChatConversation GetConversationbyGroupId(long groupId)
        //{
        //    ChatConversation chatConv = null;
        //    var db = new SQLiteConnection(CommonConstant.DBPath);
        //    try
        //    {
        //        chatConv =
        //            db.Query<ChatConversation>("select * from 'ChatConversation' where GroupId=" + groupId + " and UserId=" + CommonHelper.GetUserId())
        //                .FirstOrDefault();
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message, ex);
        //    }
        //    db.Close();

        //    return chatConv;
        //}

        public static ChatConversationSaveResponceViewModel GetConversationbyGroupId(long groupId)
        {
            var chatConvResponce = new ChatConversationSaveResponceViewModel();
            var db = new SQLiteConnection(CommonConstant.DBPath);
            chatConvResponce.success = true;
            try
            {
                //chatConv = db.Query<ChatConversation> ("select * from 'ChatConversation' where ChatId=" + Chatid + " Order By ID Desc").FirstOrDefault ();
                chatConvResponce.chatConversation =
                                    db.Query<ChatConversation>("select * from 'ChatConversation' where GroupId=" + groupId + " and UserId=" + CommonHelper.GetUserId())
                        .ToList()
                        .FirstOrDefault();
            }
            catch (Exception ex)
            {
                chatConvResponce.success = false;
                Crashes.TrackError(ex);
                Console.WriteLine(ex.Message, ex);
            }
            db.Close();
            return chatConvResponce;
        }
        internal static List<ChatConversation> GetAllChat()
        {

            List<ChatConversation> lstmessages = null;
            var db = new SQLiteConnection(CommonConstant.DBPath);
            try
            {
                lstmessages = db.Query<ChatConversation>("select distinct * from 'ChatConversation' where  UserId=" + CommonHelper.GetUserId());


            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                Console.WriteLine(ex.Message, ex);
                lstmessages = new List<ChatConversation>();
            }
            db.Close();
            return lstmessages.OrderByDescending(m => m.LastMessageDate).ToList();

        }

        //public static ChatConversation GetConversationbyId(long Chatid, long Contactid)
        //{
        //    var chatConv = new ChatConversation();
        //    var db = new SQLiteConnection(CommonConstant.DBPath);

        //    try
        //    {
        //        //chatConv = db.Query<ChatConversation> ("select * from 'ChatConversation' where ChatId=" + Chatid + " Order By ID Desc").FirstOrDefault ();
        //        chatConv =
        //            db.Query<ChatConversation>("select * from 'ChatConversation' where ContactId=" + Contactid + " and UserId=" + CommonHelper.GetUserId())
        //                .ToList()
        //                .FirstOrDefault();
        //    }
        //    catch (Exception ex)
        //    {
        //        Crashes.TrackError(ex);
        //        Console.WriteLine(ex.Message, ex);
        //    }
        //    db.Close();
        //    return chatConv;
        //}

        public static ChatConversationSaveResponceViewModel GetConversationbyId(long Chatid, long Contactid)
        {
            var chatConvResponce = new ChatConversationSaveResponceViewModel();
            var db = new SQLiteConnection(CommonConstant.DBPath);
            chatConvResponce.success = true;
            try
            {
                //chatConv = db.Query<ChatConversation> ("select * from 'ChatConversation' where ChatId=" + Chatid + " Order By ID Desc").FirstOrDefault ();
                chatConvResponce.chatConversation =
                    db.Query<ChatConversation>("select * from 'ChatConversation' where ContactId=" + Contactid + " and UserId=" + CommonHelper.GetUserId())
                        .ToList()
                        .FirstOrDefault();
            }
            catch (Exception ex)
            {
                chatConvResponce.success = false;
                Crashes.TrackError(ex);
                Console.WriteLine(ex.Message, ex);
            }
            db.Close();
            return chatConvResponce;
        }

        //public static ChatConversation GetConversationbyGroupId(long Chatid, long GroupId)
        //{
        //    var chatConv = new ChatConversation();
        //    var db = new SQLiteConnection(CommonConstant.DBPath);

        //    try
        //    {
        //        //chatConv = db.Query<ChatConversation> ("select * from 'ChatConversation' where ChatId=" + Chatid + " Order By ID Desc").FirstOrDefault ();
        //        chatConv =
        //            db.Query<ChatConversation>("select * from 'ChatConversation' where GroupId=" + GroupId + " and UserId=" + CommonHelper.GetUserId())
        //                .ToList()
        //                .FirstOrDefault();
        //    }
        //    catch (Exception ex)
        //    {
        //        Crashes.TrackError(ex);
        //        Console.WriteLine(ex.Message, ex);
        //    }
        //    db.Close();
        //    return chatConv;
        //}

        public static ChatConversationSaveResponceViewModel GetConversationbyGroupId(long Chatid, long GroupId)
        {
            var chatConvResponce = new ChatConversationSaveResponceViewModel();
            var db = new SQLiteConnection(CommonConstant.DBPath);
            chatConvResponce.success = true;
            try
            {
                //chatConv = db.Query<ChatConversation> ("select * from 'ChatConversation' where ChatId=" + Chatid + " Order By ID Desc").FirstOrDefault ();
                chatConvResponce.chatConversation =
                                    db.Query<ChatConversation>("select * from 'ChatConversation' where GroupId=" + GroupId + " and UserId=" + CommonHelper.GetUserId())
                        .ToList()
                        .FirstOrDefault();
            }
            catch (Exception ex)
            {
                chatConvResponce.success = false;
                Crashes.TrackError(ex);
                Console.WriteLine(ex.Message, ex);
            }
            db.Close();
            return chatConvResponce;
        }

        public static void SaveConverstionNewFromServer(ChatModel chatViewModel)
        {
           
                var db = new SQLiteConnection(CommonConstant.DBPath);
                ChatConversation mdchatconversation = null;
            try
            {
                var chatconversationRespinse = GetConversationbyContactId((long)chatViewModel.ContactId);
                if(chatconversationRespinse.success)
                {
                    if (chatconversationRespinse.chatConversation == null)
                {
                    chatconversationRespinse.chatConversation = new ChatConversation
                    {
                        ContactId = chatViewModel.ContactId,
                        LastMessage = chatViewModel.Message,
                        LastMessageDate = TimeZone.CurrentTimeZone.ToLocalTime(Convert.ToDateTime(chatViewModel.LastActiveTime)),
                        ChatId = chatViewModel.ChatId,
                        IsGroup = false,
                        UserId = CommonHelper.GetUserId()
                    };
                        db.Insert(chatconversationRespinse.chatConversation);
                        mdchatconversation = chatconversationRespinse.chatConversation;

                }
                else
                {
                        chatconversationRespinse.chatConversation.LastMessage = chatViewModel.Message;
                        chatconversationRespinse.chatConversation.LastMessageDate = TimeZone.CurrentTimeZone.ToLocalTime(Convert.ToDateTime(chatViewModel.LastActiveTime));
                        chatconversationRespinse.chatConversation.IsGroup = false;
                        chatconversationRespinse.chatConversation.UserId = CommonHelper.GetUserId();
                        db.Update(chatconversationRespinse.chatConversation);
                        mdchatconversation = chatconversationRespinse.chatConversation;

                }
                db.Commit();
            }
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                    Console.WriteLine(ex.Message, ex);
                }
                db.Close();

        }

        public static ChatConversation SaveGroupConverstionNewFromServer(ChatModel chatViewModel)
        {
            var db = new SQLiteConnection(CommonConstant.DBPath);
            ChatConversation mdchatconversation = null;
            try
            {
               
                var chatConversationResponce = GetConversationbyGroupId((long)chatViewModel.GroupId);
                if (chatConversationResponce.success)
                   {
                    if (chatConversationResponce.chatConversation == null)
                    {
                        chatConversationResponce.chatConversation = new ChatConversation
                        {
                            GroupId = chatViewModel.GroupId,
                            LastMessage = chatViewModel.Message,
                            LastMessageDate = TimeZone.CurrentTimeZone.ToLocalTime(Convert.ToDateTime(chatViewModel.LastActiveTime)),
                            ChatId = chatViewModel.ChatId,
                            IsGroup = true,
                            UserId = CommonHelper.GetUserId()
                        };
                        db.Insert(chatConversationResponce.chatConversation);
                        mdchatconversation = chatConversationResponce.chatConversation;
                    }
                    else
                    {
                        chatConversationResponce.chatConversation.LastMessage = chatViewModel.Message;
                        chatConversationResponce.chatConversation.LastMessageDate = TimeZone.CurrentTimeZone.ToLocalTime(Convert.ToDateTime(chatViewModel.LastActiveTime));
                        chatConversationResponce.chatConversation.IsGroup = true;
                        chatConversationResponce.chatConversation.UserId = CommonHelper.GetUserId();
                        db.Update(chatConversationResponce.chatConversation);
                        mdchatconversation = chatConversationResponce.chatConversation;
                    }
            }
                    db.Commit();
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                    Console.WriteLine(ex.Message, ex);
                }
                db.Close();
                return mdchatconversation;

        }

        public static ChatConversation UpdateChatLastMessage(long id, string message, string senderName)
        {
            var db = new SQLiteConnection(CommonConstant.DBPath);
            var chatConversation = GetConversationbyConversationId(id);
            try
            {
                if (chatConversation != null)
                {
                    chatConversation.LastMessage = message;
                    chatConversation.SenderName = senderName;
                    chatConversation.LastMessageDate = TimeZone.CurrentTimeZone.ToLocalTime(DateTime.UtcNow);
                    chatConversation.UserId = CommonHelper.GetUserId();
                    db.Update(chatConversation);
                }
            }
            catch (Exception exp)
            {
                Crashes.TrackError(exp);
                //  CommonHelper.PrintException("Err updating last msg chat", exp);
            }
            db.Commit();
            db.Close();
            return chatConversation;
        }
        public static ChatConversation GetConversationbyConversationId(long id)
        {
            var chatConv = new ChatConversation();
            var db = new SQLiteConnection(CommonConstant.DBPath);

            try
            {
                //chatConv = db.Query<ChatConversation> ("select * from 'ChatConversation' where ChatId=" + Chatid + " Order By ID Desc").FirstOrDefault ();
                chatConv =
                    db.Query<ChatConversation>("select * from 'ChatConversation' where id=" + id + " and UserId=" + CommonHelper.GetUserId())
                        .ToList()
                        .FirstOrDefault();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                Console.WriteLine(ex.Message, ex);
            }
            db.Close();
            return chatConv;
        }
        public static void UpdateBlock(long ChatId)
        {

            var db = new SQLiteConnection(CommonConstant.DBPath);

            try
            {
                if (ChatId > 0)
                {
                    var ChatModel = GetConversationbyConversationId(ChatId);
                    ChatModel.IsBlock = true;
                    db.Update(ChatModel);
                    db.Commit();
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                Console.WriteLine(ex.Message, ex);
            }
            db.Close();

        }

        public static List<ChatConversation> GetAllBlockList()
        {

            List<ChatConversation> lstmessages = null;
            var db = new SQLiteConnection(CommonConstant.DBPath);
            try
            {
                lstmessages = db.Query<ChatConversation>("select * from 'ChatConversation' where IsBlock =1 and UserId=" + CommonHelper.GetUserId());


            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                Console.WriteLine(ex.Message, ex);
                lstmessages = new List<ChatConversation>();
            }
            db.Close();
            return lstmessages.OrderByDescending(m => m.LastMessageDate).ToList();

        }

        public static void UpdateUnBlock(long ChatId)
        {

            var db = new SQLiteConnection(CommonConstant.DBPath);

            try
            {
                if (ChatId > 0)
                {
                    var ChatModel = GetConversationbyConversationId(ChatId);
                    ChatModel.IsBlock = false;
                    db.Update(ChatModel);
                    db.Commit();
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                Console.WriteLine(ex.Message, ex);
            }
            db.Close();

        }

    }
}