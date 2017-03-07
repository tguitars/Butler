namespace Butler
{
    public class QueryInfoModel
    {
        public QueryInfoModel()
        {
        }

        public QueryInfoModel(QueryGridItemViewModel qgivm)
        {
            CanDelete = qgivm.CanDelete;
            Name = qgivm.Name;
            Content = qgivm.Content;
        }

        public string Name { get; set; }
        public string Content { get; set; }
        public bool CanDelete { get; set; }

        public override bool Equals(object obj)
        {
            var qi = obj as QueryInfoModel;
            return qi != null && qi.Name.Equals(Name);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}