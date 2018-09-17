using Foundation;
using System;
using UIKit;
using InPowerIOS.Model;
using static InPowerIOS.Chats.ChatViewContarollerSource;
using System.Collections.Generic;

namespace InPowerIOS.Chats
{
    public partial class ChatTableViewCell : UITableViewCell
    {
        
        public ChatTableViewCell (IntPtr handle) : base (handle)
        {
        }

      

        public void UpdateCell(ListItem ChatConverstions, int position)
        {
            

            switch (ChatConverstions.getType())
            {
                case 1:
                    {
                     //   txt_Date_message.Hidden = true;
                        GeneralItem GeneralItem = (GeneralItem)ChatConverstions;
            var item = GeneralItem.getChatMessagearray();
            Boolean isMe = item.ContactId != Common.CommonHelper.GetUserId();
            var AttachList = (item.ChatMessageId != 0) ? Repositories.ChatAttachmentRepository.GetChatAttachList(item.ChatMessageId) : new List<ChatAttachment>();

            if (isMe)
            {
                //RightUserMsg.Text = item.MessageText;
                //RightMsgTime.Text = item.MessageTime.ToLocalTime().ToString("hh:mm tt");
                //if (AttachList.Count > 0)
                //{

                //    CommonHelper.SetImageOnUIImageView(vh.iv_AttachImage, AttachList.FirstOrDefault().url, context, 600, 600);

                //    vh.iv_AttachImage.SetOnClickListener(new iv_AttachImageClikLitener(item, context));
                //    vh.ll_LinearLayoutForImageAttachRight.Visibility = ViewStates.Visible;
                //}
                //else
                //{
                //    vh.ll_LinearLayoutForImageAttachRight.Visibility = ViewStates.Gone;
                //}
                //if (item.IsRead)
                //{
                //    vh.iv_StatusRight.SetImageResource(Resource.Drawable.message_got_read_receipt_from_target);
                //}

                //else if (item.IsRecieved)
                //{
                //    vh.iv_StatusRight.SetImageResource(Resource.Drawable.message_got_receipt_from_target);
                //}

                //else if (item.IsSend)
                //{
                //    vh.iv_StatusRight.SetImageResource(Resource.Drawable.message_got_receipt_from_server);
                //}
                //else
                //{
                //    vh.iv_StatusRight.SetImageResource(Resource.Drawable.pending);
                //}

                //vh.ll_LinearLayoutRight.Visibility = ViewStates.Visible;
                //vh.ll_LinearLayoutLeft.Visibility = ViewStates.Gone;

                //LeftView.Hidden = true;
                //RightView.Hidden = false;
                           // txt_Date_message.Hidden = true;
                         //   LeftUserMsg.Hidden = true;
                         //   LeftMsgTime.Hidden = true;
            }
            else
            {
                LeftUserMsg.Text = item.MessageText;
                LeftMsgTime.Text = item.MessageTime.ToLocalTime().ToString("hh:mm tt");
                if (AttachList.Count > 0)
                {


                    //CommonHelper.SetImageOnUIImageView(vh.iv_AttachImageLeft, AttachList.FirstOrDefault().url, context, 600, 600);
                    //vh.iv_AttachImageLeft.SetOnClickListener(new iv_AttachImageLeftClikLitener(item, context));



                   //ll_LinearLayoutForImageAttachLeft.Visibility = ViewStates.Visible;

                }
                else
                {
                    //vh.ll_LinearLayoutForImageAttachLeft.Visibility = ViewStates.Gone;
                }

                //vh.ll_LinearLayoutLeft.Visibility = ViewStates.Visible;
                //vh.ll_LinearLayoutRight.Visibility = ViewStates.Gone;

                            LeftUserMsg.Hidden = false;
                            LeftMsgTime.Hidden = false;
                            //LeftView.Hidden = false;
                            //RightView.Hidden = true;
                          
            }
                        break;
                    }
                case 0:
                    {
                      //  txt_Date_message.Hidden = false;
                        LeftUserMsg.Hidden = false;
                        LeftMsgTime.Hidden = false;
                        //LeftView.Hidden = true;
                        //RightView.Hidden = true;
                        DateItem DateItem = (DateItem)ChatConverstions;
                        var datetimedata = Convert.ToDateTime(DateItem.getDate()).ToLocalTime().Date;

                        //if (datetimedata.Date == DateTime.Now.Date)
                        //{
                        //    txt_Date_message.Text = "Today";
                        //}
                        //else if (datetimedata.Date == DateTime.Now.Date.AddDays(-1))
                        //{
                        //    txt_Date_message.Text = "Yesterday";
                        //}
                        //else
                        //{
                        //    txt_Date_message.Text = datetimedata.ToString("MMM dd, yyyy");
                        //}
                        break;
                    }
            }

        }

       
    }
}