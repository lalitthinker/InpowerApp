using System;
using GlobalToast;
using UIKit;

namespace InPowerIOS.Common
{
    public static class CustomToast
    {
        public static void Show(string Message, bool Success=true,String Title="",bool Default=false)
        {
            ToastAppearance toastLayout = new ToastAppearance();
            if (Default)
            {
                toastLayout.Color = ColorExtensions.DefaultToast();
                Toast.MakeToast(Message).SetAppearance(toastLayout).Show();
            }
            else
            {
                if (Success)
                {
                    toastLayout.Color = ColorExtensions.SuccessToast();
                }
                else
                {
                    toastLayout.Color = ColorExtensions.ErrorToast();
                }
                if (string.IsNullOrEmpty(Title))
                    Toast.MakeToast(Message).SetAppearance(toastLayout).Show();
                else
                {
                    Toast.ShowToast(Message, Title).SetAppearance(toastLayout).Show();
                }
            }

        }
    }
}
