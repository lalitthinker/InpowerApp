using Foundation;
using System;
using UIKit;
using InPowerApp.Model;
using PCL.Model;
using System.Collections.Generic;
using InPowerIOS.Model;
using PCL.Service;
using Newtonsoft.Json;
using InPowerIOS.Common;
using Microsoft.AppCenter.Crashes;

namespace InPowerIOS.Chats
{
    public partial class AddContactsListViewController : UIViewController
    {
        bool loadList = true;
        public UISearchBar searchBar;
        public AddContactsListViewController (IntPtr handle) : base (handle)
        {
        }
        AddContactsListViewControllerSource addContactsListViewControllerSource;
        public List<UserProfile> ContactList;
        PaginationModel paginationModel = new PaginationModel();
        public override void ViewDidAppear(bool animated)
        {
            paginationModel.Status = 2;
            base.ViewDidAppear(animated);
        }


        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
          
            Title = "ADD CONTACTS";
            searchBar = CommonSearchView.Create();

            searchBar.SearchButtonClicked += SearchBar_SearchButtonClicked;
            searchBar.CancelButtonClicked += SearchBar_CancelButtonClicked;
            tblAddContacts.TableHeaderView = searchBar;
            paginationModel.Status = 2;
            if (InternetConnectivityModel.CheckConnection())
            {
                LoadServerContacts();
            }
            else
            {
                paginationModel.SkipRecords = 0;
                paginationModel.TakeRecords = 30;
            }
        }


        private async void LoadServerContacts(string SearchText = null)
        {
            try
            {
                paginationModel.SkipRecords = 0;
                paginationModel.TakeRecords = 30;
                paginationModel.SearchText = SearchText;
                var result = await new ContactsService().GetContactsAll(paginationModel);// 0 = WishList ,  1 = Read , 2 = All
                if (result.Status == 1)
                {
                    ContactList = JsonConvert.DeserializeObject<List<UserProfile>>(result.Response.ToString());
                    loadContactAdapter(ContactList);
                }
            }
            catch (Exception ex)
            {
                CustomToast.Show(ex.ToString(), false);
            }
        }

        private async void LoadMoreContacts()
        {
            try
            {
                loadList = false;
                paginationModel.SkipRecords += 30;
                var result = await new ContactsService().GetContactsAll(paginationModel);
                if (result.Status == 1)
                {
                    ContactList = JsonConvert.DeserializeObject<List<UserProfile>>(result.Response.ToString());
                    try
                    {
                        if (ContactList != null && ContactList.Count > 0)
                        {
                            addContactsListViewControllerSource.AddMoreContactList(ContactList);
                            tblAddContacts.ReloadData();
                            loadList = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        CustomToast.Show(ex.ToString(), false);
                    }
                }
                else
                {
                    CustomToast.Show(Message: "No More Contacs", Default: true);
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                CustomToast.Show( ex.ToString(), false);
            }
        }

        public void loadContactAdapter(List<UserProfile> ContactList)
        {
            try
            {
                if (ContactList != null && ContactList.Count > 0)
                {
                    tblAddContacts.TableFooterView = new UIView();

                    addContactsListViewControllerSource = new AddContactsListViewControllerSource(ContactList);

                    tblAddContacts.Source = addContactsListViewControllerSource;
                    addContactsListViewControllerSource.ReloadList += AddContactsListViewControllerSource_ReloadList;
                    addContactsListViewControllerSource.ItemRemoved += AddContactsListViewControllerSource_ItemRemoved;
                    tblAddContacts.RowHeight = 50;
                    tblAddContacts.ReloadData();
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                CustomToast.Show(ex.ToString(), false);
            }
        }

        void SearchBar_CancelButtonClicked(object sender, EventArgs e)
        {
            LoadServerContacts();
            View.EndEditing(false);
        }

        void SearchBar_SearchButtonClicked(object sender, EventArgs e)
        {
            LoadServerContacts(searchBar.Text);
            View.EndEditing(false);
        }

        void AddContactsListViewControllerSource_ReloadList(object sender, long e)
        {
            if (loadList == true)
            {
                if (String.IsNullOrEmpty(paginationModel.SearchText))
                {
                    CustomToast.Show(Message: "Loading More Contacts", Default: true);
                    LoadMoreContacts();
                }
            }
        }

        void AddContactsListViewControllerSource_ItemRemoved(object sender, long e)
        {
            addContactsListViewControllerSource.RemoveBook(e);
            tblAddContacts.ReloadData();
        }

    }
}