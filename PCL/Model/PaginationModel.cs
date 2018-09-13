using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCL.Model
{
    public class PaginationModel
    {
        public int Status { get; set; }
        public int SkipRecords { get; set; }
        public int TakeRecords = 30;
        public string SearchText { get; set; }
    }
}
