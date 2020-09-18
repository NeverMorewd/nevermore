using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace nevermore.ui.wpfcontrols
{
    public class TaskMonitorFileUploadFactory : TaskMonitorBaseFactory
    {
        public static readonly TaskMonitorFileUploadFactory TaskMonitorFileUploadFactoryInstance = new TaskMonitorFileUploadFactory();
        private TaskMonitorFileUploadDataContext TaskMonitorDataContext = null;
        private TaskMonitorFileUploadFactory()
        {

        }
        public override ITaskMonitorDataContext GetTaskMonitorDataContext()
        {
            if (TaskMonitorDataContext == null)
            {
                TaskMonitorDataContext = new TaskMonitorFileUploadDataContext();
            }
            return TaskMonitorDataContext;
        }
        public override void RunTaskMonitor(ITaskMonitorContext taskMonitorContext,IEnumerable<ITaskItemContext> aTaskItems, Func<ITaskItemContext, object[], Task> aTaskExecuteFunc, object[] aTaskActionParams)
        {
            TaskMonitorDataContext?.RunTaskMonitor(taskMonitorContext,aTaskItems, aTaskExecuteFunc, aTaskActionParams);
        }
    }
}
