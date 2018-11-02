using Foundation;
using System;
using UIKit;
using System.Collections.Generic;
using InPowerIOS.Model;
using System.Threading.Tasks;
using PCL.Service;
using Newtonsoft.Json;
using InPowerIOS.Repositories;
using InPowerApp.Model;
using InPowerIOS.Common;
using Microsoft.AppCenter.Crashes;

namespace InPowerIOS.Chats
{
    public partial class ContactListViewController : UIViewController
    {
        public UISearchBar searchBar;
        public ContactListViewController (IntPtr handle) : base (handle)
        {
        }

        List<Contact> Contacts;
        private ContactListViewControllerSource contactSource;
        private static void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs unobservedTaskExceptionEventArgs)
        {
            var newExc = new Exception("TaskSchedulerOnUnobservedTaskException", unobservedTaskExceptionEventArgs.Exception);
            LogUnhandledException(newExc);
        }


        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            var newExc = new Exception("CurrentDomainOnUnhandledException", unhandledExceptionEventArgs.ExceptionObject as Exception);
            LogUnhandledException(newExc);
        }


        public static void LogUnhandledException(Exception exception)
        {
            try
            {
                // Log to Android Device Logging.
                //Android.Util.Log.Error("Crash Report", exception.Message);
                //Crashes.TrackError(exception);
            }
            catch
            {
                // just suppress any error logging exceptions
            }
        }



        public async Task GetMyContactsFromServer()
        {
            try
            {
                await Task.Run(async () =>
                {
                    var result = await new ContactsService().GetAllMyContact();

                    if (result.Status == 1)
                    {
                        var chatlist = JsonConvert.DeserializeObject<List<UserProfile>>(result.Response.ToString());
                        ContactRepository.SaveMyContactsFromServer(chatlist, "mycontacts");
                        InvokeOnMainThread(() =>
                        {
                            loadadapter();
                        });
                    }

                });

            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }

        }


        public void loadadapter()
        {
            try
            {
                Contacts = new List<Contact>();
                Contacts = ContactRepository.GetContactsbyType("mycontacts");
                Contacts.Insert(0, null);
                if (Contacts.Count > 0)
                {
                    tblContactList.TableFooterView = new UIView();

                    contactSource = new ContactListViewControllerSource(Contacts, this);

                    tblContactList.Source = contactSource;
                    tblContactList.RowHeight = 60;
                    tblContactList.ReloadData();
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }


        private async Task Loadonresume()
        {
            try
            {
                InvokeOnMainThread(() =>
                {
                    loadadapter();
                });

                if (InternetConnectivityModel.CheckConnection())
                {
                    GetMyContactsFromServer();
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();


            searchBar = CommonSearchView.Create();
            searchBar.TextChanged += (sender, e) =>
            {
                searchChatUsers();
            };
            searchBar.CancelButtonClicked += SearchBar_CancelButtonClicked;
            tblContactList.TableHeaderView = searchBar;

            Title = "CONTACTS";

            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;

            Loadonresume();
        }


        public override void ViewDidAppear(bool animated)
        {
            Loadonresume();
            base.ViewDidAppear(animated);
        }

        void SearchBar_CancelButtonClicked(object sender, EventArgs e)
        {
            searchBar.Text = "";
            searchChatUsers();
            View.EndEditing(false);
        }


        private void searchChatUsers()  
        {  
            //perform the search, and refresh the table with the results  
            contactSource.PerformSearch(searchBar.Text);  
            tblContactList.ReloadData(); 
        
        } 
    }
}