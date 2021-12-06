using System;

namespace Hi.DevOps.TimeSheet.API.DataObject.TimeSheet
{
    public class WeekTimeInfoDO:BaseDo
    {
        public int TimeSheetId { get; set; }
        public string UserId { get; set; }
        public string Project { get; set; }
        public string Epic { get; set; }
        public string Feature { get; set; }
        public string UserStory { get; set; }
        public string Requirements { get; set; }
        public string Task { get; set; }
        public DateTime TimeSheetDate { get; set; }
        public int TimeSheetHours { get; set; }
    }
}
