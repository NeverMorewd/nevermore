using System;
using nevermore.mvvmlight.Model;

namespace nevermore.mvvmlight.Design
{
    public class DesignDataService : IDataService
    {
        public void GetData(Action<DataItem, Exception> callback)
        {
            // Use this to create design time data

            var item = new DataItem("Welcome to MVVM Light [design]","ll","ll");
            callback(item, null);
        }
    }
}