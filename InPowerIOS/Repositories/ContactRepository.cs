using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InPowerIOS.Common;
using InPowerIOS.Model;
using InPowerIOS.Models;
using PCL.Common;
using SQLite;

namespace InPowerIOS.Repositories
{
    public  class ContactRepository
    {
        private readonly static object LockSaveContact = new object();

        public static void SaveorUpdateContact(Contact contact,string source=null)
        {
            
                var db = new SQLiteConnection(CommonConstant.DBPath);
                try
                {
                    var dbContact = CheckExistingContact(contact.contactId);
                    if (dbContact.success == true)
                    {
                    if (dbContact.contact == null)
                        {
                            db.Insert(contact);
                        }
                        else
                        {
                        dbContact.contact.contactId = contact.contactId;
                            dbContact.contact.source = source;
                            dbContact.contact.contactPicUrl = contact.contactPicUrl;
                            dbContact.contact.name = contact.name;
                            dbContact.contact.screenName = dbContact.contact.name;
                            dbContact.contact.city = contact.city;
                            dbContact.contact.state = contact.state;
                            dbContact.contact.Aboutme = contact.Aboutme;
                            dbContact.contact.number = contact.number;
                            dbContact.contact.email = contact.email;
                            dbContact.contact.UserId = CommonHelper.GetUserId();
                        db.Update(dbContact.contact);
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
        public static Contact GetContactbyUserId(long userId)
        {
            Contact contact=null;
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

        public static ContactSaveResponceViewModel CheckExistingContact(long userId)
        {
            ContactSaveResponceViewModel contactResponce = new  ContactSaveResponceViewModel();
            contactResponce.success = true;
            var db = new SQLiteConnection(CommonConstant.DBPath);
            try
            {
                contactResponce.contact = db.Query<Contact>("select * from 'Contact' where contactId=" + userId + " and UserId=" + CommonHelper.GetUserId()).FirstOrDefault();

            }
            catch (Exception ex)
            {
                contactResponce.success = false;
                Console.WriteLine(ex.Message, ex);
            }
            db.Close();
            return contactResponce;
        }

        public static void SaveMyContactsFromServer(List<UserProfile> contact,string type)
        {

            if (GlobalConstant.ContactInsertStart)
            {
                // var db = new SQLiteConnection(CommonConstant.DBPath);
                try
                {
                    GlobalConstant.ContactInsertStart = false;
                    foreach (var c in contact)
                    {
                        Contact cnt = new Contact();
                        cnt.contactId = c.UserId;
                        cnt.name = c.FirstName + " " + c.LastName;
                        cnt.screenName = c.FirstName + " " + c.LastName; ;
                        cnt.contactPicUrl = c.ProfileImageUrl;
                        cnt.source = type;
                        cnt.UserId = CommonHelper.GetUserId();
                        SaveorUpdateContact(cnt, type);
                        //var cnt = GetContactbyUserId(c.UserId);
                        //if (cnt == null)
                        //{
                        //    cnt = new Contact();
                        //    cnt.contactId = c.UserId;
                        //    cnt.name = c.FirstName + " " + c.LastName;
                        //    cnt.screenName = c.FirstName + " " + c.LastName; ;
                        //    cnt.contactPicUrl = c.ProfileImageUrl;
                        //    cnt.source = type;
                        //    cnt.UserId = CommonHelper.GetUserId();
                        //    db.Insert(cnt);
                        //}
                        //else
                        //{
                        //    //cnt.contactId = c.BitmobUserId.ToString();
                        //    cnt.name = c.FirstName + " " + c.LastName;
                        //    cnt.screenName = c.FirstName + " " + c.LastName;
                        //    cnt.contactPicUrl = c.ProfileImageUrl;
                        //    cnt.source = type;
                        //    cnt.UserId = CommonHelper.GetUserId();
                        //    db.Update(cnt);
                        //}
                        //db.Commit();
                    }
                    GlobalConstant.ContactInsertStart = true;
                }
                catch (Exception ex)
                {
                    GlobalConstant.ContactInsertStart = true;
                    Console.WriteLine(ex.Message, ex);
                }
            }
               // db.Close();

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