using Foundation;
using System;
using UIKit;
using System.Collections.Generic;
using PCL.Service;
using PCL.Model;
using Newtonsoft.Json;
using InPowerIOS.Common;
using System.Linq;
using CoreGraphics;
using BigTed;
using Microsoft.AppCenter.Crashes;

namespace InPowerIOS.Chats
{
    public partial class CreateGroupViewController : UIViewController
    {

        UIImagePickerController imagePicker;
        UIImage PhotoCapture;
        string ProfileImageURL;
        string documentsDirectory, filePath;
        string mediaType = "Photo";
        UIButton cameraButton;


        bool privategroup = false;
        int grouptype = 1;
        int interestID = 0;
        string[] interestString;
        private List<KeyValuePair<string, string>> drpInterestList;
        private const int GROUP_NAME = 30;
        private const int GROUP_DESCRIPTION = 180;

        public CreateGroupViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
           
            NavigationItem.SetRightBarButtonItems(
               new[]
               {
                new UIBarButtonItem(CommonHelper.GetCorrectIcon()
                        , UIBarButtonItemStyle.Plain
                        ,
                        (esender, args) =>
                        {
                    
                    InsertCreateGroupdata();
                })
            }, true);
                
            
            Title = "Create Group";
        
          
            GetAllInterest();
            MakeThisGroupPrivate();
            SetTextFieldTextLimit();
            switchMakeThisGroupPrivate.On = false;

            ivGroupImage.UserInteractionEnabled = true;
            var selectivUserProfilePicTapped = new UITapGestureRecognizer(() => { ImageAttacted(); });

            ivGroupImage.AddGestureRecognizer(selectivUserProfilePicTapped);

            imagePicker = new UIImagePickerController();
            imagePicker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
            imagePicker.MediaTypes = UIImagePickerController.AvailableMediaTypes(UIImagePickerControllerSourceType.PhotoLibrary);
            imagePicker.FinishedPickingMedia += Handle_FinishedPickingMedia;
            imagePicker.Canceled += Handle_Canceled;


            var g = new UITapGestureRecognizer(() => View.EndEditing(false));
            g.CancelsTouchesInView = false;
            View.AddGestureRecognizer(g);

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
                    ivGroupImage.Image = originalImage; // display
                }


                // This bit of code saves to the application's Documents directory, doesn't save metadata

                documentsDirectory = Environment.GetFolderPath
                                              (Environment.SpecialFolder.Personal);
                string imagename = "myGroupTemp" + DateTime.UtcNow.ToString("ddMMyyyyhhmmss") + ".jpg";
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
                ivGroupImage.Image = PhotoCapture;

                // This bit of code saves to the application's Documents directory, doesn't save metadata

                documentsDirectory = Environment.GetFolderPath
                                              (Environment.SpecialFolder.Personal);
                string imagename = "myGroupTemp" + DateTime.UtcNow.ToString("ddMMyyyyhhmmss") + ".jpg";
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
        public void SetTextFieldTextLimit()
        {
            //txtDescription.ScrollEnabled = true;
            txtGroupName.ShouldChangeCharacters +=
                delegate (UITextField textField, NSRange range, string replacementString)
                {
                    if ((range.Location + replacementString.Length) > GROUP_NAME)
                    {
                        return false;
                    }
                    return true;
                };

            txtDescription.ShouldChangeCharacters +=
                delegate (UITextField textField, NSRange range, string replacementString)
                {
                    if ((range.Location + replacementString.Length) > GROUP_DESCRIPTION)
                    {
                        return false;
                    }
                    return true;
                };
        }
    
        public void MakeThisGroupPrivate()
        {
            lblMakeThisGroupPrivate.UserInteractionEnabled = true;
            var selectImageTapped = new UITapGestureRecognizer(() => { MakeThisGroupPrivateClickedEvent(); });
            lblMakeThisGroupPrivate.AddGestureRecognizer(selectImageTapped);
        }

        private void MakeThisGroupPrivateClickedEvent()
        {

            if(switchMakeThisGroupPrivate.On == true)
            {
                switchMakeThisGroupPrivate.On = false;
                privategroup = false;
            }
            else
            {
                switchMakeThisGroupPrivate.On = true;
                privategroup = true;
            }


        }

        private void GetAllInterest()
        {
            drpInterestList = new List<KeyValuePair<string, string>>();
            var result = new CommonService().GetInterest().Result;
            if (result.Status == 1)
            {

                var InterestResult = JsonConvert.DeserializeObject<List<InterestResponseViewModel>>(result.Response.ToString());
                interestString = new string[InterestResult.Count + 1];
                int i = 1;
                interestString[0] = "--Select Interest--";
                drpInterestList.Add(new KeyValuePair<string, string>("--Select Interest--", "0"));
                foreach (var item in InterestResult)
                {
                    drpInterestList.Add(new KeyValuePair<string, string>(item.Name, item.InterestId.ToString()));
                    interestString[i] = item.Name;
                    i++;
                }

            }
            else
            {
                new UIAlertView("Create Group", result.Message, null, "OK", null).Show();
            }



            cmbInterestPicker.Layer.BorderColor = UIColor.Black.CGColor;
            cmbInterestPicker.Layer.BorderWidth = 1;
            cmbInterestPicker.Layer.CornerRadius = 4;
            cmbInterestPicker.TintColor = UIColor.Clear;

            //proximityList = CommonHelper.GetProximityList();
            var Interestmodel = new PickerModel(interestString);
            Interestmodel.PickerChanged += (sender, e) => { cmbInterestPicker.Text = e.SelectedValue; };
            //cmbInterestPicker.Text = CommonHelper.GetDefaultInterest();
            //          ProximityPicker.ShouldChangeText += delegate {
            //              return false;
            //          };
            var picker = new UIPickerView();
            picker.ShowSelectionIndicator = true;
            picker.Model = Interestmodel;
            picker.SelectedRowInComponent(0);
            // Setup the toolbar
            var toolbar = new UIToolbar();
            toolbar.BarStyle = UIBarStyle.Default;
            toolbar.Translucent = false;
            toolbar.SizeToFit();
            // Create a 'done' button for the toolbar and add it to the toolbar
            var doneButton = new UIBarButtonItem("Done", UIBarButtonItemStyle.Done,
                (s, e) =>
                {
                    //interestID = drpInterestList.ToArray();
    
                interestID = Convert.ToInt32(drpInterestList.Where(sa => sa.Key == cmbInterestPicker.Text).FirstOrDefault().Value);


                cmbInterestPicker.ResignFirstResponder();
                //CommonHelper.SetDefaultInterest(cmbInterestPicker.Text);





                });
            toolbar.SetItems(new[] { doneButton }, true);
            // Tell the textbox to use the picker for input
            cmbInterestPicker.InputView = picker;
            // Display the toolbar over the pickers
            cmbInterestPicker.InputAccessoryView = toolbar;
                cmbInterestPicker.Text = "--Select Interest--";

        }

        partial void GroupTypeUISC_ValueChanged(UISegmentedControl sender)
        {
            var index = GroupTypeUISC.SelectedSegment;  
            if(index == 0)  
            {  
                 grouptype = 1;
            }  
            else if(index == 1)  
            {  
                grouptype = 2;
            }  
            else if(index == 2)  
            {  
                grouptype = 3;
            }  

        }


        public void InsertCreateGroupdata()
        {
            if (txtGroupName.Text.Trim() == "")
            {
                txtGroupName.BecomeFirstResponder();
                new UIAlertView("Create Group", "Enter Group Name", null, "OK", null).Show();
                return;
            }
            if (cmbInterestPicker.Text == "--Select Interest--")
            {
                new UIAlertView("Create Group", "Please Select Interest", null, "OK", null).Show();
                return;
            }

            if (txtDescription.Text == "")
            {
                txtDescription.BecomeFirstResponder();
                new UIAlertView("Create Group", "Group Description is empty", null, "OK", null).Show();
                return;
            }
            BTProgressHUD.Show("Please Wait", maskType: ProgressHUD.MaskType.Black);

            GroupRequestViewModel model = new GroupRequestViewModel();
            model = new GroupRequestViewModel
            {
                Name = txtGroupName.Text,
                    Description = txtDescription.Text,
                    GroupType = (GroupType)grouptype,
                    InterestId = interestID,
                    IsPrivate = privategroup
                };
            if (!string.IsNullOrEmpty(filePath))
            {
                uploadMedia(filePath, mediaType, model);
            }
            else
            {
                BTProgressHUD.Dismiss();
                var viewController = (SelectGroupContactViewController)Storyboard.InstantiateViewController("SelectGroupContactViewController");
                viewController.groupmodel = model;
                NavigationController.PushViewController(viewController, true);

              
            }

        
        }



        private async void uploadMedia(string filePath, string mediaType, GroupRequestViewModel model)
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
                        GroupRequestViewModel groupRequestViewModel = new GroupRequestViewModel();
                        groupRequestViewModel = new GroupRequestViewModel
                        {
                            Name = txtGroupName.Text,
                            Description = txtDescription.Text,
                            GroupType = (GroupType)grouptype,
                            InterestId = interestID,
                            IsPrivate = privategroup,
                            PictureUrl = url
                        };
                        BTProgressHUD.Dismiss();
                       
                        InvokeOnMainThread(delegate
                        {
                            var viewController = (SelectGroupContactViewController)Storyboard.InstantiateViewController("SelectGroupContactViewController");
                            viewController.groupmodel = groupRequestViewModel;
                            NavigationController.PushViewController(viewController, true);

                        });
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