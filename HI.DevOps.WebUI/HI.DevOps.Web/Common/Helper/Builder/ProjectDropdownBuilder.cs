using System.Collections.Generic;
using System.Linq;
using HI.DevOps.DomainCore.Extensions;
using HI.DevOps.DomainCore.Models.DevOps;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HI.DevOps.Web.Common.Helper.Builder
{
    public class ProjectDropdownBuilder
    {
        public static List<SelectListItem> GetList(string selected, List<HiDevOpsWorkItem> devOpsWorkItem)
        {
            var dropdownList = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "-- Select Project--",
                    Value = "",
                    Selected = selected.IsNullOrEmpty()
                }
            };

            var projectList = devOpsWorkItem.GroupBy(project => project.Project).Where(group => group.Count() > 1)
                .Select(x => x.Key);

            dropdownList.AddRange(projectList.Select(selectedValue => new SelectListItem
            {
                Text = $"{selectedValue}", Value = $"{selectedValue}",
                Selected = !selected.IsNullOrEmpty() && selected.Trim() == selectedValue.Trim()
            }));


            return dropdownList;
        }
    }
}