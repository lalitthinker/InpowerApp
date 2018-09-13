using PCL.Common;
using PCL.Helper;
using PCL.Model;
using PCL.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCL
{
   public class GroupChatService
    {
        HttpClientHelper _helper;

        public GroupChatService()
        {
            _helper = new HttpClientHelper();
        }
        public async Task<InpowerResult> CreateGroup(GroupRequestViewModel model)
        {
            InpowerResult resp = null;
            try
            {

                resp = await _helper.Post<GroupRequestViewModel>(model, GlobalConstant.GroupChatUrls.PostGroupUrl.ToString());
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
        public async Task<InpowerResult> GetGroup()
        {
            InpowerResult resp = null;
            try
            {

                resp = await _helper.Get<InpowerResult>(GlobalConstant.GroupChatUrls.GetGroupUrl.ToString());
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

        public async Task<InpowerResult>  GetGroupMessagesUpto(long groupId, string unixTimeStamp)
        {
            InpowerResult resp = null;
            try
            {

                resp = await _helper.Get<InpowerResult>(GlobalConstant.GroupChatUrls.GetGroupMessagesUptoUrl.ToString() + "?groupId=" + groupId + "&unixTicks=" + unixTimeStamp);
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

        public async Task<InpowerResult> PostGroupMessageService(GroupMessageRequestViewModel groupSend)
        {
            InpowerResult resp = null;
            try
            {

                resp = await _helper.Post<GroupMessageRequestViewModel>(groupSend,GlobalConstant.GroupChatUrls.PostMessageGroupUrl.ToString());
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
