using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using HI.DevOps.Application.Common.Exceptions;
using HI.DevOps.Application.Common.Interfaces.IDevOpsRequestBroker;
using HI.DevOps.DomainCore.Enumeration.ErrorEn;
using HI.DevOps.DomainCore.Extensions;
using HI.DevOps.DomainCore.Helper.Constant;
using HI.DevOps.DomainCore.Models.DevOps;
using HI.DevOps.DomainCore.Models.TimeSheet;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace HI.DevOps.Infrastructure.Helper
{
    public class DevOpsRequestBroker : IDevOpsRequestBroker
    {
        #region Constructor

        /// <summary>
        ///     Returns the Base portion of the url for API Services
        /// </summary>
        private readonly string _baseApiUrl;

        public DevOpsRequestBroker(string baseApiUrl)
        {
            _baseApiUrl = baseApiUrl;
        }

        #endregion

        #region Public Member

        /// <summary>
        ///     Get Current DevOps User Project Information
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public TeamProjectViewModel GetTeamProject(string accessToken)
        {
            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(
                        Encoding.ASCII.GetBytes($" :{accessToken}")));

                using var response = client.GetAsync($"{_baseApiUrl}/_apis/projects?api-version=5.1")
                    .Result;
                response.EnsureSuccessStatusCode();
                var responseBody = response.Content.ReadAsStringAsync().Result;
                return TeamProjectViewModel.FromJson(responseBody);
            }
            catch (AggregateException ae)
            {
                ae.Handle(exception =>
                {
                    if (exception is SocketException || exception is HttpRequestException)
                        throw new AppException(string.Format(AppConstants.GenericApiActionError,
                            ErrorEnum.ServiceUnavailable.GetDescription(), ErrorEnum.ServiceUnavailable.ToInt()));

                    throw new AppException(string.Format(AppConstants.GenericApiActionError,
                        ErrorEnum.UnknownApiError.GetDescription(), ErrorEnum.UnknownApiError.ToInt()));
                });

                throw new AppException(string.Format(AppConstants.GenericApiActionError,
                    ErrorEnum.UnknownApiError.GetDescription(), ErrorEnum.UnknownApiError.ToInt()));
            }
            catch (Exception ex)
            {
                if (ex is SocketException || ex is HttpRequestException)
                    throw new AppException(string.Format(AppConstants.GenericApiActionError,
                        ErrorEnum.ServiceUnavailable.GetDescription(), ErrorEnum.ServiceUnavailable.ToInt()));

                throw new AppException(string.Format(AppConstants.GenericApiActionError,
                    ErrorEnum.UnknownApiError.GetDescription(), ErrorEnum.UnknownApiError.ToInt()));
            }
        }

        /// <summary>
        ///     Get Current DevOps User Profile Information
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public DevOpsUserProfile GetDevOpsUserProfile(string accessToken)
        {
            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(
                        Encoding.ASCII.GetBytes($" :{accessToken}")));

                using var response = client
                    .GetAsync("https://app.vssps.visualstudio.com/_apis/profile/profiles/me?api-version=5.1")
                    .Result;
                response.EnsureSuccessStatusCode();
                var responseBody = response.Content.ReadAsStringAsync().Result;
                return DevOpsUserProfile.FromJson(responseBody);
            }
            catch (AggregateException ae)
            {
                ae.Handle(exception =>
                {
                    if (exception is SocketException || exception is HttpRequestException)
                        throw new AppException(string.Format(AppConstants.GenericApiActionError,
                            ErrorEnum.ServiceUnavailable.GetDescription(), ErrorEnum.ServiceUnavailable.ToInt()));

                    throw new AppException(string.Format(AppConstants.GenericApiActionError,
                        ErrorEnum.UnknownApiError.GetDescription(), ErrorEnum.UnknownApiError.ToInt()));
                });

                throw new AppException(string.Format(AppConstants.GenericApiActionError,
                    ErrorEnum.UnknownApiError.GetDescription(), ErrorEnum.UnknownApiError.ToInt()));
            }
            catch (Exception ex)
            {
                if (ex is SocketException || ex is HttpRequestException)
                    throw new AppException(string.Format(AppConstants.GenericApiActionError,
                        ErrorEnum.ServiceUnavailable.GetDescription(), ErrorEnum.ServiceUnavailable.ToInt()));

                throw new AppException(string.Format(AppConstants.GenericApiActionError,
                    ErrorEnum.UnknownApiError.GetDescription(), ErrorEnum.UnknownApiError.ToInt()));
            }
        }


        /// <summary>
        ///     Get Work Item
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public List<HiDevOpsWorkItem> GetWorkItem(string accessToken)
        {
            var hiDevOpsWorkItems = new List<HiDevOpsWorkItem>();
            try
            {
                var orgUrl = new Uri("https://azure.visualstudio.com/");

                // Create a connection
                var connection = new VssConnection(orgUrl,
                    new VssBasicCredential(string.Empty, accessToken));
                connection.ConnectAsync().Wait();

                var workItemQuery = new Wiql
                {
                    Query =
                        "select [System.Id], [System.WorkItemType], [System.Title], [System.AssignedTo], [System.State], [System.TeamProject],[Microsoft.VSTS.Scheduling.OriginalEstimate], [Microsoft.VSTS.Scheduling.RemainingWork] from WorkItemLinks where (Source.[System.WorkItemType] = 'Epic' or Source.[System.WorkItemType] = 'Requirement' or Source.[System.WorkItemType] = 'Feature') and ([System.Links.LinkType] = 'System.LinkTypes.Hierarchy-Forward') and (Target.[System.AssignedTo] = @me and Target.[System.ChangedDate] >= @today - 30) mode (Recursive, ReturnMatchingChildren)"
                };

                using var httpClient = connection.GetClient<WorkItemTrackingHttpClient>();
                // execute the query to get the list of work items in the results
                var result = httpClient.QueryByWiqlAsync(workItemQuery).Result;
                var ids = result.WorkItemRelations.Select(item => item.Target.Id).ToArray();

                // some error handling
                if (ids.Length == 0) return hiDevOpsWorkItems;

                // get work items for the ids found in query
                var workItemsList = httpClient.GetWorkItemsAsync(ids, null, null, WorkItemExpand.Relations).Result;

                workItemsList = workItemsList.Where(item =>
                    item.Fields["System.WorkItemType"].ToString() == "Bug" ||
                    item.Fields["System.WorkItemType"].ToString() == "Task").ToList();

                foreach (var item in workItemsList)
                    switch (item.Fields["System.WorkItemType"].ToString())
                    {
                        case "Bug":
                        case "Task":

                            var workItem = new HiDevOpsWorkItem
                            {
                                Task =
                                    $"{item.Fields["System.WorkItemType"]} {item.Id} {item.Fields["System.Title"]}",
                                Project = item.Fields["System.TeamProject"].ToString(),
                                ParentLink = item.Relations == null
                                    ? ""
                                    : item.Relations.First(itemRel =>
                                            itemRel.Rel == "System.LinkTypes.Hierarchy-Reverse")
                                        .Url.IsNullOrEmpty()
                                        ? ""
                                        : item.Relations.First(itemRel =>
                                            itemRel.Rel == "System.LinkTypes.Hierarchy-Reverse").Url,
                               

                            };

                            if (item.Fields.ContainsKey("Microsoft.VSTS.Scheduling.OriginalEstimate"))
                            {
                                workItem.OriginalEstimate =
                                    Convert.ToInt32(item.Fields["Microsoft.VSTS.Scheduling.OriginalEstimate"].ToString());
                            }

                            if (item.Fields.ContainsKey("Microsoft.VSTS.Scheduling.RemainingWork"))
                            {
                                workItem.RemainingWork =
                                   Convert.ToInt32(item.Fields["Microsoft.VSTS.Scheduling.RemainingWork"].ToString());
                            }
                            if (item.Fields.ContainsKey("Microsoft.VSTS.Scheduling.CompletedWork"))
                            {
                                workItem.CompletedWork =
                                    Convert.ToInt32(item.Fields["Microsoft.VSTS.Scheduling.CompletedWork"].ToString());
                            }
                            if (item.Fields.ContainsKey("Microsoft.VSTS.Scheduling.Effort"))
                            {
                                workItem.Effort =
                                    Convert.ToInt32(item.Fields["Microsoft.VSTS.Scheduling.Effort"].ToString());
                            }
                            

                           

                            hiDevOpsWorkItems.Add(workItem);
                            break;
                    }
            }
            catch
            {
                // ignored
            }

            return hiDevOpsWorkItems;
        }


        public void GetParentWorkItemById(int id, string accessToken, WeekTimeInfoModel hiDevOpsWorkItem)
        {
            var orgUrl = new Uri("https://azure.visualstudio.com/");

            // Create a connection
            var connection = new VssConnection(orgUrl,
                new VssBasicCredential(string.Empty, accessToken));
            connection.ConnectAsync().Wait();
            using var httpClient = connection.GetClient<WorkItemTrackingHttpClient>();
            var ids = new int[1];
            ids[0] = id;
            var parentInfo = httpClient.GetWorkItemsAsync(ids, null, null, WorkItemExpand.Relations).Result;

            switch (parentInfo[0].Fields["System.WorkItemType"].ToString())
            {
                case "Epic":
                    hiDevOpsWorkItem.Epic =
                        $"{parentInfo[0].Fields["System.WorkItemType"]} {parentInfo[0].Id} {parentInfo[0].Fields["System.Title"]}";

                    break;
                case "Feature":
                    hiDevOpsWorkItem.Feature =
                        $"{parentInfo[0].Fields["System.WorkItemType"]} {parentInfo[0].Id} {parentInfo[0].Fields["System.Title"]}";

                    break;
                case "User Story":
                    hiDevOpsWorkItem.UserStory =
                        $"{parentInfo[0].Fields["System.WorkItemType"]} {parentInfo[0].Id} {parentInfo[0].Fields["System.Title"]}";

                    break;
                case "Requirement":
                    hiDevOpsWorkItem.Requirements =
                        $"{parentInfo[0].Fields["System.WorkItemType"]} {parentInfo[0].Id} {parentInfo[0].Fields["System.Title"]}";
                    break;
            }

            GetParentFromWorkItem(httpClient, parentInfo[0], hiDevOpsWorkItem);
        }

        private static void GetParentFromWorkItem(WorkItemTrackingHttpClient witClient, WorkItem child,
            WeekTimeInfoModel hiDevOpsWorkItem)
        {
            var taskWorkItemRelations = child.Relations.Where(item =>
                item.Rel == "System.LinkTypes.Hierarchy-Reverse").ToArray();
            foreach (var workItemRelation in taskWorkItemRelations)
            {
                var ids = new int[1];
                ids[0] = int.Parse(workItemRelation.Url.Split('/').Last());
                var parentInfo = witClient.GetWorkItemsAsync(ids, null, null, WorkItemExpand.Relations).Result;
                switch (parentInfo[0].Fields["System.WorkItemType"].ToString())
                {
                    case "Epic":
                        hiDevOpsWorkItem.Epic =
                            $"{parentInfo[0].Fields["System.WorkItemType"]} {parentInfo[0].Id} {parentInfo[0].Fields["System.Title"]}";
                        if (parentInfo[0].Relations != null)
                            GetParentFromWorkItem(witClient, parentInfo[0], hiDevOpsWorkItem);
                        break;
                    case "Feature":
                        hiDevOpsWorkItem.Feature =
                            $"{parentInfo[0].Fields["System.WorkItemType"]} {parentInfo[0].Id} {parentInfo[0].Fields["System.Title"]}";
                        if (parentInfo[0].Relations != null)
                            GetParentFromWorkItem(witClient, parentInfo[0], hiDevOpsWorkItem);
                        break;
                    case "User Story":
                        hiDevOpsWorkItem.UserStory =
                            $"{parentInfo[0].Fields["System.WorkItemType"]} {parentInfo[0].Id} {parentInfo[0].Fields["System.Title"]}";
                        if (parentInfo[0].Relations != null)
                            GetParentFromWorkItem(witClient, parentInfo[0], hiDevOpsWorkItem);
                        break;
                    case "Requirement":
                        hiDevOpsWorkItem.Requirements =
                            $"{parentInfo[0].Fields["System.WorkItemType"]} {parentInfo[0].Id} {parentInfo[0].Fields["System.Title"]}";
                        if (parentInfo[0].Relations != null)
                            GetParentFromWorkItem(witClient, parentInfo[0], hiDevOpsWorkItem);
                        break;
                }
            }
        }

        #endregion
    }
}