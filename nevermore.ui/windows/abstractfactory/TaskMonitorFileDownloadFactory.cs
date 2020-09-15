using nevermore.ui.wpfcontrols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nevermore.ui.windows
{
    public class TaskMonitorFileDownloadFactory : TaskMonitorBaseFactory
    {
        public static readonly TaskMonitorFileDownloadFactory TaskMonitorFileDownloadFactoryInstance = new TaskMonitorFileDownloadFactory();
        private TaskMonitorFileDownloadFactory()
        { }
        public override ITaskMonitorDataContext GetTaskMonitorDataContext()
        {
            throw new NotImplementedException();
        }

        public override void RunTaskMonitor(ITaskMonitorContext taskMonitorContext, IEnumerable<TaskItemFileUpload> aTaskItems, Func<TaskItemFileUpload, object[],Task> aTaskExecuteFunc, object[] aTaskActionParams)
        {
            throw new NotImplementedException();
        }
    }
}
