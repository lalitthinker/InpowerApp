using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using InPowerApp.Common;
using InPowerApp.Model;
using PCL.Model;
using SQLite;

namespace InPowerApp.Repositories
{
    public class ChatConversationRepository
    {
        public static void SaveChatConversationFromServer(List<ChatModel> chats)
        {
            var db = new SQLiteConnection(CommonConstant.DBPath);
            ChatConversation mdchatconversation = null;
            try
            {
                foreach (var chat in chats)
                {
                    if (chat.IsGroup == false)
                    {
                        mdchatconversation = GetConversationbyId(chat.ChatId, (long)chat.ContactId);
                        if (mdchatconversation == null)
                        {
                            mdchatconversation = new ChatConversation();
                            mdchatconversation.LastMessage = chat.Message;
                            mdchatconversation.ChatId = chat.ChatId;
                            mdchatconversation.ContactId = chat.ContactId;
                            mdchatconversation.LastMessageDate = chat.LastActiveTime;
                            mdchatconversation.OwnerId = CommonHelper.GetUserId();  //set my owner id from preference
                            mdchatconversation.UserId = CommonHelper.GetUserId();
                            var contact = new Contact
                            {
                                name = chat.ContactProfile.FirstName + chat.ContactProfile.LastName,
                                contactId = chat.ContactProfile.UserId,
                                contactPicUrl = chat.ContactProfile.ProfileImageUrl,
                                Aboutme = chat.ContactProfile.AboutMe,
                                city = chat.ContactProfile.City,
                                state = chat.ContactProfile.State,
                                email = chat.email,
                                screenName = chat.ContactProfile.FirstName + " " + chat.ContactProfile.LastName,
                                UserId = CommonHelper.GetUserId()
                            };
                            ContactRepository.SaveorUpdateContact(contact);
                            db.Insert(mdchatconversation);
                        }
                        else
                        {
                            mdchatconversation.LastMessage = chat.Message;
                            mdchatconversation.SenderName = chat.SenderName;
                            mdchatconversation.LastMessageDate = chat.LastActiveTime;
                            mdchatconversation.OwnerId = CommonHelper.GetUserId(); //set my owner id from preference
                            mdchatconversation.UserId = CommonHelper.GetUserId();
                            var contact = new Contact
                            {
                                name = chat.ContactProfile.FirstName + " " + chat.ContactProfile.LastName,
                                contactId = chat.ContactProfile.UserId,
                                contactPicUrl = chat.ContactProfile.ProfileImageUrl,
                                Aboutme = chat.ContactProfile.AboutMe,
                                city = chat.ContactProfile.City,
                                state = chat.ContactProfile.State,
                                email = chat.email,
                                screenName = chat.ContactProfile.FirstName + " " + chat.ContactProfile.LastName,
                                UserId = CommonHelper.GetUserId()
                            };
                            ContactRepository.SaveorUpdateContact(contact);
                            db.Update(mdchatconversation);
                        }
                    }
                    else
                    {
                        mdchatconversation = GetConversationbyGroupId((long)chat.GroupId);
                        if (mdchatconversation == null)
                        {
                            mdchatconversation = new ChatConversation();
                            mdchatconversation.LastMessage = chat.Message;
                            mdchatconversation.SenderName = chat.SenderName;
                            mdchatconversation.ChatId = chat.ChatId;
                            mdchatconversation.GroupId = chat.GroupId;
                            mdchatconversation.IsGroup = chat.IsGroup;
                            mdchatconversation.LastMessageDate = chat.LastActiveTime;
                            mdchatconversation.OwnerId = CommonHelper.GetUserId();  //set my owner id from preference
                            mdchatconversation.UserId = CommonHelper.GetUserId();
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
                                MessageTime = (DateTime)chat.LastActiveTime,
                                UserId = chat.GroupDetails.UserId
                            };
                             GroupRepository.SaveGroupCreated(group);
                            db.Insert(mdchatconversation);
                            foreach(var member in chat.GroupMember)
                            {

                                GroupRepository.SaveorUpdateGroupMember(member, group.GroupId);

                            }
                          
                        }
                        else
                        {
                            mdchatconversation.LastMessage = chat.Message;
                            mdchatconversation.SenderName = chat.SenderName;
                            mdchatconversation.LastMessageDate = chat.LastActiveTime;
                            mdchatconversation.OwnerId = CommonHelper.GetUserId(); //set my owner id from preference
                            mdchatconversation.UserId = CommonHelper.GetUserId();
                            mdchatconversation.GroupId = chat.GroupId;
                            mdchatconversation.IsGroup = chat.IsGroup;
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
                            db.Update(mdchatconversation);

                           
                        }
                    }
                    db.Commit();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
            }
            db.Close();
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
        public static ChatConversation GetConversationbyContactId(long contactId)
        {
            ChatConversation chatConv = null;
            var db = new SQLiteConnection(CommonConstant.DBPath);
            try
            {
                chatConv =
                    db.Query<ChatConversation>("select * from 'ChatConversation' where ContactId=" + contactId + " and UserId=" + CommonHelper.GetUserId())
                        .FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
            }
            db.Close();

            return chatConv;
        }
        public static ChatConversation GetConversationbyGroupId(long groupId)
        {
            ChatConversation chatConv = null;
            var db = new SQLiteConnection(CommonConstant.DBPath);
            try
            {
                chatConv =
                    db.Query<ChatConversation>("select * from 'ChatConversation' where GroupId=" + groupId + " and UserId=" + CommonHelper.GetUserId())
                        .FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
            }
            db.Close();

            return chatConv;
        }
        internal static List<ChatConversation> GetAllChat()
        {

            List<ChatConversation> lstmessages = null;
            var db = new SQLiteConnection(CommonConstant.DBPath);
            try
            {
                lstmessages = db.Query<ChatConversation>("select * from 'ChatConversation' where  UserId=" + CommonHelper.GetUserId());


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
                lstmessages = new List<ChatConversation>();
            }
            db.Close();
            return lstmessages.OrderByDescending(m => m.LastMessageDate).ToList();

        }

        public static ChatConversation GetConversationbyId(long Chatid, long Contactid)
        {
            var chatConv = new ChatConversation();
            var db = new SQLiteConnection(CommonConstant.DBPath);

            try
            {
                //chatConv = db.Query<ChatConversation> ("select * from 'ChatConversation' where ChatId=" + Chatid + " Order By ID Desc").FirstOrDefault ();
                chatConv =
                    db.Query<ChatConversation>("select * from 'ChatConversation' where ContactId=" + Contactid + " and UserId=" + CommonHelper.GetUserId())
                        .ToList()
                        .FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
            }
            db.Close();
            return chatConv;
        }
        public static ChatConversation GetConversationbyGroupId(long Chatid, long GroupId)
        {
            var chatConv = new ChatConversation();
            var db = new SQLiteConnection(CommonConstant.DBPath);

            try
            {
                //chatConv = db.Query<ChatConversation> ("select * from 'ChatConversation' where ChatId=" + Chatid + " Order By ID Desc").FirstOrDefault ();
                chatConv =
                    db.Query<ChatConversation>("select * from 'ChatConversation' where GroupId=" + GroupId + " and UserId=" + CommonHelper.GetUserId())
                        .ToList()
                        .FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
            }
            db.Close();
            return chatConv;
        }

        public static void SaveConverstionNewFromServer(ChatModel chatViewModel)
        {
            var db = new SQLiteConnection(CommonConstant.DBPath);
            ChatConversation mdchatconversation = null;
            try
            {
                mdchatconversation = GetConversationbyContactId((long) chatViewModel.ContactId);
                if (mdchatconversation == null)
                {
                    mdchatconversation = new ChatConversation
                    {
                        ContactId = chatViewModel.ContactId,
                        LastMessage = chatViewModel.Message,
                        LastMessageDate = chatViewModel.LastActiveTime,
                        ChatId = chatViewModel.ChatId,
                        IsGroup = false,
                        UserId = CommonHelper.GetUserId()
                    };
                    db.Insert(mdchatconversation);
                }
                else
                {
                    mdchatconversation.LastMessage = chatViewModel.Message;
                    mdchatconversation.LastMessageDate = chatViewModel.LastActiveTime;
                    mdchatconversation.IsGroup = false;
                    mdchatconversation.UserId = CommonHelper.GetUserId();
                    db.Update(mdchatconversation);
                }
                db.Commit();
            }
            catch (Exception ex)
            {
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
                mdchatconversation = GetConversationbyGroupId((long)chatViewModel.GroupId);
                if (mdchatconversation == null)
                {
                    mdchatconversation = new ChatConversation
                    {
                        GroupId = chatViewModel.GroupId,
                        LastMessage = chatViewModel.Message,
                        LastMessageDate = chatViewModel.LastActiveTime,
                        ChatId = chatViewModel.ChatId,
                        IsGroup = true,
                        UserId = CommonHelper.GetUserId()
                    };
                    db.Insert(mdchatconversation);
                }
                else
                {
                    mdchatconversation.LastMessage = chatViewModel.Message;
                    mdchatconversation.LastMessageDate = chatViewModel.LastActiveTime;
                    mdchatconversation.IsGroup = true;
                    mdchatconversation.UserId = CommonHelper.GetUserId();
                    db.Update(mdchatconversation);
                }
                db.Commit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
            }
            db.Close();
            return mdchatconversation;
        }

        public static ChatConversation UpdateChatLastMessage(long id, string message,string senderName)
        {
            var db = new SQLiteConnection(CommonConstant.DBPath);
            var chatConversation = GetConversationbyConversationId(id);
            try
            {
                if (chatConversation != null)
                {
                    chatConversation.LastMessage = message;
                    chatConversation.SenderName = senderName;
                    chatConversation.LastMessageDate = DateTime.UtcNow.ToUniversalTime();
                    chatConversation.UserId = CommonHelper.GetUserId();
                    db.Update(chatConversation);
                }
            }
            catch (Exception exp)
            {
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
                Console.WriteLine(ex.Message, ex);
            }
            db.Close();
            return chatConv;
        }
        public static void UpdateBlock(long ChatId,bool BlockVal)
        {
           
            var db = new SQLiteConnection(CommonConstant.DBPath);

            try
            {
                if (ChatId >0)
                {
                    var ChatModel = GetConversationbyConversationId(ChatId);
                    ChatModel.IsBlock = BlockVal;
                    db.Update(ChatModel);
                    db.Commit();
                }
            }
            catch (Exception ex)
            {
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
                Console.WriteLine(ex.Message, ex);
            }
            db.Close();

        }

    }
}