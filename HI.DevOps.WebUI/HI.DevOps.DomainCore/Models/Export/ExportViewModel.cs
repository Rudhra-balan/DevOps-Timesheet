using System;

namespace HI.DevOps.DomainCore.Models.Export
{
   public class ExportViewModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public int Department { get; set; }
        public DateTime Date { get; set; }
        public string Project { get; set; }
        public string Epic { get; set; }
        public string Feature { get; set; }
        public string UserStory { get; set; }
        public string Requirement { get; set; }
        public string Task { get; set; }
        public int Week1 { get; set; }
        public int Week2 { get; set; }
        public int Week3 { get; set; }
        public int Week4 { get; set; }
        public int Week5 { get; set; }
        public int Total { get; set; }
    }
}
