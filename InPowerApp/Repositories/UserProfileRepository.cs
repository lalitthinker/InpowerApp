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
using SQLite;
using InPowerApp.Model;

namespace InPowerApp.Repositories
{
  public  class UserProfileRepository
    {
        public static void SaveUserProfile(UserProfile userProfile)
        {
            try
            {
                var db = new SQLiteConnection(CommonConstant.DBPath);

                db.Insert(userProfile);
                db.Commit();
                db.Close();
            }
            catch (SQLiteException e)
            {
                Console.WriteLine("Error while saving user data : " + e.Message);
            }
        }
        public static UserProfile GetUserProfile(int UserId, int page = 0)
        {
            UserProfile UserProfile11 = new UserProfile();
           // List<UserProfile> UserProfile = null;
            var db = new SQLiteConnection(CommonConstant.DBPath);
            try
            {
                UserProfile11 = db.Query<UserProfile>("select * from 'UserProfile' where UserId=" + UserId).FirstOrDefault();
              //  UserProfile = db.Query<UserProfile>("select * from 'UserProfile'");
                return UserProfile11;
            }
            catch (SQLiteException e)
            {
                UserProfile11 = new UserProfile();
                Console.WriteLine("Error while fetching user data : " + e.Message);
            }
            db.Close();
            return UserProfile11;
        }

        public static void UpdateUserProfile(UserProfile userprofileRepo)
        {
            var db = new SQLiteConnection(CommonConstant.DBPath);
            try
            {
                db.Update(userprofileRepo);
                db.Commit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
            }
            db.Close();
        }


        public static UserProfile CheckUserId(long UserId)
        {
            UserProfile UserProfile11 = new UserProfile();
        
            var db = new SQLiteConnection(CommonConstant.DBPath);
            try
            {
                UserProfile11 = db.Query<UserProfile>("select * from 'UserProfile' where UserId=" + UserId).FirstOrDefault();
              
                return UserProfile11;
            }
            catch (SQLiteException e)
            {
                UserProfile11 = new UserProfile();
                Console.WriteLine("Error while fetching user data : " + e.Message);
            }
            db.Close();
            return UserProfile11;
        }
    }
}