using Foundation;
using System;
using UIKit;
using InPowerIOS.Repositories;
using InPowerIOS.Common;
using SDWebImage;
using PCL.Model;
using PCL.Service;
using PCL.Common;
using Newtonsoft.Json;
using BigTed;
using Plugin.Connectivity;
using System.IO;
using Microsoft.AppCenter.Crashes;

namespace InPowerIOS.SideBarMenu
{
    public partial class UpdateUserProfileTableViewController : UITableViewController
    {
        UIImagePickerController imagePicker;
        UIImage PhotoCapture;
        string ProfileImageURL;
        string documentsDirectory, filePath;
        string mediaType = "Photo";
        UIButton cameraButton;
        public UpdateUserProfileTableViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ivUserProfilePic.UserInteractionEnabled = true;
            var selectivUserProfilePicTapped = new UITapGestureRecognizer(() => { ImageAttacted(); });

            ivUserProfilePic.AddGestureRecognizer(selectivUserProfilePicTapped);

            Title = "Update Your Profile";
            FillProfileData();


            imagePicker = new UIImagePickerController();
            imagePicker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
            imagePicker.MediaTypes = UIImagePickerController.AvailableMediaTypes(UIImagePickerControllerSourceType.PhotoLibrary);
            imagePicker.FinishedPickingMedia += Handle_FinishedPickingMedia;
            imagePicker.Canceled += Handle_Canceled;

        }
        protected void Handle_FinishedPickingMedia(object sender, UIImagePickerMediaPickedEventArgs e)
        {
            // determine what was selected, video or image
            bool isImage = false;
            switch (e.Info[UIImagePickerController.MediaType].ToString())
            {
                case "public.image":
                    Console.WriteLine("Image selected");
                    isImage = true;
                    break;
                case "public.video":
                    Console.WriteLine("Video selected");
                    break;
            }

            // get common info (shared between images and video)

            var referenceURL = (NSUrl)e.Info.ValueForKey(new NSString("UIImagePickerControllerImageURL"));


            //  stringTaskCompletionSource.SetResult(url.Path);
            // NSUrl referenceURL = e.Info[new NSString("UIImagePickerControllerReferenceURL")] as NSUrl;
            if (referenceURL != null)
                Console.WriteLine("Url:" + referenceURL.Path.ToString());

            // if it was an image, get the other image info
            if (isImage)
            {

                // get the original image
                string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                UIImage originalImage = e.Info[UIImagePickerController.OriginalImage] as UIImage;
                NSData data = originalImage.AsJPEG();
                filePath = referenceURL.AbsoluteString;

                if (originalImage != null)
                {
                    // do something with the image
                    Console.WriteLine("got the original image");
                    ivUserProfilePic.Image = originalImage; // display
                }


                // This bit of code saves to the application's Documents directory, doesn't save metadata

                documentsDirectory = Environment.GetFolderPath
                                              (Environment.SpecialFolder.Personal);
                string imagename = "myProfileTemp" + DateTime.UtcNow.ToString("ddMMyyyyhhmmss") + ".jpg";
                filePath = System.IO.Path.Combine(documentsDirectory, imagename);
                NSData imgData = originalImage.AsJPEG();
                NSError err = null;

                if (imgData.Save(filePath, false, out err))
                {
                    Console.WriteLine("saved as " + filePath);
                }
                else
                {
                    Console.WriteLine("NOT saved as" + filePath + " because" + err.LocalizedDescription);
                }
            }
            else
            { // if it's a video
              // get video url
                NSUrl mediaURL = e.Info[UIImagePickerController.MediaURL] as NSUrl;
                if (mediaURL != null)
                {
                    Console.WriteLine(mediaURL.ToString());
                }
            }
            // dismiss the picker
            imagePicker.DismissModalViewController(true);
        }

        void Handle_Canceled(object sender, EventArgs e)
        {
            imagePicker.DismissModalViewController(true);
        }
        private void ImageAttacted()
        {
            UIAlertController _ImageSelection = new UIAlertController();
            _ImageSelection.AddAction(UIAlertAction.Create("Camera", UIAlertActionStyle.Default, (action) => SelectImageFromCamera()));
            _ImageSelection.AddAction(UIAlertAction.Create("Photo Library", UIAlertActionStyle.Default, (action) => SelectImageFromLibrary()));
            _ImageSelection.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, (action) => cancelUploadImage()));

            UIPopoverPresentationController presentationPopover = _ImageSelection.PopoverPresentationController;
            if (presentationPopover != null)
            {
                presentationPopover.SourceView = this.View;
                presentationPopover.PermittedArrowDirections = UIPopoverArrowDirection.Up;
            }
            // Display the pop up for options for selecting image
            this.PresentViewController(_ImageSelection, true, null);
        }

        private void SelectImageFromCamera()
        {

            #region FromCamera      
            Camera.TakePicture(this, (obj) =>
            {


                PhotoCapture = new UIImage();
                PhotoCapture = obj.ValueForKey(new NSString("UIImagePickerControllerOriginalImage")) as UIImage;
                ivUserProfilePic.Image = PhotoCapture;

                // This bit of code saves to the application's Documents directory, doesn't save metadata

                documentsDirectory = Environment.GetFolderPath
                                              (Environment.SpecialFolder.Personal);
                string imagename = "myProfileTemp" + DateTime.UtcNow.ToString("ddMMyyyyhhmmss") + ".jpg";
                filePath = System.IO.Path.Combine(documentsDirectory, imagename);
                NSData imgData = PhotoCapture.AsJPEG();
                NSError err = null;

                if (imgData.Save(filePath, false, out err))
                {
                    Console.WriteLine("saved as " + filePath);
                }
                else
                {
                    Console.WriteLine("NOT saved as" + filePath + " because" + err.LocalizedDescription);
                }
            });
            #endregion
        }

        private void SelectImageFromLibrary()
        {
            #region FromLibrary
            //for selecting image from library
            NavigationController.PresentModalViewController(imagePicker, true);
            #endregion
        }

        private void cancelUploadImage()
        {

        }

        private void FillProfileData()
        {
            CommonHelper.SetCircularImage(ivUserProfilePic);
            var UserprofileRepo = UserProfileRepository.GetUserProfile(CommonHelper.GetUserId());
            txtZipCode.Text = (UserprofileRepo.ZipCode.ToString() != null) ? UserprofileRepo.ZipCode.ToString() : "";
            txtCity.Text = (UserprofileRepo.City != null) ? UserprofileRepo.City.ToString() : "";
            txtState.Text = (UserprofileRepo.State != null) ? UserprofileRepo.State.ToString() : "";
            txtCountry.Text = (UserprofileRepo.Country != null) ? UserprofileRepo.Country.ToString() : "";
            txtAboutMe.Text = (UserprofileRepo.AboutMe != null) ? UserprofileRepo.AboutMe.ToString() : "";

            if (!string.IsNullOrEmpty(UserprofileRepo.ProfileImageUrl))
            {
                ivUserProfilePic.SetImage(new NSUrl(UserprofileRepo.ProfileImageUrl), UIImage.FromBundle("default_profile.png"));
            }
            else
            {
                ivUserProfilePic.Image = new UIImage("default_profile.png");
            }
        }


        partial void BtnUpdateProfile_TouchUpInside(UIButton sender)
        {
            UpdateProfileData();
        }


        public async void UpdateProfileData()
        {
            InpowerResult Result = null;
            try
            {
                if (txtZipCode.Text != "")
                {
                    if (txtCity.Text != "")
                    {
                        if (txtState.Text != "")
                        {
                            if (txtCountry.Text != "")
                            {
                                if (txtAboutMe.Text != "")
                                {
                                    if (CrossConnectivity.Current.IsConnected)
                                    {

                                        BTProgressHUD.Show("Please Wait", maskType: ProgressHUD.MaskType.Black);

                                        var UserprofileRepo = UserProfileRepository.GetUserProfile(CommonHelper.GetUserId());

                                        Result = await new AccountService().Registration(new UserRegisterRequestViewModel
                                        {
                                            Email = UserprofileRepo.Email,
                                            Password = UserprofileRepo.Password,

                                            ZipCode = Convert.ToInt32(txtZipCode.Text),
                                            City = txtCity.Text,
                                            State = txtState.Text,
                                            Country = txtCountry.Text,
                                            AboutMe = txtAboutMe.Text,


                                        }, GlobalConstant.AccountUrls.RegisterServiceUrl);

                                        if (Result.Status == 1)
                                        {

                                            var modelReporeg = JsonConvert.DeserializeObject<UserRegisterResponseViewModel>(Result.Response.ToString());
                                            if (!string.IsNullOrEmpty(filePath))
                                            {
                                                uploadMedia(filePath, mediaType, modelReporeg);
                                            }
                                            else
                                            {
                                                UserprofileRepo.ZipCode = modelReporeg.ZipCode;
                                                UserprofileRepo.City = modelReporeg.City;
                                                UserprofileRepo.State = modelReporeg.State;
                                                UserprofileRepo.Country = modelReporeg.Country;

                                                UserprofileRepo.AboutMe = modelReporeg.AboutMe;
                                                UserprofileRepo.ProfileImageUrl = modelReporeg.ProfileImageUrl;



                                                UserProfileRepository.UpdateUserProfile(UserprofileRepo);
                                                new UIAlertView("Update Your Profile", "Profile Updated Successfully", null, "OK", null).Show();

                                                BTProgressHUD.Dismiss();
                                                this.DismissViewController(true, null);
                                            }
                                        }
                                        else
                                        {
                                            BTProgressHUD.Dismiss();
                                            new UIAlertView("Update Your Profile", Result.Message, null, "OK", null).Show();
                                            return;
                                        }
                                    }

                                    else
                                    {
                                        new UIAlertView("Update Your Profile", "You're not connected to a Network", null, "OK", null).Show();
                                        return;
                                    }
                                }
                                else
                                {
                                    txtAboutMe.BecomeFirstResponder();
                                    new UIAlertView("Update Your Profile", "Please Enter About Me First", null, "OK", null).Show();
                                }
                            }
                            else
                            {
                                txtCountry.BecomeFirstResponder();
                                new UIAlertView("Update Your Profile", "Please Enter Country First", null, "OK", null).Show();
                            }
                        }
                        else
                        {
                            txtState.BecomeFirstResponder();
                            new UIAlertView("Update Your Profile", "Please Enter State First", null, "OK", null).Show();
                        }
                    }
                    else
                    {
                        txtCity.BecomeFirstResponder();
                        new UIAlertView("Update Your Profile", "Please Enter City First", null, "OK", null).Show();
                    }
                }
                else
                {
                    txtZipCode.BecomeFirstResponder();
                    new UIAlertView("Update Your Profile", "Please Enter Zip Code First", null, "OK", null).Show();
                }


            }
            catch (Exception ex)
            {
                BTProgressHUD.Dismiss();
                Crashes.TrackError(ex);
                string ErrorMsg = ex.ToString();
                new UIAlertView("Error", ErrorMsg, null, "OK", null).Show();
            }
        }

        private async void uploadMedia(string filePath, string mediaType, UserRegisterResponseViewModel model)
        {
            var mediaName = System.IO.Path.GetFileName(filePath); //AWSUploader.SetMediaName (mediaType);
            var url = "";
            try
            {
                // BTProgressHUD.Show("Processing media..", maskType: ProgressHUD.MaskType.Black);
                if (mediaType == "Photo")
                    await AWSUploader.AWSUploadImage(filePath, mediaName);
                else
                    await AWSUploader.AWSUploadAudioVideo(filePath, mediaName, mediaType);
                url = AWSUploader.GetMediaUrl(mediaType) + mediaName;


                try
                {
                    if (string.IsNullOrEmpty(url))
                    {
                        //Toast.MakeText(this, "Check Your Internet Connection", ToastLength.Long).Show();

                    }
                    else
                    {
                        var Result = await new AccountService().Registration(new UserRegisterRequestViewModel
                        {
                            Email = model.Email,
                            Password = model.Password,

                            ZipCode = model.ZipCode,
                            City = model.City,
                            State = model.State,
                            Country = model.Country,
                            AboutMe = model.AboutMe,
                            ProfileImageUrl = url,

                        }, GlobalConstant.AccountUrls.RegisterServiceUrl);

                        if (Result.Status == 1)
                        {


                            var modelReporeg = JsonConvert.DeserializeObject<UserRegisterResponseViewModel>(Result.Response.ToString());

                            var UserprofileRepo = UserProfileRepository.GetUserProfile(CommonHelper.GetUserId());
                            UserprofileRepo.ZipCode = modelReporeg.ZipCode;
                            UserprofileRepo.City = modelReporeg.City;
                            UserprofileRepo.State = modelReporeg.State;
                            UserprofileRepo.Country = modelReporeg.Country;

                            UserprofileRepo.AboutMe = modelReporeg.AboutMe;
                            UserprofileRepo.ProfileImageUrl = modelReporeg.ProfileImageUrl;





                            UserProfileRepository.UpdateUserProfile(UserprofileRepo);
                            new UIAlertView("Update Your Profile", "Profile Updated Successfully", null, "OK", null).Show();

                            BTProgressHUD.Dismiss();
                            this.DismissViewController(true, null);
                        }

                    }

                }
                catch (Exception e)
                {
                    Crashes.TrackError(e);
                    BTProgressHUD.Dismiss();
                    new UIAlertView("Error", e.ToString(), null, "OK", null).Show();
                    //Toast.MakeText(this, e.Message, ToastLength.Long).Show();
                }


            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                BTProgressHUD.Dismiss();
                new UIAlertView("Error", e.ToString(), null, "OK", null).Show();
                //Toast.MakeText(this, e.Message, ToastLength.Long).Show();

            }
        }

    }
}