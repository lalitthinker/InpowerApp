using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCL.Model
{
   
    public class InterestResponseViewModel
    {
        public long  InterestId { get; set; }
        public string Name { get; set; }
        public bool InterestCheck { get; set; }
        public bool isSelected()
        {
            return InterestCheck;
        }
        public void setSelected(bool selected)
        {
            this.InterestCheck = selected;
        }
    }
    public class InterestRequestViewModel
    {
        public long InterestId { get; set; }
        public long UserId { get; set; }

    }
    public class SuggestedInterestsRequestViewModel
    {
        

        public string InterestName { get; set; }
        public int UserId { get; set; }
    }

}
