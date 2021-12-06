namespace HI.DevOps.DomainCore.Models.DevOps
{
    public class HiDevOpsWorkItem
    {
        public string Epic { get; set; }
        public string Feature { get; set; }
        public string UserStory { get; set; }
        public string Requirement { get; set; }
        public string Task { get; set; }
        public string Project { get; set; }
        public string ParentLink { get; set; }
        public int OriginalEstimate { get; set; }
        public int RemainingWork { get; set; }
        public int CompletedWork { get; set; }
        public int Effort { get; set; }
    }

}