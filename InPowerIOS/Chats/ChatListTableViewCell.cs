using Foundation;
using System;
using UIKit;
using System.Collections.Generic;
using InPowerIOS.Repositories;
using InPowerIOS.Common;
using SDWebImage;
using InPowerIOS.Model;

namespace InPowerIOS.Chats
{
   
    public partial class ChatListTableViewCell : UITableViewCell
    {
        public ChatListTableViewCell (IntPtr handle) : base (handle)
        {
        }

        public void UpdateCell(ChatConversation chatConverstions)
        {
            var chat = ChatConversationRepository.GetConversationIdbyChatId(Convert.ToInt32(chatConverstions.ChatId));
            CommonHelper.SetCircularImage(ProfileImage);

            if (chat != null)
            {
                if (chat.IsGroup == false)
                {
                    var contact = ContactRepository.GetContactbyUserId((long)chat.ContactId);
                    if (contact != null)
                    {
                        if (!string.IsNullOrEmpty(contact.contactPicUrl))
                        {
                            ProfileImage.SetImage(new NSUrl(contact.contactPicUrl), UIImage.FromBundle("default_profile.png"));
                        }
                        else
                        {
                            ProfileImage.Image = new UIImage("default_profile.png");
                        }

                        lblChatUserName.Text = contact.screenName;

                        if (chatConverstions.LastMessageDate.HasValue)
                        {

                            if (Convert.ToDateTime(chatConverstions.LastMessageDate).Date == DateTime.UtcNow.ToLocalTime().Date)
                            {
                                lblChatLastTime.Text = Convert.ToDateTime(chatConverstions.LastMessageDate).ToString("hh:mm tt");
                            }
                            else
                            {
                                lblChatLastTime.Text = Convert.ToDateTime(chatConverstions.LastMessageDate).Date.ToString("MM/dd/yyyy");
                            }
                        }
                        if (!string.IsNullOrEmpty(chatConverstions.LastMessage))
                        {
                            lblChatLastMessage.Text = chatConverstions.LastMessage;
                        }
                        else
                        {
                            lblChatLastMessage.Text = "";
                        }

                        int count = ChatMessageRepository.getChatMessageUnRead(chatConverstions.ChatId);

                        if (count > 0)
                        {
                            lblMessageCount.Hidden = false;
                            lblMessageCount.SetTitle(count.ToString(),UIControlState.Normal);
                        }
                        else
                        {
                            lblMessageCount.Hidden = true;
                        }
                    }
                }
                else
                {
                    var group = GroupRepository.GetGroupByID((long)chat.GroupId);
                    if (group != null)
                    {
                        if (!string.IsNullOrEmpty(group.GroupPictureUrl))
                        {
                            ProfileImage.SetImage(new NSUrl(group.GroupPictureUrl), UIImage.FromBundle("grouplist.png"));
                        }
                        else
                        {
                            ProfileImage.Image = new UIImage("grouplist.png");
                        }

                        lblChatUserName.Text = group.GroupName;

                        if (chatConverstions.LastMessageDate.HasValue)
                        {
                            if (Convert.ToDateTime(chatConverstions.LastMessageDate).Date == DateTime.UtcNow.ToLocalTime().Date)
                            {
                                lblChatLastTime.Text = Convert.ToDateTime(chatConverstions.LastMessageDate).ToString("hh:mm tt");
                            }
                            else
                            {
                                lblChatLastTime.Text = Convert.ToDateTime(chatConverstions.LastMessageDate).Date.ToString("MM/dd/yyyy");
                            }
                        }
                        if (!string.IsNullOrEmpty(chatConverstions.LastMessage))
                        {
                            lblChatLastMessage.Text = chatConverstions.LastMessage;
                        }
                        else
                        {
                            lblChatLastMessage.Text = "";
                        }

                        if (!string.IsNullOrEmpty(chatConverstions.LastMessage))
                        {
                            lblChatLastMessage.Text = chatConverstions.SenderName + " : " + chatConverstions.LastMessage;
                        }
                        else
                        {
                            var GroupUser = ContactRepository.GetContactbyUserId(chatConverstions.OwnerId);
                            if (GroupUser != null)
                            {
                                lblChatLastMessage.Text = "Created by " + GroupUser.screenName;
                            }
                            else
                            {
                                lblChatLastMessage.Text = "Created by me";
                            }
                        }

                        int countGroupUnread = GroupRepository.getGroupMessageUnRead((long)chatConverstions.GroupId);
                    
                        if (countGroupUnread > 0)
                        {
                            lblMessageCount.Hidden = false;
                            lblMessageCount.SetTitle(countGroupUnread.ToString(), UIControlState.Normal);
                        }
                        else
                        {
                            lblMessageCount.Hidden = true;
                        }
                    }
                }
            }

          
        }
    }
}