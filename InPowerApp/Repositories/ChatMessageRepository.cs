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
    public class ChatMessageRepository
    {
        public static Object thisLock = new Object();

        public static void SaveTextMessage(ChatMessage message)
        {
            var db = new SQLiteConnection(CommonConstant.DBPath);
            try
            {
                db.Insert(message);
                db.Commit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
            }
            db.Close();
        }

        public static ChatMessage SaveChatMessage(ChatMessageViewModel model, long ChatId)
        {
           
                var db = new SQLiteConnection(CommonConstant.DBPath);

                ChatMessage chatMessage = null;
                try
            {
                lock (thisLock)
                {
                    if (chatMessage == null)
                    {
                        chatMessage = new ChatMessage();
                        chatMessage.ContactId = model.ContactId;
                        chatMessage.ChatId = ChatId;
                        chatMessage.ChatMessageId = model.ChatMessageId;
                        chatMessage.MessageText = model.Message;
                        chatMessage.MessageTime = model.MessageTime;
                        chatMessage.IsRead = model.IsRead;
                        chatMessage.IsRecieved = model.IsRecieved;
                        chatMessage.IsSend = model.IsSend;
                        chatMessage.UserId = CommonHelper.GetUserId();
                        db.Insert(chatMessage);
                        if (model.Attachments.Count > 0)
                        {
                            ChatAttachmentRepository.SaveChatAttachment(model.Attachments, model.ContactId, model.ChatMessageId);
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
                return chatMessage;
        }
        public static ChatMessage updateChatMessage(ChatMessageViewModel model)
        {
            var db = new SQLiteConnection(CommonConstant.DBPath);

            ChatMessage chatMessage = getmessagebyid(model.MobiledatabaseId);
            try
            {
                if (chatMessage != null)
                {
                    chatMessage.ChatMessageId = model.ChatMessageId;
                    chatMessage.IsRecieved = model.IsRecieved;
                    chatMessage.IsRead = model.IsRead;
                    chatMessage.IsSend = model.IsSend;
                    chatMessage.MessageTime = model.MessageTime;
                    chatMessage.UserId = CommonHelper.GetUserId();
                    db.Update(chatMessage);
                    if (model.Attachments.Count > 0)
                    {
                        ChatAttachmentRepository.SaveChatAttachment(model.Attachments, model.ContactId, model.ChatMessageId);
                    }
                }
                db.Commit();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
            }
            db.Close();
            return chatMessage;
        }

        public static int getChatMessageUnRead(long chatId)
        {
            int Count = 0;
            var db = new SQLiteConnection(CommonConstant.DBPath);
            try
            {
                var uid = CommonHelper.GetUserId();
              Count =
                   db.Query<int>("select * from 'ChatMessage' where (IsRead=0 and ChatId=" + chatId + " and ContactId =" + CommonHelper.GetUserId() + " and UserId =" + CommonHelper.GetUserId() + ")").Count();
                //Count =
                //    db.Query<int>("select * from 'ChatMessage' where (IsRead=0 and ChatId=" + chatId + " and ContactId =" + CommonHelper.GetUserId()+") OR ()").Count();


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
            }
            db.Close();
            return Count;
        }

        private static ChatMessage getmessagebyid(long Id)
        {
            ChatMessage chat = null;
            var db = new SQLiteConnection(CommonConstant.DBPath);
            try
            {
                chat =
                    db.Query<ChatMessage>("select * from 'ChatMessage' where id=" + Id + " and UserId=" + CommonHelper.GetUserId())
                        .ToList()
                        .FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
            }
            db.Close();
            return chat;
        }
        public static List<ChatMessage> SaveChatMessages(List<ChatMessageViewModel> lstChatMessages, long chatConvId)
        {
            var db = new SQLiteConnection(CommonConstant.DBPath);
            ChatMessage chatMessage = null;

            var lstMessages = new List<ChatMessage>();
            try
            {
                foreach (var chat in lstChatMessages)
                {
                    try
                    {
                        chatMessage = CheckMessage(chat.ChatMessageId);
                        if (chatMessage == null)
                        {

                            SaveChatMessage(chat, chatConvId);


                            //chatMessage = new ChatMessage();
                            //chatMessage.ContactId = chat.ContactId;
                            //chatMessage.ChatId = chatConvId;
                            //chatMessage.ChatMessageId = chat.ChatMessageId;
                            //chatMessage.MessageText = chat.Message;
                            //chatMessage.MessageTime = chat.MessageTime;
                            //chatMessage.IsRead = chat.IsRead;
                            //chatMessage.IsRecieved = chat.IsRecieved;
                            //chatMessage.IsSend = chat.IsSend;
                            //chatMessage.UserId = CommonHelper.GetUserId();
                            //db.Insert(chatMessage);
                            //lstMessages.Add(chatMessage);
                            //if (chat.Attachments.Count > 0)
                            //{
                            //    ChatAttachmentRepository.SaveChatAttachment(chat.Attachments, chat.ContactId,
                            //        chat.ChatMessageId);
                            //}
                            ////
                        }
                        else
                        {
                            chatMessage.ChatMessageId = chat.ChatMessageId;
                            chatMessage.IsRecieved = chat.IsRecieved;
                            chatMessage.IsRead = chat.IsRead;
                            chatMessage.IsSend = chat.IsSend;
                            chatMessage.MessageTime = chat.MessageTime;
                            chatMessage.UserId = CommonHelper.GetUserId();
                            db.Update(chatMessage);
                        }
                        db.Commit();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message, ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
                lstMessages = new List<ChatMessage>();
            }
            db.Close();
            return lstMessages;
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
        public static Dictionary<DateTime, List<ChatMessage>> GetChatMessagesForPageIndex(PaginationModel paginationModel, long chatId)
        {
            Dictionary<DateTime, List<ChatMessage>> lstmessages = null;
            var db = new SQLiteConnection(CommonConstant.DBPath);
            try
            {
                db.Trace = true;


                var l = db.Query<ChatMessage>("select * from 'ChatMessage' where ChatId=" + chatId + " and UserId=" + CommonHelper.GetUserId())

                    .OrderBy(m => m.MessageTime.Ticks).TakeLast(paginationModel.SkipRecords + paginationModel.TakeRecords).ToList();
                //.Skip(pageIndex * CommonConstant.MessagePageSize)
                //.Take(CommonConstant.MessagePageSize);
                var lmsg = l.GroupBy(a => a.MessageTime.Date).ToDictionary(a => a.Key, a => a.ToList());
                return lmsg;


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
                lstmessages = new Dictionary<DateTime, List<ChatMessage>>();
            }
            db.Close();
            return lstmessages;
        }

        public static ChatMessage CheckMessage(long chatMessageId)
        {

            var db = new SQLiteConnection(CommonConstant.DBPath);


            var chatRecord = db.Query<ChatMessage>("select * from 'ChatMessage' where ChatMessageId=" + chatMessageId + " and UserId=" + CommonHelper.GetUserId()).FirstOrDefault();


            db.Close();

            return chatRecord;
        }
    }
}