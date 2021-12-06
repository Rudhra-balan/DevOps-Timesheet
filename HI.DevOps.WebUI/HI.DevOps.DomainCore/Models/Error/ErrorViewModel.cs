using HI.DevOps.DomainCore.Enumeration.ErrorEn;

namespace HI.DevOps.DomainCore.Models.Error
{
    public class ErrorViewModel
    {
        public int Id { get; set; }

        public string Message { get; set; }

        public ErrorTypeEnum Type { get; set; }

        public string ResponseXml { get; set; }

        public string ResponseValue { get; set; }
    }
}