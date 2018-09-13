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
    public class GroupRepository
    {
        public static void SaveGroupCreated(GroupResponseViewModel grp)
        {
            var db = new SQLiteConnection(CommonConstant.DBPath);
            try
            {
                var group = GetGroupByID(grp.GroupId);
                if (group == null)
                {
                    group = new GroupModel();
                    group.GroupName = grp.Name;
                    group.GroupDescription = grp.Description;
                    group.GroupPictureUrl = grp.PictureUrl;
                    group.InterestId = grp.InterestId;
                    group.GroupId = grp.GroupId;
                    group.MemberCount = grp.MemberCount;
                    group.OwnerId = grp.UserId;
                    group.Visibility = grp.Visibility;
                    group.LastMessage = grp.Message;
                    group.LastMessageDate = grp.MessageTime;
                    group.UserId = CommonHelper.GetUserId();
                    db.Insert(group);
                }
                else
                {
                    group.GroupName = grp.Name;
                    group.GroupDescription = grp.Description;
                    group.GroupPictureUrl = grp.PictureUrl;
                    group.InterestId = grp.InterestId;
                    group.MemberCount = grp.MemberCount;
                    group.OwnerId = grp.UserId;
                    group.Visibility = grp.Visibility;
                    group.LastMessage = grp.Message;
                    group.LastMessageDate = grp.MessageTime;
                    group.UserId = CommonHelper.GetUserId();
                    db.Update(group);
                }
                db.Commit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
            }
            db.Close();
        }

     

        public static Dictionary<DateTime, List<GroupMessage>> GetGroupMessagesForPageIndex(PaginationModel paginationModel, long groupId)
        {
            Dictionary<DateTime, List<GroupMessage>> lstmessages = null;
            var db = new SQLiteConnection(CommonConstant.DBPath);
            try
            {
                db.Trace = true;


                var l = db.Query<GroupMessage>("select * from 'GroupMessage' where GroupId=" + groupId + " and UserId=" + CommonHelper.GetUserId())

                    .OrderBy(m => m.MessageTime.Ticks).TakeLast(paginationModel.SkipRecords + paginationModel.TakeRecords).ToList();
                //.Skip(pageIndex * CommonConstant.MessagePageSize)
                //.Take(CommonConstant.MessagePageSize);
                var lmsg = l.GroupBy(a => a.MessageTime.Date).ToDictionary(a => a.Key, a => a.ToList());
                return lmsg;


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
                lstmessages = new Dictionary<DateTime, List<GroupMessage>>();
            }
            db.Close();
            return lstmessages;
        }


        public static GroupModel GetGroupByID(long GroupId)
        {
            GroupModel group = null;
            var db = new SQLiteConnection(CommonConstant.DBPath);
            try
            {
                group =
                    db.Query<GroupModel>("select * from 'GroupModel' where GroupId=" + GroupId + " and UserId=" + CommonHelper.GetUserId()).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
            }
            db.Close();
            return group;
        }

        public static GroupMessage CheckGroupChatMessage(long messageid, long groupId)
        {
            GroupMessage chat = null;
            var db = new SQLiteConnection(CommonConstant.DBPath);
            try
            {
                chat =
                    db.Query<GroupMessage>("select * from 'GroupMessage' where MessageId =" + messageid + " and GroupId=" + groupId + " and UserId=" + CommonHelper.GetUserId())

                        .FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
            }
            db.Close();
            return chat;
        }

        public static int getGroupMessageUnRead(long groupId)
        {
            int Count = 0;
            var db = new SQLiteConnection(CommonConstant.DBPath);
            try
            {
                var uid = CommonHelper.GetUserId();
                Count =
                     db.Query<int>("select * from 'GroupMessage' where (IsRead=0 and GroupId=" + groupId + " and SenderUserId <> " + CommonHelper.GetUserId() + " and UserId =" + CommonHelper.GetUserId() + ")").Count();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
            }
            db.Close();
            return Count;
        }


  
       
        public static List<GroupMessage> SaveGroupMessageFromList(List<GroupMessageResponseViewModel> lstgrpMessage)
        {
            var db = new SQLiteConnection(CommonConstant.DBPath);
            GroupMessage groupMessage = null;
            var lstMessages = new List<GroupMessage>();
            try
            {
                foreach (var grpMsg in lstgrpMessage)
                {
                    groupMessage = GetGroupMessageID(grpMsg.MessageId);
                    if (groupMessage == null)
                    {
                        groupMessage = new GroupMessage();
                        groupMessage.GroupId = grpMsg.GroupId;
                        groupMessage.MessageId = grpMsg.MessageId;
                        groupMessage.MessageText = grpMsg.Message;
                        groupMessage.MessageTime = grpMsg.MessageTime;
                        groupMessage.senderName = grpMsg.SenderProfileName;
                        groupMessage.senderPicUrl = grpMsg.SenderProfilePicUrl;
                        groupMessage.SenderUserId = grpMsg.SenderId;
                        groupMessage.UserId = CommonHelper.GetUserId();
                        groupMessage.IsRead = true;
                        db.Insert(groupMessage);
                        lstMessages.Add(groupMessage);
                        SaveAttachmentFromServer(grpMsg.Attachments, grpMsg.GroupId,
                            grpMsg.MessageId);

                        if (grpMsg.GroupMessageStatuses.Count > 0)
                        {
                            SaveOrUpdateGroupMessageStatusFromServer(grpMsg.GroupMessageStatuses,
                           grpMsg.MessageId);
                        }
                    }
                    else
                    {
                        groupMessage.IsRead = true;
                        db.Update(groupMessage);
                        if (grpMsg.GroupMessageStatuses.Count > 0)
                        {
                            SaveOrUpdateGroupMessageStatusFromServer(grpMsg.GroupMessageStatuses,
                           grpMsg.MessageId);
                        }
                    }

                }
                db.Commit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
            }
            db.Close();
            return lstMessages;
        }


        public static GroupMessage UpdateGroupMessage(GroupMessageResponseViewModel grpMessage)
        {
            var db = new SQLiteConnection(CommonConstant.DBPath);
            GroupMessage grpMsg = null;
            try
            {
                if (grpMessage != null)
                {
                    grpMsg = getGroupMessagebyid(grpMessage.MobileDatabaseId);
                    if (grpMsg != null)
                    {

                        grpMsg.GroupId = grpMessage.GroupId;
                        grpMsg.MessageId = grpMessage.MessageId;
                        grpMsg.MessageText = grpMessage.Message;
                        grpMsg.MessageTime = grpMessage.MessageTime;
                        grpMsg.senderName = grpMessage.SenderProfileName;
                        grpMsg.senderPicUrl = grpMessage.SenderProfilePicUrl;
                        grpMsg.SenderUserId = grpMessage.SenderId;
                        grpMsg.UserId = CommonHelper.GetUserId();


                        db.Update(grpMsg);
                        if (grpMessage.Attachments.Count > 0)
                        {
                            SaveAttachmentFromServer(grpMessage.Attachments, grpMsg.GroupId,
                           grpMsg.MessageId);
                        }

                        if (grpMessage.GroupMessageStatuses.Count > 0)
                        {
                            SaveOrUpdateGroupMessageStatusFromServer(grpMessage.GroupMessageStatuses,
                           grpMsg.MessageId);
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
            return grpMsg;
        }
        private static GroupMessage getGroupMessagebyid(long Id)
        {
            GroupMessage chat = null;
            var db = new SQLiteConnection(CommonConstant.DBPath);
            try
            {
                chat =
                    db.Query<GroupMessage>("select * from 'GroupMessage' where id=" + Id + " and UserId=" + CommonHelper.GetUserId())
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
        public static GroupMessage SaveGroupMessage(GroupMessageResponseViewModel grpMessage, string From)
        {
            var db = new SQLiteConnection(CommonConstant.DBPath);
            GroupMessage grpMsg = null;
            try
            {
                if (grpMessage != null)
                {
                    grpMsg = new GroupMessage
                    {
                        GroupId = grpMessage.GroupId,
                        MessageId = grpMessage.MessageId,
                        MessageText = grpMessage.Message,
                        MessageTime = grpMessage.MessageTime,
                        senderName = grpMessage.SenderProfileName,
                        senderPicUrl = grpMessage.SenderProfilePicUrl,
                        SenderUserId = grpMessage.SenderId,
                        IsRead = (From == "Private") ? true : false,
                        UserId = CommonHelper.GetUserId()

                    };
                    db.Insert(grpMsg);
                    if (grpMessage.Attachments.Count > 0)
                    {
                        SaveAttachmentFromServer(grpMessage.Attachments,
                            grpMessage.GroupId, grpMessage.MessageId);
                    }

                    db.Commit();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
            }
            db.Close();
            return grpMsg;
        }
        public static GroupMessage GetGroupMessageID(long? grpMsgId)
        {
            GroupMessage groupMsg = null;
            var db = new SQLiteConnection(CommonConstant.DBPath);
            try
            {
                groupMsg =
                    db.Query<GroupMessage>("select * from 'GroupMessage' where MessageId=" + grpMsgId + " and UserId=" + CommonHelper.GetUserId())
                        .ToList()
                        .FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
            }
            db.Close();
            return groupMsg;
        }
     
        public static void SaveAttachmentFromServer(List<AttachmentViewModel> lstattachments, long groupId,long messageId)
        {
            var db = new SQLiteConnection(CommonConstant.DBPath);
            GroupAttachment groupAttachment = null;
            try
            {
                foreach (var grpAttachment in lstattachments)
                {
                    if (groupAttachment == null)
                    {
                        groupAttachment = new GroupAttachment();
                        groupAttachment.url = grpAttachment.Url;
                        groupAttachment.type = grpAttachment.Type;
                        groupAttachment.groupMessageId = messageId;
                        groupAttachment.groupId = groupId;
                        groupAttachment.UserId = CommonHelper.GetUserId();
                        db.Insert(groupAttachment);
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

        public static void SaveOrUpdateGroupMessageStatusFromServer(List<GroupMessageStatusViewModel> lstGroupMessageStatus,long messageId)
        {
            var db = new SQLiteConnection(CommonConstant.DBPath);
            try
            {
                foreach (var grpMessageStatus in lstGroupMessageStatus)
                {
                    UpdateGroupMessageStatus(grpMessageStatus);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
            }
            db.Close();
        }

        public static List<GroupAttachment> GetGroupMessageAttachList(long messageId)
        {
            var db = new SQLiteConnection(CommonConstant.DBPath);
            List<GroupAttachment> LstAttach;
            try
            {

                LstAttach =
                      db.Query<GroupAttachment>("select * from 'GroupAttachment' where groupMessageId=" + messageId + "  and UserId=" + CommonHelper.GetUserId())
                          .ToList();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
                return LstAttach = new List<GroupAttachment>();
            }
            db.Close();
            return LstAttach;
        }

        public static GroupMessageStatus UpdateGroupMessageStatus(GroupMessageStatusViewModel grpMessage)
        {
            var db = new SQLiteConnection(CommonConstant.DBPath);
            GroupMessageStatus grpMsg = null;
            try
            {
                if (grpMessage != null)
                {
                    grpMsg = getGroupMessageStatusbyidAndReceiver(grpMessage.GroupMessageId, grpMessage.ReceiverId);
                    if (grpMsg != null)
                    {
                        grpMsg.IsSend = grpMessage.IsSend;
                        grpMsg.IsRead = grpMessage.IsRead;
                        grpMsg.IsRecieved = grpMessage.IsRecieved;
                        grpMsg.UserId = CommonHelper.GetUserId();
                        db.Update(grpMsg);
                    }
                    else
                    {
                        grpMsg = new GroupMessageStatus();
                        grpMsg.GroupMessageId = grpMessage.GroupMessageId;
                        grpMsg.IsSend = grpMessage.IsSend;
                        grpMsg.IsRead = grpMessage.IsRead;
                        grpMsg.IsRecieved = grpMessage.IsRecieved;
                        grpMsg.ReceiverId = grpMessage.ReceiverId;
                        grpMsg.UserId = CommonHelper.GetUserId();
                        db.Insert(grpMsg);
                    }
                    db.Commit();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
            }
            db.Close();
            return grpMsg;
        }

        private static GroupMessageStatus getGroupMessageStatusbyidAndReceiver(long GroupMessageId, long ReceiverId)
        {
            GroupMessageStatus chat = null;
            var db = new SQLiteConnection(CommonConstant.DBPath);
            try
            {
                chat =
                    db.Query<GroupMessageStatus>("select * from 'GroupMessageStatus' where GroupMessageId=" + GroupMessageId + " and  ReceiverId = " + ReceiverId + " and UserId=" + CommonHelper.GetUserId())
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

        public static GroupMessageStatus GetGroupMessageOverallStatusbyid(long GroupMessageId)
        {
            List<GroupMessageStatus> var = null;
            var db = new SQLiteConnection(CommonConstant.DBPath);
            try
            {
                var =
                    db.Query<GroupMessageStatus>("select * from 'GroupMessageStatus' where GroupMessageId=" + GroupMessageId + " and UserId=" + CommonHelper.GetUserId())
                        .ToList();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
            }
            db.Close();
            GroupMessageStatus returnStatus = new GroupMessageStatus();
            if (var != null)
            {
                if (var.Where(a => a.IsSend == true).ToList().Count == var.Count)
                {
                    returnStatus.IsSend = true;
                }
                if (var.Where(a => a.IsRead == true).ToList().Count == var.Count)
                {
                    returnStatus.IsRead = true;
                }
                if (var.Where(a => a.IsRecieved == true).ToList().Count == var.Count)
                {
                    returnStatus.IsRecieved = true;
                }
            }

            return returnStatus;
        }

        public static List<GroupMember> GroupMemberList(long groupId)
        {
            var db = new SQLiteConnection(CommonConstant.DBPath);
            List<GroupMember> lstGroupMember;
            try
            {

                lstGroupMember =
                      db.Query<GroupMember>("select * from 'GroupMember' where GroupId=" + groupId )
                          .ToList();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
                return lstGroupMember = new List<GroupMember>();
            }
            db.Close();
            return lstGroupMember;
        }

        public static void SaveorUpdateGroupMember(GroupMemberViewModel member,long groupid)
        {
            var db = new SQLiteConnection(CommonConstant.DBPath);
            try
            {
                var group = GetGroupMemeberByID(groupid,member.MemberId);
                if (group == null)
                {
                  var  groupmember = new GroupMember();
                    groupmember.GroupId = groupid;
                    groupmember.GroupMemberId = member.MemberId;
                    groupmember.MemberName = member.MemberName;

                    groupmember.UserId = CommonHelper.GetUserId();
                    db.Insert(groupmember);
                }
                else
                {
                    group.GroupId = groupid;
                    group.GroupMemberId = member.MemberId;
                    group.MemberName = member.MemberName;
                    group.UserId = CommonHelper.GetUserId();
                    db.Update(group);
                }
                db.Commit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
            }
            db.Close();
        }

        public static GroupMember GetGroupMemeberByID(long groupid,long MemberID)
        {
            var db = new SQLiteConnection(CommonConstant.DBPath);
           GroupMember GroupMember;
            try
            {

                GroupMember =
                      db.Query<GroupMember>("select * from 'GroupMember' where GroupId=" + groupid + " and      GroupMemberId ="+ MemberID + "  and UserId=" + CommonHelper.GetUserId())
                          .FirstOrDefault();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
                return GroupMember = new GroupMember();
            }
            db.Close();
            return GroupMember;
        }
    }
}