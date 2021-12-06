using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HI.DevOps.Web.Common.Helper.Builder
{
    public class UserDropdownBuilder
    {
        public static List<SelectListItem> GetList( List<string> userList)
        {
            var dropdownList = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "-- Select User--",
                    Value = "",
                    Selected = true
                }
            };
            dropdownList.AddRange(userList.Select(usr => new SelectListItem() {Text = usr, Value = usr}));

            return dropdownList;
        }
    }
}