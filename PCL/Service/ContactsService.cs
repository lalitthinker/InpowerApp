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
    public class ContactsService
    {
        HttpClientHelper _helper;

        public ContactsService()
        {
            _helper = new HttpClientHelper();
        }
        //public async Task<InpowerResult> GetContactsAll(PaginationModel model)
        //{
        //    InpowerResult resp = null;
        //    try
        //    {

        //        resp = await _helper.Get<InpowerResult>(GlobalConstant.ContactUrls.getAllContactServiceUrl.ToString());
        //        return resp;
        //    }
        //    catch (Exception ex)
        //    {
        //        CrashReportService crashReport = new CrashReportService();
        //        CrashReportModel CR = new CrashReportModel();
        //        CR.Filename = "Registration";
        //        CR.Eventname = "AccountService";
        //        // CR.UserID = GlobalClass.UserID == null ? "0" : GlobalClass.UserID;
        //        CR.ErrorMsg = ex.Message + ex.StackTrace;
        //        await crashReport.SendCrashReport(CR, GlobalConstant.CrashUrl);
        //        return resp;
        //    }
        //}

        public async Task<InpowerResult> GetContactsAll(PaginationModel model)
        {
            model.SearchText = (String.IsNullOrEmpty(model.SearchText) ? "" : model.SearchText);
            InpowerResult resp = null;
            try
            {
                resp = await _helper.Post<PaginationModel>(model, GlobalConstant.ContactUrls.getAllContactServiceUrl.ToString());
                return resp;
            }
            catch (Exception ex)
            {
                CrashReportService crashReport = new CrashReportService();
                CrashReportModel CR = new CrashReportModel();
                CR.Filename = "ContactService";
                CR.Eventname = "GetAllContacts";
                // CR.UserID = GlobalClass.UserID == null ? "0" : GlobalClass.UserID;
                CR.ErrorMsg = ex.Message + ex.StackTrace;
                await crashReport.SendCrashReport(CR, GlobalConstant.CrashUrl);
                return resp;
            }
        }
        public async Task<InpowerResult> GetAllMyContact()
        {
            InpowerResult resp = null;
            try
            {

                resp = await _helper.Get<InpowerResult>(GlobalConstant.ContactUrls.getGetAllMyContactServiceUrl.ToString());
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
        public async Task<InpowerResult> GetContactFriendhipSuggesst()
        {
            InpowerResult resp = null;
            try
            {

                resp = await _helper.Get<InpowerResult>(GlobalConstant.ContactUrls.getFriendshipContactServiceUrl.ToString());
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
        public async Task<InpowerResult> AddContactService(long contactId)
        {
            InpowerResult resp = null;
            try
            {

                resp = await _helper.Get<InpowerResult>(GlobalConstant.ContactUrls.AddContactUrl.ToString() + "?contactId=" + contactId);
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
