using Butler.Properties;

namespace Butler
{
    public class WorkItemModel
    {
        protected readonly string Project;
        protected readonly string ServerUrl;

        public WorkItemModel(int id, string title)
        {
            Id = id;
            Title = title;
            Project = Settings.Default.Project;
            ServerUrl = Settings.Default.Serverurl;
        }

        public int Id { get; private set; }
        public string Title { get; private set; }

        /// <summary>
        ///     WorkItem.Uri.AbsoluteUri, vstfs:///WorkItemTracking/WorkItem/3661089?url=http://tfsmr:8080/tfs/IEB
        /// </summary>
        public virtual string ArtifactUrl
        {
            get { return string.Format(@"vstfs:///WorkItemTracking/WorkItem/{0}?url={1}", Id, ServerUrl); }
        }

        /// <summary>
        ///     http://tfsmr:8080/tfs/web/wi.aspx?pcguid=28131a03-64c8-426c-b6d9-98b823fd81c4&id=3628566 =>
        ///     http://tfsmr:8080/tfs/IEB/ISS Media Platform/_workitems#_a=edit&id=3628566
        /// </summary>
        public virtual string WebUrl
        {
            get { return string.Format(@"{0}/{1}/_workitems#_a=edit&id={2}", ServerUrl, Project, Id); }
        }
    }

    public class CodeReviewRequestModel : WorkItemModel
    {
        private readonly string _ownerAlias;
        private readonly string _sheleveSet;

        public CodeReviewRequestModel(int id, string title, string sheleveSet, string ownerAlias)
            : base(id, title)
        {
            _sheleveSet = sheleveSet;
            _ownerAlias = ownerAlias;
        }

        /// <summary>
        ///     vstfs:///CodeReview/Request/3628566?url=http://tfsmr:8080/tfs/IEB
        /// </summary>
        public override string ArtifactUrl
        {
            get { return string.Format(@"vstfs:///CodeReview/Request/{0}?url={1}", Id, ServerUrl); }
        }

        /// <summary>
        ///     http://tfsmr:8080/tfs/web/ss.aspx?pcguid=28131a03-64c8-426c-b6d9-98b823fd81c4&ssname
        ///     =FixMediaListOrderInRightsFactory&ssowner=ERICSSON\ecinzou =>
        ///     http://tfsmr:8080/tfs/IEB/_versionControl/shelveset?ss=FixMediaListOrderInRightsFactory;ERICSSON\ecinzou
        /// </summary>
        public override string WebUrl
        {
            get
            {
                return string.Format(@"{0}/_versionControl/shelveset?ss={1};{2}", ServerUrl, _sheleveSet, _ownerAlias);
            }
        }
    }


    public class CodeReviewResponseModel : WorkItemModel
    {
        private readonly CodeReviewRequestModel _request;

        public CodeReviewResponseModel(int id, string title, CodeReviewRequestModel request)
            : base(id, title)
        {
            _request = request;
        }

        public override string ArtifactUrl
        {
            get { return _request.ArtifactUrl; }
        }

        public override string WebUrl
        {
            get { return _request.WebUrl; }
        }
    }
}