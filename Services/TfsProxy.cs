using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Butler.Properties;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Butler
{
    public static class TfsProxy
    {
        private const string QueryWorkItemById =
            @"SELECT [System.Id] FROM WorkItems WHERE [System.TeamProject] = @Project AND [System.Id] = '{0}'";

        private const string QueryLinkItems =
            @"SELECT [System.Id], [System.Links.LinkType], [System.WorkItemType], [System.Title], [System.AssignedTo], [System.State], [System.Tags] FROM WorkItemLinks WHERE ([Source].[System.TeamProject] = @Project AND [Source].[System.WorkItemType] = 'Code Review Request') And ([System.Links.LinkType] = 'System.LinkTypes.Hierarchy-Forward') And ([Target].[System.WorkItemType] = 'Code Review Response'  AND  [Target].[System.Id] = {0}) ORDER BY [System.Id] mode(MustContain)";


        private static WorkItemStore _store;
        private static IIdentityManagementService _identityManagementService;
        private static readonly IDictionary Context;

        static TfsProxy()
        {
            Context = new Dictionary<string, string>
            {
                {"Project", Settings.Default.Project}
            };
        }

        public static void Connect()
        {
            var tfs = new TfsTeamProjectCollection(new Uri(Settings.Default.Serverurl));
            _store = tfs.GetService<WorkItemStore>();
            _identityManagementService = tfs.GetService<IIdentityManagementService>();
        }

        public static TeamProjectPicker GetTeamProjectPicker()
        {
            return new TeamProjectPicker(TeamProjectPickerMode.SingleProject, false);
        }

        public static IEnumerable<WorkItem> GetWorkItems(string query)
        {
            return new Query(_store, query, Context)
                .RunQuery()
                .OfType<WorkItem>();
        }

        public static WorkItem GetWorkItem(int id)
        {
            return new Query(_store, string.Format(QueryWorkItemById, id), Context)
                .RunQuery()
                .OfType<WorkItem>()
                .FirstOrDefault();
        }

        public static void OpenWorkItem(string url)
        {
            Process.Start(url);
        }


        public static QueryHierarchy GetQueryHierarchy()
        {
            return _store.Projects[Settings.Default.Project].QueryHierarchy;
        }

        public static string GetSheleveSetName(this WorkItem wi)
        {
            return !wi.Fields.Contains("Microsoft.VSTS.CodeReview.Context")
                ? null
                : wi.Fields["Microsoft.VSTS.CodeReview.Context"].Value.ToString();
        }

        public static string GetOwnerAlias(this WorkItem request)
        {
            var identity = _identityManagementService.ReadIdentity(IdentitySearchFactor.DisplayName,
                request.Fields["Created By"].Value.ToString(), MembershipQuery.None, ReadIdentityOptions.None);
            return identity.UniqueName;
        }

        public static WorkItem GetCodeReviewRequestsByResponse(this WorkItem request)
        {
            if (request.Type.Name != "Code Review Response") return null;
            var treeQuery = new Query(_store, string.Format(QueryLinkItems, request.Id), Context);
            var requestLink = treeQuery.RunLinkQuery().Where(l => l.TargetId != request.Id).FirstOrDefault();
            return GetWorkItem(requestLink.TargetId);
        }
    }
}