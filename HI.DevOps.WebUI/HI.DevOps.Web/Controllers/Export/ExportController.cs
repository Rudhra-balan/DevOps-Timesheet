using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using HI.DevOps.Application.Common.Interfaces.IDevOpsRequestBroker;
using HI.DevOps.Application.Common.Interfaces.IRequestBroker;
using HI.DevOps.DomainCore.Helper.Constant;
using HI.DevOps.DomainCore.Models.Export;
using HI.DevOps.Web.Controllers.TimeSheet;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.IO;
using System.Linq;
using System.Text;
using HI.DevOps.DomainCore.Extensions;
using HI.DevOps.DomainCore.Models.Response;
using HI.DevOps.Web.Common;
using Microsoft.EntityFrameworkCore.Internal;

namespace HI.DevOps.Web.Controllers.Export
{
    [Authorize]
    [Route("Export")]
    public class ExportController : Controller
    {
        #region Private Variable

        private static ILog _webLog;
        public IConfiguration Configuration { get; }
        private readonly IMemoryCache _memCache;
        private readonly IRequestBrokerService _iRequestBrokerService;
        private readonly IDevOpsRequestBroker _iDevOpsRequestBrokerService;

        #endregion
        public ExportController(IConfiguration iConfiguration, IMemoryCache memoryCache,
            IRequestBrokerService requestBroker, IDevOpsRequestBroker devOpsRequestBroker)
        {
            _iRequestBrokerService = requestBroker;
            Configuration = iConfiguration;
            _memCache = memoryCache;
            _iDevOpsRequestBrokerService = devOpsRequestBroker;
            _webLog = LogManager.GetLogger(typeof(TimeSheetMonthEntryController));
        }
        [HttpGet]
        [Route(UrlConstant.ExportView)]
        public IActionResult Export()
        {
          var userEmail =  _iRequestBrokerService.SendRequest<List<string>>("/HI.DevOps.Export.Api/GetAllUser", false);
          MemoryCacheHelper.SetInMemoryCache(AppConstants.UserList,userEmail, _memCache);
              var exportQueryViewModel = new ExportQueryViewModel
            {
                NumberOfControl = 2,
                ControlIndex = 1,
                UserList = userEmail
            };
            return View(UrlConstant.ExportViewCshtml, exportQueryViewModel);
        }

        [HttpGet]
        [Route(UrlConstant.AddQueryControl)]
        public ActionResult AddQueryControl(int id)
        {
            var exportQueryViewModel = new ExportQueryViewModel
            {
                NumberOfControl = 1,
                ControlIndex = id,
                UserList = MemoryCacheHelper.GetInMemoryCache<List<string>>(AppConstants.UserList,  _memCache)
        };
            return View(UrlConstant.QueryViewCshtml, exportQueryViewModel);
        }

        [HttpGet]
        [Route(UrlConstant.ExportTimeSheet)]
        public IActionResult ExportTimeSheet()
        {
            var queryViewModels =
                JsonConvert.DeserializeObject<List<ExportRequestViewModel>>(HttpContext.Request.Query["Data"].ToString());
            var queryBuilder = GetExportQueryFromUserInput(queryViewModels);

            const string url = "/HI.DevOps.Export.Api/ExportTimeSheet";
            var timeSheetList = _iRequestBrokerService.PostRequest<List<ExportViewModel>>(url, queryBuilder.ToString());
            var excel = FormatExcelFile(timeSheetList);
            var stream = new MemoryStream(excel.GetAsByteArray());
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Timesheet Report {DateTime.Now.ToString(CultureInfo.InvariantCulture)}");

      }

        private static StringBuilder GetExportQueryFromUserInput(List<ExportRequestViewModel> queryViewModels)
        {
            
            var queryBuilder = new StringBuilder();
            var index = 0;
            var betweenGroupAndNonGroupOperator = string.Empty;
          
            var groupModel = queryViewModels.FindAll(query => query.IsGroupSelected);
            if (groupModel.Any())
            {
                queryBuilder.Append(" (");

                foreach (var query in groupModel)
                {
                    index += 1;
                    if (index == 1)
                        betweenGroupAndNonGroupOperator = query.LogicOperator.IsAnyNullOrEmpty()
                            ? string.Empty
                            : query.LogicOperator;
                    var logicOperator = index == 1 ? string.Empty : query.LogicOperator;
                    if (query.Operator.ToLower().Contains("in"))
                    {
                        queryBuilder.Append(
                            $" {logicOperator} {query.Field} {query.Operator} ( {query.FieldValue} ) ");
                        continue;
                    }

                    if (query.Operator.ToLower().Contains("like"))
                    {
                        queryBuilder.Append(
                            $" {logicOperator} {query.Field} {query.Operator} '%{query.FieldValue}%' ");
                        continue;
                    }


                    queryBuilder.Append($" {logicOperator} {query.Field} {query.Operator} '{query.FieldValue}' ");
                }

                queryBuilder.Append(") ");
            }

            var nonGroupModel = queryViewModels.FindAll(query => query.IsGroupSelected == false);
            if (!nonGroupModel.Any()) return queryBuilder;

            if (groupModel.Any())
            {
                var nonGroupQueryOperator = betweenGroupAndNonGroupOperator.IsAnyNullOrEmpty()
                    ? " and "
                    : betweenGroupAndNonGroupOperator;
                queryBuilder.Append(nonGroupQueryOperator);
            }

            foreach (var query in nonGroupModel)
            {
                index += 1;
                var logicOperator = index == 1 ? string.Empty : query.LogicOperator;
                if (query.Operator.ToLower().Contains("in"))
                {
                    queryBuilder.Append($" {logicOperator} {query.Field} {query.Operator} ( {query.FieldValue} )");
                    continue;
                }

                if (query.Operator.ToLower().Contains("like"))
                {
                    queryBuilder.Append($" {logicOperator} {query.Field} {query.Operator} '%{query.FieldValue}%' ");
                    continue;
                }

                queryBuilder.Append($" {logicOperator} {query.Field} {query.Operator} '{query.FieldValue}' ");
            }


            return queryBuilder;
        }
        private static ExcelPackage FormatExcelFile(WebClientResponse timeSheetList)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
            workSheet.TabColor = Color.Black;
            workSheet.DefaultRowHeight = 12;
            //Header of table  
            //  
            Color colFromHex = ColorTranslator.FromHtml("#6699ff");
            workSheet.Row(1).Height = 20;
            workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(1).Style.Font.Bold = true;
            workSheet.Row(1).Style.Font.Color.SetColor(Color.White);
            workSheet.Row(1).Style.Fill.PatternType = ExcelFillStyle.Solid;
            workSheet.Row(1).Style.Fill.SetBackground(colFromHex);
            workSheet.Row(1).Style.Fill.SetBackground(colFromHex);
            workSheet.Cells[1, 1].Value = "UserName";
            workSheet.Cells[1, 2].Value = "Email Address";
            workSheet.Cells[1, 3].Value = "Project";
            workSheet.Cells[1, 4].Value = "Epic";
            workSheet.Cells[1, 5].Value = "Feature";
            workSheet.Cells[1, 6].Value = "UserStory";
            workSheet.Cells[1, 7].Value = "Requirement";
            workSheet.Cells[1, 8].Value = "Task ";
            workSheet.Cells[1, 9].Value = "Week 1";
            workSheet.Cells[1, 10].Value = "Week 2";
            workSheet.Cells[1, 11].Value = "Week 3";
            workSheet.Cells[1, 12].Value = "Week 4";
            workSheet.Cells[1, 13].Value = "Week 5";
            workSheet.Cells[1, 14].Value = "Total";
            //Body of table  
            //  
            var recordIndex = 2;
            
            foreach (var exportViewModel in (List<ExportViewModel>) timeSheetList.SourceObject)
            {

                workSheet.Cells[10, 1].Merge = true;


                workSheet.Cells[recordIndex, 1].Value = exportViewModel.Username;
                workSheet.Cells[recordIndex, 2].Value = exportViewModel.Email;
                workSheet.Cells[recordIndex, 3].Value = exportViewModel.Project;
                workSheet.Cells[recordIndex, 4].Value = exportViewModel.Epic;
                workSheet.Cells[recordIndex, 5].Value = exportViewModel.Feature;
                workSheet.Cells[recordIndex, 6].Value = exportViewModel.UserStory;
                workSheet.Cells[recordIndex, 7].Value = exportViewModel.Requirement;
                workSheet.Cells[recordIndex, 8].Value = exportViewModel.Task;
                workSheet.Cells[recordIndex, 9].Value = exportViewModel.Week1;
                workSheet.Cells[recordIndex, 10].Value = exportViewModel.Week2;
                workSheet.Cells[recordIndex, 11].Value = exportViewModel.Week3;
                workSheet.Cells[recordIndex, 12].Value = exportViewModel.Week4;
                workSheet.Cells[recordIndex, 13].Value = exportViewModel.Week5;
                workSheet.Cells[recordIndex, 14].Value = exportViewModel.Total;

                recordIndex++;
            }

            workSheet.Column(1).AutoFit();
            workSheet.Column(2).AutoFit();
            workSheet.Column(3).AutoFit();
            workSheet.Column(4).AutoFit();
            workSheet.Column(5).AutoFit();
            workSheet.Column(6).AutoFit();
            workSheet.Column(7).AutoFit();
            workSheet.Column(8).AutoFit();
            workSheet.Column(9).AutoFit();
            workSheet.Column(10).AutoFit();
            workSheet.Column(11).AutoFit();
            workSheet.Column(12).AutoFit();
            workSheet.Column(13).AutoFit();
            workSheet.Column(14).AutoFit();
            return excel;
        }
    }
}