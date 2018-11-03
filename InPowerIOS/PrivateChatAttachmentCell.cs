using Foundation;
using InPowerIOS.Common;
using InPowerIOS.Model;
using System;
using System.Drawing;
using UIKit;
using System.Linq;
using System.Collections.Generic;
using SDWebImage;
using System.IO;
using BigTed;

namespace InPowerIOS
{
    public partial class PrivateChatAttachmentCell : UITableViewCell
    {
        public static NSString KeyLeft = new NSString("Incoming");
        public static NSString KeyRight = new NSString("Outgoing");
        public static UIImage bleft, bright, left, right;
        public static UIImage singleGray, doubleGray, doubleBlue, wait, attachmentImage;
        public static UIFont font = UIFont.SystemFontOfSize(14);
        UIView view;
        bool isLeft;
        string ImagePath;
        UIViewController uiNewView;

        public event EventHandler<string> OpenImageViewEvent;

     

        public PrivateChatAttachmentCell(IntPtr handle) : base(handle)
        {
        }


        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            var frame = ContentView.Frame;

            var sizeMessage = GetSizeForText(this, lblMessage.Text) + BubblePadding;
            var sizeTime = GetSizeForText(this, lblTime.Text) + BubblePadding;


            SizeF StatusImageSize = new SizeF(sizeTime.Height + 5, sizeTime.Height);


            float ImageSizeWidth = (sizeTime.Width + StatusImageSize.Width) * 2;
            SizeF AttachmentImageSize = new SizeF(ImageSizeWidth, ImageSizeWidth - 30);
            SizeF TimeLabelSize = new SizeF(ImageSizeWidth - sizeTime.Height + 10, sizeTime.Height);

            SizeF ImageSize = new SizeF(ImageSizeWidth, sizeMessage.Height + sizeTime.Height + AttachmentImageSize.Height - BubblePadding.Height);
            BackgroundImageView.Frame = new RectangleF(new PointF((float)(isLeft ? 10 : frame.Width - ImageSizeWidth - 10), (float)frame.Y), ImageSize);
            view.SetNeedsDisplay();
            frame = BackgroundImageView.Frame;

            lblMessage.Frame = new RectangleF(new PointF((float)(frame.X + (isLeft ? 12 : 8)), (float)(frame.Y + AttachmentImageSize.Height + 5)), sizeMessage - BubblePadding);
            lblTime.Frame = new RectangleF(new PointF((float)(frame.X + (isLeft ? 10 : 8)), (float)(frame.Y + sizeMessage.Height + AttachmentImageSize.Height + -10)), TimeLabelSize - BubblePadding);

            if (!this.isLeft)
            {
                StatusTick.Frame = new RectangleF(new PointF((float)(frame.X + (isLeft ? frame.Width - BubblePadding.Width + 5 - frame.X : frame.Width - BubblePadding.Width - 10)), (float)(frame.Y + sizeMessage.Height + AttachmentImageSize.Height - 10)), StatusImageSize - BubblePadding);
            }
           // AttachmentImageView.Frame = new RectangleF(new PointF((float)(frame.X + (isLeft ? 12 : 8)), (float)(frame.Y + 5)), AttachmentImageSize - BubblePadding);
            AttahmentImageButton.Frame = new RectangleF(new PointF((float)(frame.X + (isLeft ? 12 : 8)), (float)(frame.Y + 5)), AttachmentImageSize - BubblePadding);

        }


        static internal SizeF BubblePadding = new SizeF(22, 16);
        //static internal SizeF label1Padding = new SizeF(10, 0);
        static internal SizeF TimeLablePadding = new SizeF(22, 60);

        static internal SizeF GetSizeForText(UIView tv, string text)
        {
            SizeF textSize;
            if (text == null)
                text = "";
            using (NSString nssSomeString = new NSString(text))
            {
                textSize = (System.Drawing.SizeF)nssSomeString.StringSize(font, new SizeF((float)tv.Bounds.Width * .7f - 10 - 22, 99999), UILineBreakMode.WordWrap);
                //Console.WriteLine(textSize);
            }
            return textSize;
            // return tv.StringSize(text, font, new SizeF((float)tv.Bounds.Width * .7f - 10 - 22, 99999));
        }

        public void Update(ChatMessage item, UIViewController uiNewView, bool IsLeft)
        {
            this.uiNewView = uiNewView;
            bright = UIImage.FromFile("green.png");
            bleft = UIImage.FromFile("grey.png");
            singleGray = UIImage.FromFile("singleGray.png");
            doubleGray = UIImage.FromFile("grey.png");
            doubleBlue = UIImage.FromFile("doubleBlue.png");
            attachmentImage = UIImage.FromFile("bg.png");
            wait = UIImage.FromFile("grey.png");

            // buggy, see https://bugzilla.xamarin.com/show_bug.cgi?id=6177
            //left = bleft.CreateResizableImage (new UIEdgeInsets (10, 16, 18, 26));
            //right = bright.CreateResizableImage (new UIEdgeInsets (11, 11, 17, 18));
            left = bleft.StretchableImage(26, 16);
            right = bright.StretchableImage(11, 11);

            var rect = new RectangleF(0, 0, 1, 1);
            this.isLeft = IsLeft;
            view = new UIView(rect);
            BackgroundImageView.Image = isLeft ? left : right;
            //view.AddSubview(BackgroundImageView);
            //view.AddSubview(lblMessage);
            //view.AddSubview(lblTime);
            //view.AddSubview(StatusTick);
            //view.AddSubview(AttachmentImageView);
            //ContentView.Add(view);

            lblMessage.Text = item.MessageText;
            lblTime.Text = item.MessageTime.ToString("hh:mm tt"); ;

            var AttachList = (item.ChatMessageId != 0) ? Repositories.ChatAttachmentRepository.GetChatAttachList(item.ChatMessageId) : new List<ChatAttachment>();

          
            if (AttachList.Count > 0)
            {
                string attachmentUrl = CommonHelper.GetImageThumbUrl(AttachList.FirstOrDefault().url);
                //AttachmentImageView.SetImage(new NSUrl(attachmentUrl), UIImage.FromBundle("PlaceHolder.png"));
                //var selectImageTapped = new UITapGestureRecognizer(() => { ShowUpdateUserProfileViewController(); });
                //AttachmentImageView.AddGestureRecognizer(selectImageTapped);
                try
                {
                    AttahmentImageButton.SetBackgroundImage(ImageClass.FromUrl(attachmentUrl), UIControlState.Normal);
                }
                catch
                {
                    AttahmentImageButton.SetBackgroundImage(UIImage.FromBundle("PlaceHolder.png"), UIControlState.Normal);
                }
               
              //  AttachmentImageView.SetImage(ImageClass.FromUrl(AttachList.FirstOrDefault().url), UIControlState.Normal);
                ImagePath = AttachList.FirstOrDefault().url;
            }
            else
            {

            }
            if (!this.isLeft)
            {
                if (item.IsRead)
                {
                    StatusTick.Image = doubleBlue;

                    // vh.iv_StatusRight.SetImageResource(Resource.Drawable.message_got_read_receipt_from_target);
                }

                else if (item.IsRecieved)
                {
                    StatusTick.Image = doubleGray;
                }

                else if (item.IsSend)
                {
                    StatusTick.Image = singleGray;
                }
                else
                {
                    StatusTick.Image = singleGray;
                }

            }
            SetNeedsLayout();
        }

        private void ShowUpdateUserProfileViewController()
        {
            this.OpenImageViewEvent(this, ImagePath);
        }

        partial void AttahmentImageButton_TouchUpInside(UIButton sender)
        {
            var imageViewcontroller = (ImageViewController)uiNewView.Storyboard.InstantiateViewController("ImageViewController");
            imageViewcontroller.filepath = ImagePath;
            BTProgressHUD.Show("Loading Image");
            uiNewView.NavigationController.PushViewController(imageViewcontroller, true);
        }

        public void UpdateGroup(GroupMessage item, UIViewController uiNewVieww, bool IsLeft)
        {
            this.uiNewView = uiNewVieww;
            bright = UIImage.FromFile("green.png");
            bleft = UIImage.FromFile("grey.png");
            singleGray = UIImage.FromFile("singleGray.png");
            doubleGray = UIImage.FromFile("grey.png");
            doubleBlue = UIImage.FromFile("doubleBlue.png");
            attachmentImage = UIImage.FromFile("bg.png");
            wait = UIImage.FromFile("grey.png");

            // buggy, see https://bugzilla.xamarin.com/show_bug.cgi?id=6177
            //left = bleft.CreateResizableImage (new UIEdgeInsets (10, 16, 18, 26));
            //right = bright.CreateResizableImage (new UIEdgeInsets (11, 11, 17, 18));
            left = bleft.StretchableImage(26, 16);
            right = bright.StretchableImage(11, 11);

            var rect = new RectangleF(0, 0, 1, 1);
            this.isLeft = IsLeft;
            view = new UIView(rect);
            BackgroundImageView.Image = isLeft ? left : right;

            lblMessage.Text = item.MessageText;
            lblTime.Text = item.MessageTime.ToString("hh:mm tt");

            var AttachList = (item.MessageId != 0) ? Repositories.GroupRepository.GetGroupMessageAttachList(item.MessageId) : new List<GroupAttachment>();
            if (AttachList.Count > 0)
            {
                string attachmentUrl = CommonHelper.GetImageThumbUrl(AttachList.FirstOrDefault().url);
                //AttachmentImageView.SetImage(new NSUrl(attachmentUrl), UIImage.FromBundle("PlaceHolder.png"));
                //var selectImageTapped = new UITapGestureRecognizer(() => { ShowUpdateUserProfileViewController(); });
                //AttachmentImageView.AddGestureRecognizer(selectImageTapped);
                try
                {
                    AttahmentImageButton.SetBackgroundImage(ImageClass.FromUrl(attachmentUrl), UIControlState.Normal);
                }
                catch
                {
                    AttahmentImageButton.SetBackgroundImage(UIImage.FromBundle("PlaceHolder.png"), UIControlState.Normal);
                }

                //  AttachmentImageView.SetImage(ImageClass.FromUrl(AttachList.FirstOrDefault().url), UIControlState.Normal);
                ImagePath = AttachList.FirstOrDefault().url;
            }
            else
            {

            }
            var Status = (item.MessageId != 0) ? Repositories.GroupRepository.GetGroupMessageOverallStatusbyid(item.MessageId) : new GroupMessageStatus();
            if (!this.isLeft)
            {
                if (Status.IsRead)
                {
                    StatusTick.Image = doubleBlue;
                }
                else if (Status.IsRecieved)
                {
                    StatusTick.Image = doubleGray;
                }

                else if (Status.IsSend)
                {
                    StatusTick.Image = singleGray;
                }
                else
                {
                    StatusTick.Image = singleGray;
                }
            }
            SetNeedsLayout();
        }

        public static SizeF GetHeight(UIView tv, string text1, string text2)
        {
            var size1 = GetSizeForText(tv, text1) + BubblePadding;
            var size2 = GetSizeForText(tv, text2) + BubblePadding;

            SizeF SmallPadding = new SizeF(0, 5);

            SizeF StatusImageSize = new SizeF(size2.Height + 5, size2.Height);

            float ImageSizeWidth = (size2.Width + StatusImageSize.Width) * 2;
            SizeF AttachmentImageSize = new SizeF(ImageSizeWidth, ImageSizeWidth - 30);
            SizeF TimeLabelSize = new SizeF(ImageSizeWidth - size2.Height + 10, size2.Height);

            return size1 + size2 + SmallPadding + AttachmentImageSize;

        }

       
    }
}