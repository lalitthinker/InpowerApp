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
using SQLite;

namespace InPowerApp.Repositories
{
   public    class ContactRepository
    {
        public static void SaveorUpdateContact(Contact contact)
        {
            var db = new SQLiteConnection(CommonConstant.DBPath);
            try
            {
                var dbContact = GetContactbyUserId(contact.contactId);
                if (dbContact == null)
                {
                   
                    db.Insert(contact);
                }
                else
                {
                    dbContact.contactPicUrl = contact.contactPicUrl;
                    dbContact.name = contact.name;
                    dbContact.city = contact.city;
                    dbContact.state = contact.state;
                    dbContact.Aboutme = contact.Aboutme;
                    dbContact.number = contact.number;
                    dbContact.email = contact.email ;

                    dbContact.UserId = CommonHelper.GetUserId();
                    db.Update(dbContact);



                }

                db.Commit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
            }
            db.Close();
        }
        public static Contact GetContactbyUserId(long userId)
        {
            Contact contact = null;
            var db = new SQLiteConnection(CommonConstant.DBPath);
            try
            {
                contact = db.Query<Contact>("select * from 'Contact' where contactId=" + userId+ " and UserId="+ CommonHelper.GetUserId()).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
            }
            db.Close();
            return contact;
        }

        public static void SaveMyContactsFromServer(List<UserProfile> contact,string type)
        {
            var db = new SQLiteConnection(CommonConstant.DBPath);
            try
            {
                foreach (var c in contact)
                {
                    var cnt = GetContactbyUserId(c.UserId);
                    if (cnt == null)
                    {
                        cnt = new Contact();
                        cnt.contactId = c.UserId;
                        cnt.name = c.FirstName + " " + c.LastName;
                        cnt.screenName = c.FirstName + " " + c.LastName; ;
                        cnt.contactPicUrl = c.ProfileImageUrl;
                        cnt.source = type;
                        cnt.UserId = CommonHelper.GetUserId();
                        db.Insert(cnt);
                    }
                    else
                    {
                        //cnt.contactId = c.BitmobUserId.ToString();
                        cnt.name = c.FirstName + " " + c.LastName;
                        cnt.screenName = c.FirstName + " " + c.LastName;
                        cnt.contactPicUrl = c.ProfileImageUrl;
                        cnt.source = type;
                        cnt.UserId = CommonHelper.GetUserId();
                        db.Update(cnt);
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

        public static List<Contact> GetAllContact()
        {
            List<Contact> contactList = null;
            var db = new SQLiteConnection(CommonConstant.DBPath);
            try
            {
                contactList = db.Query<Contact>("select * from 'Contact' where UserId=" + CommonHelper.GetUserId()).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
            }
            db.Close();
            return contactList;
        }

        public static List<Contact> GetContactsbyType(string type)
        {
            List<Contact> lstmessages = null;
            var db = new SQLiteConnection(CommonConstant.DBPath);
            try
            {
                lstmessages =
                    db.Query<Contact>("select * from Contact where source='" + type + "' and UserId="+ CommonHelper.GetUserId() + "").OrderBy(c => c.name).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
                lstmessages = new List<Contact>();
            }
            db.Close();
            return lstmessages;
        }
    }
}