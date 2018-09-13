using PCL.Common;
using PCL.Helper;
using PCL.Model;
using System;
using System.Threading.Tasks;

namespace PCL.Service
{

    public class BookShelfService
    {
        HttpClientHelper _helper;

        public BookShelfService()
        {
            _helper = new HttpClientHelper();
        }
        //public async Task<InpowerResult> getAllBooks()
        //{
        //    InpowerResult resp = null;
        //    try
        //    {

        //        resp = await _helper.Get<InpowerResult>(GlobalConstant.BookShelfUrls.getAllBooksUrl.ToString());
        //        return resp;
        //    }
        //    catch (Exception ex)
        //    {
        //        CrashReportService crashReport = new CrashReportService();
        //        CrashReportModel CR = new CrashReportModel();
        //        CR.Filename = "BookShelfService";
        //        CR.Eventname = "GetAllBooks";
        //        // CR.UserID = GlobalClass.UserID == null ? "0" : GlobalClass.UserID;
        //        CR.ErrorMsg = ex.Message + ex.StackTrace;
        //        await crashReport.SendCrashReport(CR, GlobalConstant.CrashUrl);
        //        return resp;
        //    }
        //}

        public async Task<InpowerResult> GetAllBooks(PaginationModel paginationModel)
        {
            BooksMapViewModel model = new BooksMapViewModel();
            model.IsRead = paginationModel.Status;
            model.SkipRecords = paginationModel.SkipRecords;
            model.TakeRecords = paginationModel.TakeRecords;
            model.SearchText =  (String.IsNullOrEmpty(paginationModel.SearchText)?"": paginationModel.SearchText);
            InpowerResult resp = null;
            try
            {
                resp = await _helper.Post<BooksMapViewModel>(model, GlobalConstant.BookShelfUrls.getAllBooksUrl.ToString());
                return resp;
            }
            catch (Exception ex)
            {
                CrashReportService crashReport = new CrashReportService();
                CrashReportModel CR = new CrashReportModel();
                CR.Filename = "BookShelfService";
                CR.Eventname = "PostBook";
                // CR.UserID = GlobalClass.UserID == null ? "0" : GlobalClass.UserID;
                CR.ErrorMsg = ex.Message + ex.StackTrace;
                await crashReport.SendCrashReport(CR, GlobalConstant.CrashUrl);
                return resp;
            }
        }

        public async Task<InpowerResult> PostBook(BooksMapViewModel model)
        {
            InpowerResult resp = null;
            try
            {
                resp = await _helper.Post<BooksMapViewModel>(model, GlobalConstant.BookShelfUrls.PostBookUrl.ToString());
                return resp;
            }
            catch (Exception ex)
            {
                CrashReportService crashReport = new CrashReportService();
                CrashReportModel CR = new CrashReportModel();
                CR.Filename = "BookShelfService";
                CR.Eventname = "PostBook";
                // CR.UserID = GlobalClass.UserID == null ? "0" : GlobalClass.UserID;
                CR.ErrorMsg = ex.Message + ex.StackTrace;
                await crashReport.SendCrashReport(CR, GlobalConstant.CrashUrl);
                return resp;
            }
        }

        public async Task<InpowerResult> UpdateBook(BooksMapViewModel model)
        {
            InpowerResult resp = null;
            try
            {
                resp = await _helper.Post<BooksMapViewModel>(model, GlobalConstant.BookShelfUrls.UpdateBookUrl.ToString());
                return resp;
            }
            catch (Exception ex)
            {
                CrashReportService crashReport = new CrashReportService();
                CrashReportModel CR = new CrashReportModel();
                CR.Filename = "BookShelfService";
                CR.Eventname = "PostBook";
                // CR.UserID = GlobalClass.UserID == null ? "0" : GlobalClass.UserID;
                CR.ErrorMsg = ex.Message + ex.StackTrace;
                await crashReport.SendCrashReport(CR, GlobalConstant.CrashUrl);
                return resp;
            }
        }

        public async Task<InpowerResult> RemoveBook(BooksMapViewModel model)
        {
            InpowerResult resp = null;
            try
            {
                resp = await _helper.Post<BooksMapViewModel>(model, GlobalConstant.BookShelfUrls.RemoveBookUrl.ToString());
                return resp;
            }
            catch (Exception ex)
            {
                CrashReportService crashReport = new CrashReportService();
                CrashReportModel CR = new CrashReportModel();
                CR.Filename = "BookShelfService";
                CR.Eventname = "PostBook";
                // CR.UserID = GlobalClass.UserID == null ? "0" : GlobalClass.UserID;
                CR.ErrorMsg = ex.Message + ex.StackTrace;
                await crashReport.SendCrashReport(CR, GlobalConstant.CrashUrl);
                return resp;
            }
        }
    }
}
