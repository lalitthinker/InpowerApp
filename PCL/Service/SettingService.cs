using PCL.Common;
using PCL.Helper;
using PCL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCL.Service
{
   public class SettingService
    {

        HttpClientHelper _helper;

        public SettingService()
        {
            _helper = new HttpClientHelper();
        }
        public async Task<InpowerResult> PostChangePasswordInterest(ChangePasswordViewModel model)
        {
            InpowerResult resp = null;
            try
            {

                resp = await _helper.Post<ChangePasswordViewModel>(model, GlobalConstant.SettingUrls.PostChangePasswordServiceUrl);
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



        public async Task<InpowerResult> PostDeleteAccountInterest(long userid )
        {
            InpowerResult resp = null;
            try
            {

                resp = await _helper.Post(userid, GlobalConstant.SettingUrls.PostDeleteAccountUrl);
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

        public async Task<InpowerResult> PostBlockUserInterest(userdetails BlockUser)
        {
            InpowerResult resp = null;
            try
            {

                resp = await _helper.Post(BlockUser, GlobalConstant.SettingUrls.postBlockUser);
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

        public async Task<InpowerResult> PostUnBlockUserInterest(userdetails BlockUser)
        {
            InpowerResult resp = null;
            try
            {

                resp = await _helper.Post(BlockUser, GlobalConstant.SettingUrls.postBlockUser);
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

        public async Task<InpowerResult> getBlockedContactAll()
        {
            InpowerResult resp = null;
            try
            {

                resp = await _helper.Get<InpowerResult>(GlobalConstant.SettingUrls.getblockedContacturl.ToString());
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
