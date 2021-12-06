using System;

namespace HI.DevOps.DomainCore.Models.TimeSheet
{
    public class TimeInfoModel
    {
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
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