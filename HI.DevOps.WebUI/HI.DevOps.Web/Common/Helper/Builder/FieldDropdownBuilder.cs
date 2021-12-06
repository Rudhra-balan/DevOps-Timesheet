using System.Collections.Generic;
using HI.DevOps.DomainCore.Enumeration.Export;
using HI.DevOps.DomainCore.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HI.DevOps.Web.Common.Helper.Builder
{
    public class FieldDropdownBuilder
    {
        public static List<SelectListItem> GetList()
        {
            var dropdownList = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "--Select--",
                    Value =string.Empty,
                    Selected = false
                },
                new SelectListItem
                {
                    Text = ExportDatabaseFieldEnum.StartDate.ToString(),
                    Value = ExportDatabaseFieldEnum.StartDate.GetDescription(),
                    Selected = false
                },
                new SelectListItem
                {
                    Text = ExportDatabaseFieldEnum.EndDate.ToString(),
                    Value = ExportDatabaseFieldEnum.EndDate.GetDescription(),
                    Selected = false
                },
                new SelectListItem
                {
                    Text = ExportDatabaseFieldEnum.Department.ToString(),
                    Value = ExportDatabaseFieldEnum.Department.GetDescription(),
                    Selected = false
                },
                new SelectListItem
                {
                    Text = ExportDatabaseFieldEnum.User.ToString(),
                    Value = ExportDatabaseFieldEnum.User.GetDescription(),
                    Selected = false
                },
            };

            

            return dropdownList;
        }
    }
}