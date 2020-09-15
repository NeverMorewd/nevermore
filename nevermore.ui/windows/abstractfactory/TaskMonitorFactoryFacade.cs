using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace nevermore.ui.windows
{
    public class TaskMonitorFactoryFacade
    {
        public static TaskMonitorBaseFactory CreateFactory(FactoryEnum factoryEnum)
        {
            switch (factoryEnum)
            {
                case FactoryEnum.FileDownloadFactory:
                    return TaskMonitorFileDownloadFactory.TaskMonitorFileDownloadFactoryInstance;
                case FactoryEnum.FileUploadFactory:
                    return TaskMonitorFileUploadFactory.TaskMonitorFileUploadFactoryInstance;
                default:
                    throw new NotImplementedException();
            }
        }
    }
    public enum FactoryEnum
    {
        FileUploadFactory = 1,
        FileDownloadFactory = 2,
    }
}
