using System;
namespace InPowerIOS.Models
{
    public class MediaFile
    {
        public string FilePath { get; set; }
        public MediaType MediaType { get; set; }
        public long EventID { get; set; }
    }

    public enum MediaType : int
    {
        Photo = 1,
        Video = 2
    }
}
