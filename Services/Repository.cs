using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Butler
{
    public sealed class Repository
    {
        private static readonly Lazy<Repository> Lazy = new Lazy<Repository>(() => new Repository());
        private readonly object _lockobj = new object();
        private readonly Dictionary<string, IEnumerable<WorkItemModel>> _workItemStore;

        private Repository()
        {
            TfsProxy.Connect();

            _workItemStore = new Dictionary<string, IEnumerable<WorkItemModel>>();
            WorkItemQueries = Enumerable.Empty<QueryTreeItemViewModel>();
        }

        public IEnumerable<QueryTreeItemViewModel> WorkItemQueries { get; private set; }

        public static Repository Instance
        {
            get { return Lazy.Value; }
        }

        public IEnumerable<WorkItemModel> GetWorkItemModels(string query)
        {
            IEnumerable<WorkItemModel> workitems;
            lock (_lockobj)
            {
                if (!_workItemStore.TryGetValue(query, out workitems))
                {
                    workitems = Enumerable.Empty<WorkItemModel>();
                }
#if DEBUG
                else
                {
                    Console.WriteLine("GetWorkItemModels:" + query);
                }
#endif
            }
            return workitems;
        }

        public int GetWorkItemCountInFloating(int index)
        {
            if (index < 0 || index > 2)
            {
                throw new ArgumentException("index");
            }

            var top3 = QueryProvider.Instance.Data.Take(3).ToArray();
            return GetWorkItemModels(top3[index].Content).Count();
        }

        public event EventHandler BeforeLoadWorkItems;

        public event EventHandler AfterLoadWorkItems;

        public event EventHandler AfterLoadWorkItems2;

        public event EventHandler AfterLoadQueryHierarchy;


        private void OnBeforeLoadWorkItems(EventArgs e)
        {
            var handler = BeforeLoadWorkItems;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void OnAfterLoadWorkItems(EventArgs e)
        {
            var handler = AfterLoadWorkItems;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void OnAfterLoadWorkItems2(EventArgs e)
        {
            var handler = AfterLoadWorkItems2;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void OnAfterLoadQueryHierarchy(EventArgs e)
        {
            var handler = AfterLoadQueryHierarchy;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        public static void OpenWorkItem(int id)
        {
            var wi = TfsProxy.GetWorkItem(id);
            if (wi == null) return;
            var wim = new WorkItemModel(wi.Id, wi.Title);
            OpenWorkItem(wim.WebUrl);
        }

        public static void OpenWorkItem(string url)
        {
            TfsProxy.OpenWorkItem(url);
        }

        public void LoadQueryHierarchy()
        {
            var queryHierarchy = TfsProxy.GetQueryHierarchy();
            Action<QueryFolder, QueryTreeItemViewModel> buildQueryFolder = null;
            Action<QueryDefinition, QueryTreeItemViewModel> buildQueryDefinition = null;

            buildQueryFolder = (query, parent) =>
            {
                foreach (var subquery in query)
                {
                    if (subquery is QueryFolder)
                    {
                        var node = new QueryTreeItemViewModel
                        {
                            Name = subquery.Name
                        };
                        parent.Children.Add(node);

                        buildQueryFolder((QueryFolder)subquery, node);
                    }
                    else if (subquery is QueryDefinition)
                    {
                        buildQueryDefinition((QueryDefinition)subquery, parent);
                    }
                }
            };

            buildQueryDefinition = (query, parent) =>
            {
                parent.Children.Add(new QueryTreeItemViewModel
                {
                    Name = query.Name,
                    Content = query.QueryText,
                    Children = null
                });
            };


            var tree = new List<QueryTreeItemViewModel>();
            var root = new QueryTreeItemViewModel
            {
                Name = queryHierarchy.Name
            };

            buildQueryFolder(queryHierarchy, root);
            tree.Add(root);

            WorkItemQueries = tree;
            //GC.Collect(); //todo

        }

        public void Refresh()
        {

#if DEBUG
            var sw = System.Diagnostics.Stopwatch.StartNew();
#endif
            OnBeforeLoadWorkItems(EventArgs.Empty);
            lock (_lockobj)
            {
                _workItemStore.Clear();
                QueryProvider.Instance.Data.ForEach(
                    queryInfo =>
                    {
                        var tfsworkitems = TfsProxy.GetWorkItems(queryInfo.Content).ToArray();
                        var workitems = new List<WorkItemModel>();
                        if (queryInfo.Name.Contains("Code Reviews"))
                        {
                            var tfscodeReviewRequests = tfsworkitems.Where(wi => wi.Type.Name == "Code Review Request");
                            var tfsCodeReviewResponses = tfsworkitems.Where(wi => wi.Type.Name == "Code Review Response");

                            foreach (var request in tfscodeReviewRequests)
                            {
                                workitems.Add(new CodeReviewRequestModel(request.Id, request.Title,
                                    request.GetSheleveSetName(), request.GetOwnerAlias()));
                            }

                            foreach (var response in tfsCodeReviewResponses)
                            {
                                var request = response.GetCodeReviewRequestsByResponse();
                                workitems.Add(new CodeReviewResponseModel(response.Id, response.Title,
                                    new CodeReviewRequestModel(request.Id, request.Title,
                                        request.GetSheleveSetName(), request.GetOwnerAlias())));
                            }
                        }
                        else
                        {
                            foreach (var wi in tfsworkitems)
                            {
                                workitems.Add(new WorkItemModel(wi.Id, wi.Title));
                            }
                        }

                        _workItemStore.Add(queryInfo.Content, workitems);
                    });


                //foreach (var queryInfo in QueryProvider.Instance.Data)
                //{
                //    var tfsworkitems = TfsProxy.GetWorkItems(queryInfo.Content).ToArray();

                //    IEnumerable<WorkItemModel> workitems;
                //    if (queryInfo.Name.Contains("Code Reviews"))
                //    {
                //        var tfscodeReviewRequests = tfsworkitems.Where(wi => wi.Type.Name == "Code Review Request");
                //        var tfsCodeReviewResponses = tfsworkitems.Where(wi => wi.Type.Name == "Code Review Response");
                //        workitems = (from request in tfscodeReviewRequests
                //                     select
                //                         new CodeReviewRequestModel(request.Id, request.Title, request.GetSheleveSetName(),
                //                             request.GetOwnerAlias()))
                //            .Union<WorkItemModel>
                //            (from response in tfsCodeReviewResponses
                //             let request = response.GetCodeReviewRequestsByResponse()
                //             select
                //                 new CodeReviewResponseModel(response.Id, response.Title,
                //                     new CodeReviewRequestModel(request.Id, request.Title,
                //                         request.GetSheleveSetName(),
                //                         request.GetOwnerAlias())));
                //    }
                //    else
                //    {
                //        workitems = from wi in tfsworkitems select new WorkItemModel(wi.Id, wi.Title);
                //    }

                //    _workItemStore.Add(queryInfo.Content, workitems);
                //}

            }
            OnAfterLoadWorkItems(EventArgs.Empty);
            OnAfterLoadWorkItems2(EventArgs.Empty);
            //GC.Collect(); //todo

#if DEBUG
            Console.WriteLine(@"WorkingSet64:" + System.Diagnostics.Process.GetCurrentProcess().WorkingSet64);
            sw.Stop();

            Console.WriteLine("Refresh: {0}", sw.Elapsed.TotalMilliseconds);
#endif

        }
    }
}