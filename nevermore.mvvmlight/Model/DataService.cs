using System;

namespace nevermore.mvvmlight.Model
{
    public class DataService : IDataService
    {
        public void GetData(Action<DataItem, Exception> callback)
        {
            // Use this to connect to the actual data service

            var item = new DataItem("Welcome to MVVM Light","aaa","aaa");
            callback(item, null);
        }
    }
}