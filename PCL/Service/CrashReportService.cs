using PCL.Helper;
using PCL.Model;
using System.Threading.Tasks;

namespace PCL.Service
{
    public class CrashReportService
    {
        private HttpClientHelper _helper;
        public async Task<InpowerResult> SendCrashReport(CrashReportModel ev,string Url)
        {

            InpowerResult resp;
            resp = await _helper.Post<CrashReportModel>(ev, Url);
            return resp;
        }
    }
}