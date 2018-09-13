using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCL.Model
{
     public class CrashReportModel
      {
        public int ID { get; set; }
        public string UserID { get; set; }
        public string Filename { get; set; }
        public string Eventname { get; set; }
        public string ErrorMsg { get; set; }
        public string CreateDate { get; set; }
    }
}
