using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HI.DevOps.Application.Common.Interfaces.IDevOpsRequestBroker;
using HI.DevOps.Application.Common.Interfaces.IRequestBroker;
using HI.DevOps.DomainCore.Helper.Constant;
using HI.DevOps.DomainCore.Models.DevOps;
using HI.DevOps.DomainCore.Models.Error;
using HI.DevOps.DomainCore.Models.Login;
using HI.DevOps.DomainCore.Models.Response;
using HI.DevOps.DomainCore.Models.TimeSheet;
using HI.DevOps.Web.Common;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using HI.DevOps.DomainCore.Extensions;
using Microsoft.AspNetCore.Http;

namespace HI.DevOps.Web.Controllers.TimeSheet
{
    [Authorize]
    [Route("TimeSheetWeekEntry")]
    public class TimeSheetWeekEntryController : Controller
    {
        public TimeSheetWeekEntryController(IMemoryCache memoryCache, IRequestBrokerService requestBroker,
            IDevOpsRequestBroker devOpsRequestBroker)
        {
            _iRequestBrokerService = requestBroker;
            _memCache = memoryCache;
            _iDevOpsRequestBrokerService = devOpsRequestBroker;
            _webLog = LogManager.GetLogger(typeof(TimeSheetWeekEntryController));
        }

        [HttpGet]
        [Route(UrlConstant.WeekView)]
        public IActionResult WeekView()
        {
          
            var timeSheetModel = new TimeSheetViewModel
            {
                NumberOfTimeSheet = 5,
                WorkItemList =
                    MemoryCacheHelper.GetInMemoryCache<List<HiDevOpsWorkItem>>(AppConstants.WorkItemList, _memCache)
            };
            return PartialView(UrlConstant.TimeSheetWeekViewCshtml, timeSheetModel);
        }


        [HttpGet]
        [Route(UrlConstant.NextPrevious)]
        public IActionResult NextPrevious(string startDate,string endDate)
        {
            var timeSheetModel = new TimeSheetViewModel
            {
             
                WeekTimeSheetList = MemoryCacheHelper.GetInMemoryCache<List<WeekTimeSheetModel>>(AppConstants.WeekTimeSheetList, _memCache).FindAll(item =>
                item.WeekStartDate == Convert.ToDateTime(startDate) || item.WeekEndDate == Convert.ToDateTime(endDate)),
                WorkItemList =
                    MemoryCacheHelper.GetInMemoryCache<List<HiDevOpsWorkItem>>(AppConstants.WorkItemList, _memCache)
            };
            timeSheetModel.NumberOfTimeSheet = timeSheetModel.WeekTimeSheetList.Count == 0 || timeSheetModel.WeekTimeSheetList.Count < 5 ? 5 : timeSheetModel.WeekTimeSheetList.Count;
            return PartialView(UrlConstant.TimeSheetWeekViewCshtml, timeSheetModel);
        }

        [HttpGet]
        [Route(UrlConstant.WeekListByDate)]
        public IActionResult WeekListByDate(string startDate, string endDate)
        {

            var url =
                $"/HI.DevOps.TimeSheet.Api/GetWeekTimeByPivotDay/{HttpUtility.UrlEncode(HttpContext.Session.GetString(AppConstants.SessionUserID))}";
            var responseObject = _iRequestBrokerService.PostRequest<List<WeekTimeSheetModel>>(url, false);

            
            switch (responseObject.SourceObject)
            {
                case List<WeekTimeSheetModel> timeSheetList when timeSheetList.Count > 0:
                    MemoryCacheHelper.UpdateInMemoryCache(AppConstants.WeekTimeSheetList, timeSheetList, _memCache);
                    break;
               default:
                    return WeekView();
            }
          
            var timeSheetModel = new TimeSheetViewModel
            {

                WeekTimeSheetList = MemoryCacheHelper.GetInMemoryCache<List<WeekTimeSheetModel>>(AppConstants.WeekTimeSheetList, _memCache).FindAll(item =>
                    item.WeekStartDate == Convert.ToDateTime(startDate) || item.WeekEndDate == Convert.ToDateTime(endDate)),
                WorkItemList =
                    MemoryCacheHelper.GetInMemoryCache<List<HiDevOpsWorkItem>>(AppConstants.WorkItemList, _memCache)
            };
            timeSheetModel.NumberOfTimeSheet = timeSheetModel.WeekTimeSheetList.Count == 0 || timeSheetModel.WeekTimeSheetList.Count < 5 ? 5 : timeSheetModel.WeekTimeSheetList.Count;
            timeSheetModel.WeekTimeSheetIndex = timeSheetModel.WeekTimeSheetList.Count;
            return PartialView(UrlConstant.TimeSheetWeekViewCshtml, timeSheetModel);
        }

        [HttpGet]
        [Route(UrlConstant.AddWeekView)]
        public ActionResult AddWeekView(int id)
        {
            var timeSheetModel = new TimeSheetViewModel
            {
                NumberOfTimeSheet = 1,
                WeekTimeSheetIndex = id,
                WorkItemList =
                    MemoryCacheHelper.GetInMemoryCache<List<HiDevOpsWorkItem>>(AppConstants.WorkItemList, _memCache)
            };
            return PartialView(UrlConstant.TimeSheetWeekViewCshtml, timeSheetModel);
        }

        [HttpGet]
        [Route(UrlConstant.GetTaskList)]
        public List<HiDevOpsWorkItem> GetTaskList(string projectName)
        {
            var workItemList =
                MemoryCacheHelper.GetInMemoryCache<List<HiDevOpsWorkItem>>(AppConstants.WorkItemList, _memCache);
            return workItemList.FindAll(project => project.Project == projectName);
        }


        [HttpPost]
        [Route(UrlConstant.SaveWeeKTimeWorkItems)]
        public WebClientResponse SaveWeeKTimeWorkItems([FromBody] List<WeekTimeInfoModel> timeSheetList)
        {
            var userInfo = MemoryCacheHelper.GetInMemoryCache<UserViewModel>(AppConstants.SessionUser, _memCache);

            if(timeSheetList == null) return new WebClientResponse {ErrorId = 1,ErrorDescription = "Error Occurred during save. Please try again."};
            foreach (var item in timeSheetList)  
            {
                if (item.ParentLink.IsNullOrEmpty()) continue;
                var tokenModel = MemoryCacheHelper.GetInMemoryCache<TokenModel>(AppConstants.TokenInfo, _memCache);
                var parentId = int.Parse(item.ParentLink.Split('/').Last());
                _iDevOpsRequestBrokerService.GetParentWorkItemById(parentId, tokenModel.AccessToken, item);
            }

            if (userInfo != null)
                timeSheetList.Where(item => item.UserId.IsNullOrEmpty()).ToList()
                    .ForEach(cc => cc.UserId = userInfo.User.UserIdentityId);
            return _iRequestBrokerService.PostRequest<ErrorViewModel>("/HI.DevOps.TimeSheet.Api/SaveWeekTimeSheet",
                timeSheetList);
          


        }

        #region Private Variable

        private static ILog _webLog;
        private readonly IMemoryCache _memCache;
        private readonly IRequestBrokerService _iRequestBrokerService;
        private readonly IDevOpsRequestBroker _iDevOpsRequestBrokerService;

        #endregion

        #region Private Member

        #endregion
    }
}