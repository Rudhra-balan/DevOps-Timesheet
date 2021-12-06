
using System.Collections.Generic;

namespace HI.DevOps.DomainCore.Models.Export
{
    public class ExportQueryViewModel
    {
        public ExportQueryViewModel()
        {
            UserList = new List<string>();
        }
        public List<string> UserList { get; set; }
        public int NumberOfControl { get; set; }
        public int ControlIndex { get; set; }
        public bool IsGroupField { get; set; }
        public string LogicOperator { get; set; }
        public string Field { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
    }
}
