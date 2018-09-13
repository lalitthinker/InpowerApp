using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCL.Model
{
   public class SettingModel
    {
    }
    public class ChangePasswordViewModel
    {
        public string OldPassword { get; set; }

    
        public string NewPassword { get; set; }

       
        public string ConfirmPassword { get; set; }
    }

    public class userdetails
    {

        public long BlockUserID { get; set; }

    }
}
