
using System;

namespace Hi.DevOps.TimeSheet.API.DataObject.TimeSheet
{
    public class WeekTimeSheetDO
    {
        public string UserID { get; set; }
        public string Project { get; set; }
        public string Epic { get; set; }
        public string Feature { get; set; }
        public string UserStory { get; set; }
        public string Requirement { get; set; }
        public DateTime WeekStartDate { get; set; }
        public DateTime WeekEndDate { get; set; }

        public string Task { get; set; }
        public int? Sunday { get; set; }
        public int? Monday { get; set; }
        public int? Tuesday { get; set; }
        public int? Wednesday { get; set; }
        public int? Thursday { get; set; }
        public int? Friday { get; set; }
        public int? Saturday { get; set; }
        public int? Total { get; set; }
    }
}
