using System;
using Foundation;

namespace InPowerIOS.Models
{
    public class PhoneContactIOSModel
    {
        public PhoneContactIOSModel()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="givenName"></param>
        /// <param name="familyName"></param>
        /// <param name="emailId"></param>
        /// <param name="phoneNumbers"></param>
        /// <param name="thumbnailImageData"></param>
        /// 
        public PhoneContactIOSModel(string givenName, string familyName, string emailId, string phoneNumbers, NSData thumbnailImageData)
        {
            GivenName = givenName;
            FamilyName = familyName;
            EmailId = emailId;
            PhoneNumbers = phoneNumbers;
            ThumbnailImageData = ThumbnailImageData;
        }

        public bool IsSelected { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }

        public string FullName { get => $"{GivenName} {FamilyName}"; }
        public string EmailId { get; set; }
        public string PhoneNumbers { get; set; }
        public NSData ThumbnailImageData { get; set; }
    }


}
