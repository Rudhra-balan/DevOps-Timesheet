using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HI.DevOps.Web.Common.Helper.Builder
{
    public class LogicDropdownBuilder
    {
        public static List<SelectListItem> GetList()
        {
            var dropdownList = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "AND",
                    Value =" and ",
                    Selected = true
                },
                new SelectListItem
                {
                    Text = "OR",
                    Value = " or ",
                    Selected = false
                }
            };



            return dropdownList;
        }
    }
}
