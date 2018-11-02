using Foundation;
using System;
using UIKit;
using InPowerIOS.Model;
using InPowerIOS.Common;
using SDWebImage;
using PCL.Service;

namespace InPowerIOS.Chats
{
    public partial class AddContactsTableViewCell : UITableViewCell
    {
        UserProfile addContacts = new UserProfile();
        public event EventHandler<long> ReloadList;
        public AddContactsTableViewCell (IntPtr handle) : base (handle)
        {
        }

        public void UpdateCell(UserProfile userProfile)
        {
            if (userProfile != null)
            {
                addContacts = userProfile;
                CommonHelper.SetCircularImage(contactPic);
                txtUserName.Text = addContacts.FirstName + " " + addContacts.LastName;
                if (!string.IsNullOrEmpty(addContacts.ProfileImageUrl))
                {
                    contactPic.SetImage(new NSUrl(addContacts.ProfileImageUrl), UIImage.FromBundle("default_profile.png"));
                }
                else
                {
                    contactPic.Image = new UIImage("default_profile.png");
                }
            }
        }

        partial void BtnAdd_TouchUpInside(UIButton sender)
        {
            var alert = new UIAlertView("Add Contacts", "Do you want to Add this Contact?", null, "No", "Yes");

            alert.Clicked += (object asender, UIButtonEventArgs e) =>
            {
                if (e.ButtonIndex == 1)
                {
                    Contact C = new Contact();
                    C.contactId = addContacts.UserId;
                    SaveContact(C);
                }
                else
                    CustomToast.Show("Add Contacts", false);
                    //new UIAlertView("Add Contacts", "Cancelled!", null, "OK", null).Show();
                alert.DismissWithClickedButtonIndex(0, true);
            };
            alert.Show();

        }


        public async void SaveContact(Contact _model)
        {
            var result = await new ContactsService().AddContactService(_model.contactId);
            if (result.Status == 1)
            {
                CustomToast.Show("Contact successfully added", true);
                this.ReloadList(this, (int)_model.contactId);
            }
            else
            {
                CustomToast.Show("Contact not added", false);
                this.ReloadList(this, 0);
            }
            //this.ListReload(this, 0);
        }
    }
}