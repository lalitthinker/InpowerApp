using Foundation;
using System;
using UIKit;
using PCL.Model;
using PCL.Service;
using InPowerIOS.Common;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Threading.Tasks;
using InPowerIOS.SideBarMenu;
using Microsoft.AppCenter.Crashes;

namespace InPowerIOS.Registration
{
    public partial class SelectInterestsViewController : UIViewController
    {
        public SelectInterestsViewController(IntPtr handle) : base(handle)
        {
        }


        List<InterestResponseViewModel> Interestlist;
        private SelectInterestsViewControllerSource selectInterestsSource;
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
          
            this.NavigationItem.SetRightBarButtonItem(new UIBarButtonItem("Done", UIBarButtonItemStyle.Plain, (sender, args) =>
            {
                BBIDone_Activated();
            }), true);
            
            NavigationItem.SetHidesBackButton(true, false);

            Title = "Select Interest";
            txtSuggestInterest.BecomeFirstResponder();
        }



        partial  void BtnRequest_TouchUpInside(UIButton sender)
        {
            SuggestInterestRequestAsync();
        }

    

        private async Task SuggestInterestRequestAsync()
        {
            try
            {

                if (txtSuggestInterest.Text != "" && txtSuggestInterest.Text != null)
                {
                    var Model = new SuggestedInterestsRequestViewModel
                    {
                        InterestName = txtSuggestInterest.Text,
                        UserId = CommonHelper.GetUserId()
                    };

                    var result = await new CommonService().PostSuggestInterest(Model);
                    if (result.Status == 1)
                    {
                        new UIAlertView("Suggested Interest", "Thank You For Suggesting This Interests.", null, "OK", null).Show();

                        txtSuggestInterest.Text = "";
                    }
                    else
                    {
                        new UIAlertView("Suggested Interest", "Failed to save this Interests.", null, "OK", null).Show();

                    }

                }
                else
                {
                    txtSuggestInterest.BecomeFirstResponder();
                    new UIAlertView("Suggested Interest", "Enter Suggest Interest First", null, "OK", null).Show();

                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                new UIAlertView("Suggested Interest", ex.ToString(), null, "OK", null).Show();
            }
        }

        public void BBIDone_Activated()
        {
            UserInterestInfoInsertAsync();
            //this.DismissViewController(true, null);
            InvokeOnMainThread(delegate
            {
                var appDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;
                var mainStoryboard = appDelegate.MainStoryboard;
                var rootViewMainController = appDelegate.GetViewController(mainStoryboard, "RootViewController");
                appDelegate.SetRootViewController(rootViewMainController, true);
            });
        }


        private async Task UserInterestInfoInsertAsync()
        {
            List<InterestRequestViewModel> ListinterestRequestViewModels = new List<InterestRequestViewModel>();
            for (int i = 0; i < Interestlist.Count; i++)
            {
                if (Interestlist[i].isSelected())
                {
                    var _objInterestRequestViewmodel = new InterestRequestViewModel
                    {
                        InterestId = Interestlist[i].InterestId,
                        UserId = CommonHelper.GetUserId()
                    };
                    ListinterestRequestViewModels.Add(_objInterestRequestViewmodel);

                }
            }
            if (ListinterestRequestViewModels.Count > 0)
            {
                await new CommonService().PostInterest(ListinterestRequestViewModels);
            }
        }


        private async void loadInterestAdapter()
        {
            var result = await new CommonService().GetInterest();
            if (result.Status == 1)
            {
                Interestlist = JsonConvert.DeserializeObject<List<InterestResponseViewModel>>(result.Response.ToString());

                if (Interestlist != null && Interestlist.Count > 0)
                {

                    tblSuggestInterest.TableFooterView = new UIView();
                    selectInterestsSource = new SelectInterestsViewControllerSource(Interestlist);

                    tblSuggestInterest.Source = selectInterestsSource;
                    tblSuggestInterest.RowHeight = 40;
                    tblSuggestInterest.ReloadData();
                }
            }
        }

        public override void ViewDidAppear(bool animated)
        {
            loadInterestAdapter();
            base.ViewDidAppear(animated);
        }

    }
}