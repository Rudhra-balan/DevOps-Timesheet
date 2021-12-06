using System.Collections.Generic;
using HI.DevOps.DomainCore.Models.Error;

namespace HI.DevOps.DomainCore.Models
{
    public class BaseViewModel
    {
        public BaseViewModel()
        {
            ErrorList = new List<ErrorViewModel>();
            Error = new ErrorViewModel();
        }

        public string Message { get; set; }

        public List<ValidationErrorViewModel> ValidationErrors { get; set; }

        public List<ErrorViewModel> ErrorList { get; set; }

        public ErrorViewModel Error { get; set; }
    }
}