using System;
using System.Collections.Generic;
using UIKit;

namespace InPowerIOS.Common
{
    public class PickerModel: UIPickerViewModel
    {
        private readonly IList<string> InterestName;
        public class PickerChangedEventArgs
        {
            public string SelectedValue { get; set; }
        }
        public PickerModel(IList<string> InterestName)
        {
            this.InterestName = InterestName;
        }

        public event EventHandler<PickerChangedEventArgs> PickerChanged;

        public override nint GetComponentCount(UIPickerView picker)
        {
            return 1;
        }

        public override nint GetRowsInComponent(UIPickerView picker, nint component)
        {
            return InterestName.Count;
        }

        public override string GetTitle(UIPickerView picker, nint row, nint component)
        {
            return InterestName[(int)row];
        }

        public override nfloat GetRowHeight(UIPickerView picker, nint component)
        {
            return 40f;
        }

        public override void Selected(UIPickerView picker, nint row, nint component)
        {
            if (PickerChanged != null)
            {
                PickerChanged(this, new PickerChangedEventArgs { SelectedValue = InterestName[(int)row] });
            }
        }
    }
}