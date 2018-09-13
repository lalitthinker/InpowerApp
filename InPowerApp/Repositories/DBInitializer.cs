using System;
using System.IO;
using InPowerApp.Common;
using InPowerApp.Model;
using SQLite;

namespace InPowerApp.Repositories
{
    public  class DBInitializer
    {
        public static void CreateDatabase()
        {
            try
            {
                var dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal),
                    CommonConstant.DBName);
                bool isDBExisting = File.Exists(dbPath);
                var db = new SQLiteConnection(dbPath);
                if (!isDBExisting)
                {
                    db.CreateTable<UserProfile>();
                    db.CreateTable<Contact>();
                    db.CreateTable<ChatConversation>();
                    db.CreateTable<GroupModel>();
                    db.CreateTable<ChatMessage>();
                    db.CreateTable<ChatAttachment>();
                    db.CreateTable<GroupAttachment>();
                    db.CreateTable<GroupMember>();
                    db.CreateTable<GroupMessage>();
                    db.CreateTable<Books>();
                    db.CreateTable<GroupMessageStatus>();
                }
                else
                {

                    return;
                }
            }
            catch(Exception ex)
            {

            }
        }
    }
}