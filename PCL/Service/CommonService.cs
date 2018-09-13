using ModernHttpClient;
using Newtonsoft.Json;
using PCL.Common;
using PCL.Helper;
using PCL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
namespace PCL.Service
{
    public class CommonService
    {

        HttpClientHelper _helper;

        public CommonService()
        {
            _helper = new HttpClientHelper();
        }
        public async Task<InpowerResult> GetInterest()
        {
            InpowerResult resp = null;
            try
            {

                resp = await _helper.Get<InpowerResult>(GlobalConstant.CommonUrls.GetInterestServiceUrl);
                return resp;
            }
            catch (Exception ex)
            {
                CrashReportService crashReport = new CrashReportService();
                CrashReportModel CR = new CrashReportModel();
                CR.Filename = "Interst";
                CR.Eventname = "InetrestService";
                // CR.UserID = GlobalClass.UserID == null ? "0" : GlobalClass.UserID;
                CR.ErrorMsg = ex.Message + ex.StackTrace;
                await crashReport.SendCrashReport(CR, GlobalConstant.CrashUrl);
                return resp;
            }
        }

        public async Task<InpowerResult> PostInterest(List<InterestRequestViewModel> listinterestRequestViewModels)
        {
            InpowerResult resp = null;
            try
            {

                resp = await _helper.PostList<InterestRequestViewModel>(listinterestRequestViewModels, GlobalConstant.CommonUrls.PostInterestServiceUrl);
                return resp;
            }
            catch (Exception ex)
            {
                CrashReportService crashReport = new CrashReportService();
                CrashReportModel CR = new CrashReportModel();
                CR.Filename = "Interst";
                CR.Eventname = "InetrestService";
                // CR.UserID = GlobalClass.UserID == null ? "0" : GlobalClass.UserID;
                CR.ErrorMsg = ex.Message + ex.StackTrace;
                await crashReport.SendCrashReport(CR, GlobalConstant.CrashUrl);
                return resp;
            }
        }

        public async Task<InpowerResult> PostSuggestInterest(SuggestedInterestsRequestViewModel model)
        {
            InpowerResult resp = null;
            try
            {

                resp = await _helper.Post<SuggestedInterestsRequestViewModel>(model, GlobalConstant.CommonUrls.PostSuggestInterestServiceUrl);
                return resp;
            }
            catch (Exception ex)
            {
                CrashReportService crashReport = new CrashReportService();
                CrashReportModel CR = new CrashReportModel();
                CR.Filename = "Interst";
                CR.Eventname = "InetrestService";
                // CR.UserID = GlobalClass.UserID == null ? "0" : GlobalClass.UserID;
                CR.ErrorMsg = ex.Message + ex.StackTrace;
                await crashReport.SendCrashReport(CR, GlobalConstant.CrashUrl);
                return resp;
            }
        }
        public class AndroidTokenViewModel
        {
            public string AndroidToken { get; set; }
        }
        public async void PostUpdateAndroidToken(AndroidTokenViewModel model)
        {
            InpowerResult resp = null;
            try
            {
               await _helper.Post<AndroidTokenViewModel>(model, GlobalConstant.CommonUrls.PostAndroidTokenServiceUrl);
               
               
           
            }
            catch (Exception ex)
            {
               
            }
        }
    }
}
