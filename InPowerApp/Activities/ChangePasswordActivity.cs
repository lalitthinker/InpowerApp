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
using Android.Support.V7.App;
using Android.Graphics.Drawables;
using PCL.Service;
using PCL.Model;
using InPowerApp.Common;

namespace InPowerApp.Activities
{
    [Activity(Label = "InPowerApp")]
    public class ChangePasswordActivity : AppCompatActivity
    {
        ProgressDialog progressDialog;
        LinearLayout hidekeybordlayout;
        Button btnChangePasswordReset, btnChangePasswordSave;
        EditText txtUserOldPassword, txtUserNewPassword, txtUserNewConfirmPassword;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ChangePasswordlayout);

            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.app_bar);
            SetSupportActionBar(toolbar);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            //SupportActionBar.SetDisplayShowHomeEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);


            btnChangePasswordReset = FindViewById<Button>(Resource.Id.btnChangePasswordReset);
            btnChangePasswordSave = FindViewById<Button>(Resource.Id.btnChangePasswordSave);

            txtUserOldPassword = FindViewById<EditText>(Resource.Id.txtUserOldPassword);
            txtUserNewPassword = FindViewById<EditText>(Resource.Id.txtUserNewPassword);
            txtUserNewConfirmPassword = FindViewById<EditText>(Resource.Id.txtUserNewConfirmPassword);

            hidekeybordlayout = FindViewById<LinearLayout>(Resource.Id.hidekeybordlayout);

            btnChangePasswordReset.Click += BtnChangePasswordReset_Click;
            btnChangePasswordSave.Click += BtnChangePasswordSave_ClickAsync;

            hidekeybordlayout.Touch += Hidekeybordlayout_Touch;
        }

        private async void BtnChangePasswordSave_ClickAsync(object sender, EventArgs e)
        {

            try
            {
                Drawable icon_error = Resources.GetDrawable(Resource.Drawable.alert);
                icon_error.SetBounds(0, 0, 25, 50);

                if (txtUserOldPassword.Text != "")
                {
                    if (txtUserNewPassword.Text != "")
                    {
                        if (txtUserNewConfirmPassword.Text != "")
                        {
                            if (txtUserNewPassword.Text == txtUserNewConfirmPassword.Text)
                            {
                                if (txtUserNewPassword.Text == txtUserNewConfirmPassword.Text)
                                {
                                    var RquestChangePasswordViewModel = new ChangePasswordViewModel
                                    {
                                        OldPassword = txtUserOldPassword.Text,
                                        NewPassword = txtUserNewPassword.Text,
                                        ConfirmPassword = txtUserNewConfirmPassword.Text,
                                    };


                                    var result = await new SettingService().PostChangePasswordInterest(RquestChangePasswordViewModel);


                                    if (result != null)
                                    {
                                        Console.WriteLine("password change succesfull");
                                    }
                                    else
                                    {
                                        Console.WriteLine("wrong old password ");

                                    }



                                    clearAll();
                                    this.Finish();
                                }
                                else
                                {
                                    Console.WriteLine("Your password and confirmation password do not match");
                                }

                            }
                            else
                            {
                                txtUserNewConfirmPassword.RequestFocus();
                                txtUserNewConfirmPassword.SetError("Password Not Match", icon_error);
                            }
                        }
                        else
                        {
                            txtUserNewConfirmPassword.RequestFocus();
                            txtUserNewConfirmPassword.SetError("Please Enter Confirm Password First", icon_error);
                        }
                    }
                    else
                    {
                        txtUserNewPassword.RequestFocus();
                        txtUserNewPassword.SetError("Please Enter New Password First", icon_error);
                    }
                }
                else
                {
                    txtUserOldPassword.RequestFocus();
                    txtUserOldPassword.SetError("Please Enter Old Password  First", icon_error);
                }


            }
            catch (Exception ex)
            {
                progressDialog.Hide();
                string ErrorMsg = ex.ToString();
                Toast.MakeText(this, ErrorMsg, ToastLength.Long).Show();
            }
        }

        private void BtnChangePasswordReset_Click(object sender, EventArgs e)
        {
            clearAll();
        }

        public void clearAll()
        {
            txtUserOldPassword.Text = "";
            txtUserNewPassword.Text = "";
            txtUserNewConfirmPassword.Text = "";
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.action_menuOKOK)
            {
                this.Finish();
            }

            else if (item.ItemId == Android.Resource.Id.Home)
            {
                Finish();
            }
            return base.OnOptionsItemSelected(item);
        }


        protected override void OnResume()
        {
            SupportActionBar.SetTitle(Resource.String.ChangePassword);
            base.OnResume();
        }
        private void Hidekeybordlayout_Touch(object sender, View.TouchEventArgs e)
        {
            CommonHelper.Hidekeyboard(this, Window);
        }
    }
}