using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using InPowerApp.Common;
using InPowerApp.ListAdapter;
using Newtonsoft.Json;
using PCL.Model;
using PCL.Service;

namespace InPowerApp.Activities
{
    [Activity(Label = "Welcome to InPower")]
    public class WhatsAreYourInterestsActivity : AppCompatActivity
    {
        EditText txtSuggestInterest;
        Button btnSuggestInterestsRequests;
        ProgressDialog progressDialog;
        ListView lvInterestList;
        LinearLayout hidekeybordlayout;
        List<InterestResponseViewModel> Interestlist;
        InterestListAdapter interestlistadapter;
        protected override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.WhatsAreYourInterestslayout);

            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.app_bar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetTitle(Resource.String.WelcometoInPower);
            SupportActionBar.SetSubtitle(Resource.String.SelectInterests);
            lvInterestList = (ListView)FindViewById(Resource.Id.lvInterestList);
            lvInterestList.ItemClick += LvInterestList_ItemClick; ;
            btnSuggestInterestsRequests = FindViewById<Button>(Resource.Id.btnSuggestInterestsRequest);
            txtSuggestInterest = FindViewById<EditText>(Resource.Id.txtSuggestInterests);
            hidekeybordlayout = FindViewById<LinearLayout>(Resource.Id.hidekeybordlayout);
            btnSuggestInterestsRequests.Click += BtnSuggestInterestsRequest_Click;
            hidekeybordlayout.Touch += Hidekeybordlayout_Touch;
            loadInterestAdapter();
        }

        private async void loadInterestAdapter()
        {
            var result = await new CommonService().GetInterest();
            if (result.Status == 1)
            {
                Interestlist = JsonConvert.DeserializeObject<List<InterestResponseViewModel>>(result.Response.ToString());
                interestlistadapter = new InterestListAdapter(this.BaseContext, Interestlist);
                lvInterestList.Adapter = interestlistadapter;
            }
        }

        private void LvInterestList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {

        }

        private async void BtnSuggestInterestsRequest_Click(object sender, EventArgs e)
        {
            try
            {
                Drawable icon_error = Resources.GetDrawable(Resource.Drawable.alert);
                icon_error.SetBounds(0, 0, 50, 50);

                if (txtSuggestInterest.Text != "" && txtSuggestInterest.Text != null)
                {
                    var Model = new SuggestedInterestsRequestViewModel
                    {
                        InterestName = txtSuggestInterest.Text,
                        UserId = Common.CommonHelper.GetUserId()
                    };
                    var result = await new CommonService().PostSuggestInterest(Model);
                    if (result.Status == 1)
                    {
                        string msg = "Suggested Interest\nThank You For Suggesting This Interests.";
                        Toast.MakeText(this, msg, ToastLength.Long).Show();
                        txtSuggestInterest.Text = "";
                    }
                    else
                    {
                        string msg = "Suggested Interest\nFailed to save  this Interests.";
                        Toast.MakeText(this, msg, ToastLength.Long).Show();
                    }

                }
                else
                {
                    txtSuggestInterest.RequestFocus();
                    txtSuggestInterest.SetError("Enter Suggest Interest First", icon_error);
                }
            }
            catch (Exception ex)
            {
                progressDialog.Hide();
                string ErrorMsg = ex.ToString();
                Toast.MakeText(this, ErrorMsg, ToastLength.Long).Show();
            }
        }


        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.action_menuOK, menu);
            if (menu != null)
            {
                menu.FindItem(Resource.Id.action_menuOKOK).SetVisible(true);
            }
            return base.OnCreateOptionsMenu(menu);
        }


        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.action_menuOKOK)
            {
                PostCheckedListData();
                this.Finish();


                var intent = new Intent(this, typeof(AddBooksToWishListActivity));
                intent.AddFlags(ActivityFlags.SingleTop);

                StartActivity(intent);
            }

            return base.OnOptionsItemSelected(item);
        }

        private async void PostCheckedListData()
        {
            List<InterestRequestViewModel> ListinterestRequestViewModels = new List<InterestRequestViewModel>();
            for (int i = 0; i < Interestlist.Count; i++)
            {
                if (Interestlist[i].isSelected())
                {
                    var _objInterestRequestViewmodel = new InterestRequestViewModel
                    {
                        InterestId = Interestlist[i].InterestId,
                        UserId = Common.CommonHelper.GetUserId()
                    };
                    ListinterestRequestViewModels.Add(_objInterestRequestViewmodel);
                }
            }
            if (ListinterestRequestViewModels.Count > 0)
            {
                await new CommonService().PostInterest(ListinterestRequestViewModels);
            }

        }

        private void Hidekeybordlayout_Touch(object sender, View.TouchEventArgs e)
        {
            CommonHelper.Hidekeyboard(this, Window);
        }
    }
}