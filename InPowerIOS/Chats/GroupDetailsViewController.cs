using Foundation;
using System;
using UIKit;
using InPowerIOS.Model;
using CoreGraphics;
using InPowerIOS.Common;
using SDWebImage;
using System.Collections.Generic;
using InPowerIOS.Repositories;
using Microsoft.AppCenter.Crashes;

namespace InPowerIOS.Chats
{
    public partial class GroupDetailsViewController : UIViewController
    {
        public List<GroupMember> GroupMemberList;
        GroupDetailsViewControllerSource groupDetailsViewControllerSource;
        public GroupModel contactViewModel { get; set; }
        public GroupDetailsViewController (IntPtr handle) : base (handle)
        {
            this.contactViewModel = contactViewModel;
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            lblLeaveGroup.UserInteractionEnabled = true;
            var selectlblLeaveGroupTapped = new UITapGestureRecognizer(() => { LeaveGroup(); });

            lblLeaveGroup.AddGestureRecognizer(selectlblLeaveGroupTapped);

            getGroupdetails();

            var g = new UITapGestureRecognizer(() => View.EndEditing(false));
            g.CancelsTouchesInView = false;
            View.AddGestureRecognizer(g);

        }

        private void LeaveGroup()
        {
           
        }

        public void getGroupdetails()
        {
            CommonHelper.SetCircularImage(GroupImageView);
            //Title = contactViewModel.GroupName == null ? "" : contactViewModel.GroupName;
        
            GroupIntrest.Text = ((contactViewModel.InterestId == null) ? "" : contactViewModel.InterestId.ToString()); 
            GroupDescription.Text = ((contactViewModel.GroupDescription == null) ? "" : contactViewModel.GroupDescription); 
            //GroupType.Text = ((contactViewModel.type == null) ? "" : contactViewModel.type); 

            if (!string.IsNullOrEmpty(contactViewModel.GroupPictureUrl))
            {
                GroupImageView.SetImage(new NSUrl(contactViewModel.GroupPictureUrl), UIImage.FromBundle("grouplist.png"));
            }
            else
            {
                GroupImageView.Image = new UIImage("grouplist.png");
            }



            GetGroupMembersData();
        }

        private void GetGroupMembersData()
        {
            try
            {
                GroupMemberList = GroupRepository.GroupMemberList(contactViewModel.GroupId);

                if (GroupMemberList != null && GroupMemberList.Count > 0)
                {
                    tblMemberList.TableFooterView = new UIView();

                    groupDetailsViewControllerSource = new GroupDetailsViewControllerSource(GroupMemberList, this);

                    tblMemberList.Source = groupDetailsViewControllerSource;
                    tblMemberList.RowHeight = 45;
                    tblMemberList.ReloadData();
                }
            }
            catch (Exception ex)
            {
                //Toast.MakeText(this, ex.ToString(), ToastLength.Long).Show();
                Crashes.TrackError(ex);
            }
        }
    }
}