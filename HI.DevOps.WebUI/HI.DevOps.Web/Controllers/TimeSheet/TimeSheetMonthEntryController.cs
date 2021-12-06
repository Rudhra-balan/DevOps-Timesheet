using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using HI.DevOps.Application.BussinessManager;
using HI.DevOps.Application.BussinessManagerInterface;
using HI.DevOps.Application.Common.Interfaces.IDevOpsRequestBroker;
using HI.DevOps.Application.Common.Interfaces.IRequestBroker;
using HI.DevOps.DomainCore.Extensions;
using HI.DevOps.DomainCore.Helper.Constant;
using HI.DevOps.DomainCore.Models.DevOps;
using HI.DevOps.DomainCore.Models.Error;
using HI.DevOps.DomainCore.Models.Login;
using HI.DevOps.DomainCore.Models.Response;
using HI.DevOps.DomainCore.Models.TimeSheet;
using HI.DevOps.Web.Common;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace HI.DevOps.Web.Controllers.TimeSheet
{
    [Authorize]
    [Route("TimeSheetMonthEntry")]
    public class TimeSheetMonthEntryController : Controller
    {
        #region Consturctor

        public TimeSheetMonthEntryController(IConfiguration iConfiguration, IMemoryCache memoryCache,
            IRequestBrokerService requestBroker, IDevOpsRequestBroker devOpsRequestBroker)
        {
            _iRequestBrokerService = requestBroker;
            Configuration = iConfiguration;
            _memCache = memoryCache;
            _iDevOpsRequestBrokerService = devOpsRequestBroker;
            _webLog = LogManager.GetLogger(typeof(TimeSheetMonthEntryController));
        }

        #endregion

        #region Private Variable

        private static ILog _webLog;
        public IConfiguration Configuration { get; }
        private readonly IMemoryCache _memCache;
        private readonly IRequestBrokerService _iRequestBrokerService;
        private readonly IDevOpsRequestBroker _iDevOpsRequestBrokerService;

        #endregion

        #region Public Member

        [HttpGet]
        [Route(UrlConstant.MonthView)]
        public IActionResult MonthView()
        {
            ITimeSheetBM timeSheetBM = new TimeSheetBM(_iRequestBrokerService);
            var timeSheetViewModel = timeSheetBM.Initialize(new TimeSheetViewModel());

            var tokenModel = MemoryCacheHelper.GetInMemoryCache<TokenModel>(AppConstants.TokenInfo, _memCache);

            WorkItemAndTimeSheetTaskRunner(tokenModel, timeSheetViewModel);

            return PartialView(UrlConstant.TimeSheetMonthViewCshtml, timeSheetViewModel);
        }

        [HttpPost]
        [Route(UrlConstant.DeleteTimeSheet)]
        public WebClientResponse DeleteTimeSheet(string date , string task)
        {
            return _iRequestBrokerService.PostRequest<ErrorViewModel>(
                $"/HI.DevOps.TimeSheet.Api/deleteTimeSheet/{date}/{task}", false);
        }

        [HttpPost]
        [Route(UrlConstant.SaveTimeWorkItems)]
        public WebClientResponse SaveWeeKTimeWorkItems([FromBody] WeekTimeInfoModel timeSheet)
        {
            var userInfo = MemoryCacheHelper.GetInMemoryCache<UserViewModel>(AppConstants.SessionUser, _memCache);


            if (!timeSheet.ParentLink.IsNullOrEmpty())
            {
                var tokenModel = MemoryCacheHelper.GetInMemoryCache<TokenModel>(AppConstants.TokenInfo, _memCache);
                var parentId = int.Parse(timeSheet.ParentLink.Split('/').Last());
                _iDevOpsRequestBrokerService.GetParentWorkItemById(parentId, tokenModel.AccessToken, timeSheet);
            }

            if (userInfo != null) timeSheet.UserId = userInfo.User.UserIdentityId;
            var response =
                _iRequestBrokerService.PostRequest<WeekTimeInfoModel>("/HI.DevOps.TimeSheet.Api/SaveTimeSheet",
                    timeSheet);
            FormatCalenderEvent(response);
            return response;
        }

        private static void FormatCalenderEvent(WebClientResponse response)
        {
            var workItem = (WeekTimeInfoModel) response.SourceObject;
            var eventItemIndex = 0;

            var calender = new TimeInfoModel();
            if (!workItem.Epic.IsNullOrEmpty())
            {
                eventItemIndex += 1;
                calender.Description += $" {eventItemIndex}) {workItem.Epic} <br/>";
            }

            if (!workItem.Feature.IsNullOrEmpty())
            {
                eventItemIndex += 1;
                calender.Description += $" {eventItemIndex}) {workItem.Feature} <br/>";
            }

            if (!workItem.UserStory.IsNullOrEmpty())
            {
                eventItemIndex += 1;
                calender.Description += $" {eventItemIndex}) {workItem.UserStory} <br/>";
            }

            if (!workItem.Requirements.IsNullOrEmpty())
            {
                eventItemIndex += 1;
                calender.Description += $" {eventItemIndex}){workItem.Requirements} <br/>";
            }

            if (!workItem.Task.IsNullOrEmpty())
            {
                eventItemIndex += 1;
                calender.Description += $" {eventItemIndex}) {workItem.Task} <br/>";
            }

            calender.StartDate = workItem.TimeSheetDate;
            calender.EndDate = workItem.TimeSheetHours > 12
                ? workItem.TimeSheetDate.AddDays(1)
                : workItem.TimeSheetDate;
            calender.Title =
                $"{workItem.Task.Split(" ")[0]} {workItem.Task.Split(" ")[1]} {workItem.TimeSheetHours} hrs";
            calender.Project = workItem.Project;
            calender.Task = workItem.Task;
            calender.TimeSheetId = workItem.TimeSheetId;
            calender.TimeSheetHours = workItem.TimeSheetHours;

            response.SourceObject = calender;
        }

        [HttpPost]
        [Route(UrlConstant.UpdateTimeWorkItems)]
        public WebClientResponse UpdateTimeWorkItems([FromBody] WeekTimeInfoModel timeSheet)
        {
            
            return _iRequestBrokerService.PostRequest<ErrorViewModel>("/HI.DevOps.TimeSheet.Api/UpdateTimeSheet",
                timeSheet);
        }

        [HttpGet]
        [Route(UrlConstant.GetCalenderData)]
        public List<TimeInfoModel> GetTimeSheetInfo()
        {
            var calenderData = new List<TimeInfoModel>();
            var url =
                $"/HI.DevOps.TimeSheet.Api/GetTimeInfoList/{HttpUtility.UrlEncode(HttpContext.Session.GetString(AppConstants.SessionUserID))}";
            var responseObject = _iRequestBrokerService.PostRequest<List<TimeInfoModel>>(url, false);

            if (!(responseObject.SourceObject is List<TimeInfoModel> timeSheetList)) return calenderData;
            if (timeSheetList.Count > 0)
                MemoryCacheHelper.UpdateInMemoryCache(AppConstants.TimeSheetList, timeSheetList, _memCache);

            Parallel.ForEach(timeSheetList, workItem =>
            {
                var eventItemIndex = 0;

                var calender = new TimeInfoModel();
                if (!workItem.Epic.IsNullOrEmpty())
                {
                    eventItemIndex += 1;
                    calender.Description += $" {eventItemIndex}) {workItem.Epic} <br/>";
                }

                if (!workItem.Feature.IsNullOrEmpty())
                {
                    eventItemIndex += 1;
                    calender.Description += $" {eventItemIndex}) {workItem.Feature} <br/>";
                }

                if (!workItem.UserStory.IsNullOrEmpty())
                {
                    eventItemIndex += 1;
                    calender.Description += $" {eventItemIndex}) {workItem.UserStory} <br/>";
                }

                if (!workItem.Requirements.IsNullOrEmpty())
                {
                    eventItemIndex += 1;
                    calender.Description += $" {eventItemIndex}){workItem.Requirements} <br/>";
                }

                if (!workItem.Task.IsNullOrEmpty())
                {
                    eventItemIndex += 1;
                    calender.Description += $" {eventItemIndex}) {workItem.Task} <br/>";
                }

                calender.StartDate = workItem.TimeSheetDate;
                calender.EndDate = workItem.TimeSheetHours > 12
                    ? workItem.TimeSheetDate.AddDays(1)
                    : workItem.TimeSheetDate;
                calender.Title =
                    $"{workItem.Task.Split(" ")[0]} {workItem.Task.Split(" ")[1]} {workItem.TimeSheetHours} hrs";
                calender.Project = workItem.Project;
                calender.Task = workItem.Task;
                calender.TimeSheetId = workItem.TimeSheetId;
                calender.TimeSheetHours = workItem.TimeSheetHours;
                calenderData.Add(calender);
            });

            return calenderData;
        }

        #endregion

        #region Private Member

        private void WorkItemAndTimeSheetTaskRunner(TokenModel tokenModel, TimeSheetViewModel timeSheetViewModel)
        {
            var workItemListTask =
                Task.Factory.StartNew(() => timeSheetViewModel.WorkItemList = GetDevOpsWorkItems(tokenModel));
           
            var timeSheetInfoTask = Task.Factory.StartNew(() =>
            {
                var timeSheetInfo = GetTimeSheetInfo();
                ViewBag.CalendarEvent = JsonConvert.SerializeObject(timeSheetInfo);
            });

            var weekTimeSheetInfoTask = Task.Factory.StartNew(delegate
            {
                var weekTimeSheetInfo = GetWeekTimeSheetInfo();
               
                var (startDate, endDate) = string.Empty.GetWeekStartEndDate();
                var timeSheetModels = weekTimeSheetInfo.FindAll(item =>
                    item.WeekStartDate == startDate || item.WeekEndDate == endDate);
                timeSheetViewModel.NumberOfTimeSheet = timeSheetModels.Count == 0 || timeSheetModels.Count < 5
                    ? 5
                    : timeSheetModels.Count;
                timeSheetViewModel.WeekTimeSheetList = timeSheetModels;
                timeSheetViewModel.WeekTimeSheetIndex = timeSheetViewModel.NumberOfTimeSheet;
            });
         
            Task.WaitAll(workItemListTask, timeSheetInfoTask, weekTimeSheetInfoTask);
        }

        private List<WeekTimeSheetModel> GetWeekTimeSheetInfo()
        {
        
            var url =
                $"/HI.DevOps.TimeSheet.Api/GetWeekTimeByPivotDay/{HttpUtility.UrlEncode(HttpContext.Session.GetString(AppConstants.SessionUserID))}";
            var responseObject = _iRequestBrokerService.PostRequest<List<WeekTimeSheetModel>>(url, false);

            if (!(responseObject.SourceObject is List<WeekTimeSheetModel> timeSheetList)) return new List<WeekTimeSheetModel>();
            if (timeSheetList.Count > 0)
                MemoryCacheHelper.SetInMemoryCache(AppConstants.WeekTimeSheetList, timeSheetList, _memCache);
            return timeSheetList;
        }

      
        private List<HiDevOpsWorkItem> GetDevOpsWorkItems(TokenModel tokenModel)
        {
            var devOpsWorkItemList = new List<HiDevOpsWorkItem>();
            if (tokenModel == null) return devOpsWorkItemList;
            var workItem =
                MemoryCacheHelper.GetInMemoryCache<List<HiDevOpsWorkItem>>(AppConstants.WorkItemList, _memCache);
            if (workItem != null && workItem.Any()) return workItem;
            devOpsWorkItemList = _iDevOpsRequestBrokerService.GetWorkItem(tokenModel.AccessToken);
            MemoryCacheHelper.SetInMemoryCache(AppConstants.WorkItemList, devOpsWorkItemList,
                _memCache);
            return devOpsWorkItemList;
        }

        #endregion
    }
}