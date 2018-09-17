using Foundation;
using System;
using UIKit;
using System.Collections.Generic;
using Contacts;
using System.Linq;
using System.Collections;
using InPowerIOS.Models;

namespace InPowerIOS.Setting
{
    public partial class InviteFriendsViaSMSViewController : UIViewController
    {

        private List<PhoneContactIOSModel> _Originalitems = new List<PhoneContactIOSModel>();
        private InviteFriendsViaSMSViewControllerSource inviteFriendsViaSMSViewControllerSource;

        public InviteFriendsViaSMSViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = "Invite Friends Via SMS";

            _Originalitems = GetAllContacts();

            if (_Originalitems.Count > 0)
            {
                tblPhoneContacts.TableFooterView = new UIView();

                inviteFriendsViaSMSViewControllerSource = new InviteFriendsViaSMSViewControllerSource(_Originalitems);

                tblPhoneContacts.Source = inviteFriendsViaSMSViewControllerSource;
                tblPhoneContacts.RowHeight = 60;
                tblPhoneContacts.ReloadData();
            }
        }

        public List<PhoneContactIOSModel> GetAllContacts()
        {
            var keysTOFetch = new[] { CNContactKey.GivenName, CNContactKey.FamilyName, CNContactKey.EmailAddresses , CNContactKey.PhoneNumbers, CNContactKey.ThumbnailImageData};
            NSError error;
            CNContact[] contactList;
            var ContainerId = new CNContactStore().DefaultContainerIdentifier;
            using (var predicate = CNContact.GetPredicateForContactsInContainer(ContainerId))

            using (var store = new CNContactStore())
            {
                contactList = store.GetUnifiedContacts(predicate, keysTOFetch, out error);
            }
            var contacts = new List<PhoneContactIOSModel>();

            foreach (var item in contactList)
            {
                if (null != item && null != item.EmailAddresses)
                {
                    contacts.Add(new PhoneContactIOSModel
                    {
                        GivenName = item.GivenName,
                        FamilyName = item.FamilyName,
                        EmailId = item.EmailAddresses.Select(m => m.Value.ToString()).FirstOrDefault(),
                        PhoneNumbers = item.PhoneNumbers.Select(m => m.Value.StringValue).FirstOrDefault(),
                        ThumbnailImageData = item.ThumbnailImageData
                    });
                }
            }

            return contacts;
        }
    }
   
}