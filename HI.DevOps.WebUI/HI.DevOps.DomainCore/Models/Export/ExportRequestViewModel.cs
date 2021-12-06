using System.Collections.Generic;

namespace HI.DevOps.DomainCore.Models.Export
{
  
   public class ExportRequestViewModel
   {
       public string LogicOperator { get; set; }
       public bool IsGroupSelected { get; set; }
       public string Field { get; set; }
       public string Operator { get; set; }
       public string FieldValue { get; set; }

    }
}

