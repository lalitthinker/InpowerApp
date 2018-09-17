using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLite;
using InPowerIOS.Model;
using InPowerIOS.Common;

namespace InPowerIOS.Repositories
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
            UserProfile UserProfile = null;
            var db = new SQLiteConnection(CommonConstant.DBPath);
            try
            {
                UserProfile = db.Query<UserProfile>("select * from 'UserProfile' where UserId=" + UserId).FirstOrDefault();
                return UserProfile;
            }
            catch (SQLiteException e)
            {
                UserProfile = new UserProfile();
                Console.WriteLine("Error while fetching user data : " + e.Message);
            }
            db.Close();
            return UserProfile;
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
    }
}