using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InPowerIOS.Model;
using PCL.Model;
using InPowerIOS.Common;
using SQLite;
using Microsoft.AppCenter.Crashes;

namespace InPowerIOS.Repositories
{
    public class ChatAttachmentRepository
    {
        public static void SaveChatAttachment(List<AttachmentViewModel> lstattachment, long contactId, long messageId)
        {
            var db = new SQLiteConnection(CommonConstant.DBPath);
            try
            {
                ChatAttachment chatAttachment = null;
                foreach (var cMsgAttachment in lstattachment)
                {
                    chatAttachment = new ChatAttachment();
                    chatAttachment.url = cMsgAttachment.Url;
                    chatAttachment.type = cMsgAttachment.Type;
                    chatAttachment.chatMessageId = messageId;
                    chatAttachment.contactId = contactId;
                    chatAttachment.UserId = CommonHelper.GetUserId();
                    db.Insert(chatAttachment);
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
        public static List<ChatAttachment> GetChatAttachList(long messageId)
        {
            var db = new SQLiteConnection(CommonConstant.DBPath);
            List<ChatAttachment> LstAttach;
            try
            {

                LstAttach =
                      db.Query<ChatAttachment>("select * from 'ChatAttachment' where chatMessageId=" + messageId + " and UserId=" + CommonHelper.GetUserId())
                          .ToList();

            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                Console.WriteLine(ex.Message, ex);
                return LstAttach = new List<ChatAttachment>();
            }
            db.Close();
            return LstAttach;
        }
    }
}