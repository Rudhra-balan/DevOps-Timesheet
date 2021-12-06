using System.Collections.Generic;
using HI.DevOps.DomainCore.Models.DevOps;

namespace HI.DevOps.DomainCore.Models.TimeSheet
{
    public class TimeSheetViewModel
    {
        public TimeSheetViewModel()
        {
            WorkItemList = new List<HiDevOpsWorkItem>();
            WeekTimeSheetList = new List<WeekTimeSheetModel>();
        }
        public int WeekTimeSheetIndex { get; set; }
        public int NumberOfTimeSheet { get; set; }
        public List<WeekTimeSheetModel> WeekTimeSheetList { get; set; }
        public List<HiDevOpsWorkItem> WorkItemList { get; set; }
    }
}