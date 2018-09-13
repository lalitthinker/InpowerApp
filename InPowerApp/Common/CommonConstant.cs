using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace InPowerApp.Common
{
   public class CommonConstant
    {
        public static string DBName = "InPowerTest.db";
        public static string DBPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), DBName);

        public static int MessagePageSize = 30;
    }
}