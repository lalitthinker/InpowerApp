using AssetsLibrary;
using BigTed;
using CoreGraphics;
using Foundation;
using InPowerIOS.Common;
using System;
using UIKit;

namespace InPowerIOS
{
    public partial class ImageViewController : UIViewController
    {
        UIScrollView scrollView;
        UIImageView imageView;
        UIImage image;
        public string filepath;
        public ImageViewController (IntPtr handle) : base (handle)
        {
            
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            NavigationItem.SetRightBarButtonItem(
                SaveImage, true);

        }

        public override void ViewWillAppear(bool animated)
        {
            InvokeOnMainThread(() =>
            {
                base.ViewWillAppear(animated);
                BTProgressHUD.Show("Loading Image", maskType: ProgressHUD.MaskType.Black);
                scrollView = new UIScrollView(new CGRect(0, 0, View.Frame.Width, View.Frame.Height - NavigationController.NavigationBar.Frame.Height));
                View.AddSubview(scrollView);
                image = ImageClass.FromUrl(filepath);
                imageView = new UIImageView(image);
                scrollView.ContentSize = imageView.Image.Size;
                scrollView.AddSubview(imageView);

                scrollView.MaximumZoomScale = 3f;
                scrollView.MinimumZoomScale = .1f;
                scrollView.ViewForZoomingInScrollView += (UIScrollView sv) => { return imageView; };

                UITapGestureRecognizer doubletap = new UITapGestureRecognizer(OnDoubleTap)
                {
                    NumberOfTapsRequired = 2 // double tap
                };

                scrollView.AddGestureRecognizer(doubletap); // detect when the scrollView is double-tapped
                scrollView.SetZoomScale(0.25f, true);

                BTProgressHUD.Dismiss();
            });
        }

        private void OnDoubleTap(UIGestureRecognizer gesture)
        {
            if (scrollView.ZoomScale >= 1)
                scrollView.SetZoomScale(0.25f, true);
            else
                scrollView.SetZoomScale(2f, true);
        }

        partial void BBSaveImageClick(UIBarButtonItem sender)
        {
            try
            {
                image.SaveToPhotosAlbum(HandleSaveStatus);
                BTProgressHUD.Dismiss();
            }
            catch(Exception ex)
            {
                
            }
        }

        void HandleSaveStatus(UIImage image, NSError error)
        {
            if(error==null)
            CustomToast.Show("Image Saved to your library");
            else
                CustomToast.Show("Image Save Failed",false);
        }

    }
}