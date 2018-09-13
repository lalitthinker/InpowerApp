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
    public class AccountService
    {

        HttpClientHelper _helper;

        public AccountService()
        {
            _helper = new HttpClientHelper();
        }
        public  async Task<InpowerResult> Registration(UserRegisterRequestViewModel model, string url)
        {
            InpowerResult resp = null;
            try
            {
  
                resp = await _helper.Post<UserRegisterRequestViewModel>(model, url);
                return resp;
            }
            catch (Exception ex)
            {
                CrashReportService crashReport = new CrashReportService();
                CrashReportModel CR = new CrashReportModel();
                CR.Filename = "Registration";
                CR.Eventname = "AccountService";
               // CR.UserID = GlobalClass.UserID == null ? "0" : GlobalClass.UserID;
                CR.ErrorMsg = ex.Message + ex.StackTrace;
                await crashReport.SendCrashReport(CR,GlobalConstant.CrashUrl);
                return resp;
            }
        }

        public async Task<TokenResponsetViewModel>  AccessToken(TokenRequestViewModel tokenRequestViewModel)
        {

          var  resp = await _helper.PostToken(tokenRequestViewModel);
           return   JsonConvert.DeserializeObject<TokenResponsetViewModel>(resp);
           
        }

        public async Task<InpowerResult> Login(UserLoginRequestViewModel model,string Url)
        {
            InpowerResult resp = null;
            try
            {

                resp = await _helper.Post<UserLoginRequestViewModel>(model, Url);
                return resp;
            }
            catch (Exception ex)
            {
                CrashReportService crashReport = new CrashReportService();
                CrashReportModel CR = new CrashReportModel();
                CR.Filename = "Registration";
                CR.Eventname = "AccountService";
                // CR.UserID = GlobalClass.UserID == null ? "0" : GlobalClass.UserID;
                CR.ErrorMsg = ex.Message + ex.StackTrace;
                await crashReport.SendCrashReport(CR, GlobalConstant.CrashUrl);
                return resp;
            }
        }


      




    }
}
