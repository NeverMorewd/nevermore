namespace nevermore.mvvmlight.Model
{
    public class DataItem
    {
        public string Title
        {
            get;
            private set;
        }
        public string Path
        {
            get;
            private set;
        }
        public string Result
        {
            get;
            private set;
        }

        public DataItem(string title,string path,string result)
        {
            Title = title;
            Path = path;
            Result = result;
        }
    }
}