using System;
using UIKit;

namespace InPowerIOS.Common
{
    public static class CommonSearchView
    {
        public static UISearchBar Create()
        {
            UISearchBar searchBar = new UISearchBar();  
            searchBar.SizeToFit();
            searchBar.ShowsCancelButton = true;
            searchBar.AutocorrectionType = UITextAutocorrectionType.No;  
            searchBar.AutocapitalizationType = UITextAutocapitalizationType.None; 
            searchBar.SearchBarStyle = UISearchBarStyle.Minimal;
            return searchBar;
        }
    }
}
