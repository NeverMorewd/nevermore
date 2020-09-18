using nevermore.ui.wpfcontrols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nevermore.ui.wpfcontrols
{
    public class TaskMonitorFileDownloadFactory : TaskMonitorBaseFactory
    {
        public static readonly TaskMonitorFileDownloadFactory TaskMonitorFileDownloadFactoryInstance = new TaskMonitorFileDownloadFactory();
        private TaskMonitorFileDownloadDataContext TaskMonitorDataContext = null;
        private TaskMonitorFileDownloadFactory()
        {
            
        }
        public override ITaskMonitorDataContext GetTaskMonitorDataContext()
        {
            if (TaskMonitorDataContext == null)
            {
                TaskMonitorDataContext = new TaskMonitorFileDownloadDataContext();
            }
            return TaskMonitorDataContext;
        }

        public override void RunTaskMonitor(ITaskMonitorContext taskMonitorContext, IEnumerable<ITaskItemContext> aTaskItems, Func<ITaskItemContext, object[],Task> aTaskExecuteFunc, object[] aTaskActionParams)
        {
            TaskMonitorDataContext?.RunTaskMonitor(taskMonitorContext, aTaskItems, aTaskExecuteFunc, aTaskActionParams);
        }
    }
}
