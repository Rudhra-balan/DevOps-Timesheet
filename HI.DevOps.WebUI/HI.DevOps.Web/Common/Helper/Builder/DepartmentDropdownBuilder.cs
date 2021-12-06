using System.Collections.Generic;
using HI.DevOps.DomainCore.Enumeration.DepartmentEnum;
using HI.DevOps.DomainCore.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HI.DevOps.Web.Common.Helper.Builder
{
    public class DepartmentDropdownBuilder
    {
        public static List<SelectListItem> GetList()
        {
            var dropdownList = new List<SelectListItem>
            {
              
                new SelectListItem
                {
                    Text = DepartmentEnum.Ui.GetDescription(),
                    Value = DepartmentEnum.Ui.ToInt().ToString(),
                    Selected = false
                },
                new SelectListItem
                {
                    Text = DepartmentEnum.Firmware.GetDescription(),
                    Value = DepartmentEnum.Firmware.ToInt().ToString(),
                    Selected = false
                },
                new SelectListItem
                {
                    Text = DepartmentEnum.Mechanical.GetDescription(),
                    Value = DepartmentEnum.Mechanical.ToInt().ToString(),
                    Selected = false
                },

            };

            

            return dropdownList;
        }
    }
}