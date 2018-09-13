using PCL.Common;
using PCL.Helper;
using PCL.Model;
using System;
using System.Threading.Tasks;

namespace PCL.Service
{
    public class ChatService
    {
        HttpClientHelper _helper;

        public ChatService()
        {
            _helper = new HttpClientHelper();
        }
        public async Task<InpowerResult> getChatAll(UserRequestViewModel model)
        {
            InpowerResult resp = null;
            try
            {

                resp = await _helper.Get<InpowerResult>(GlobalConstant.ChatUrls.getAllChatServiceUrl.ToString());
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
        public async Task<InpowerResult> PostChat(ChatMessageViewModel model)
        {
            InpowerResult resp = null;
            try
            {

                resp = await _helper.Post<ChatMessageViewModel>(model,GlobalConstant.ChatUrls.PostChat.ToString());
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

        public async Task<InpowerResult> GetChatMessagesUptoId(long contactId,  string unixTimeStamp)
        {

            InpowerResult resp = null;
            try
            {

                resp = await _helper.Get<InpowerResult>(GlobalConstant.ChatUrls.PrivateChatMessage.ToString() + "?contactId=" + contactId);//&unixTicks=" + unixTimeStamp
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
