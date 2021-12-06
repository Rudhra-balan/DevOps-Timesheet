using System.Collections.Generic;
using HI.DevOps.DomainCore.Models.DevOps;
using HI.DevOps.DomainCore.Models.TimeSheet;

namespace HI.DevOps.Application.Common.Interfaces.IDevOpsRequestBroker
{
    public interface IDevOpsRequestBroker
    {
        TeamProjectViewModel GetTeamProject(string accessToken);
        DevOpsUserProfile GetDevOpsUserProfile(string accessToken);
        List<HiDevOpsWorkItem> GetWorkItem(string accessToken);
        void GetParentWorkItemById(int id, string accessToken, WeekTimeInfoModel hiDevOpsWorkItem);
    }
}