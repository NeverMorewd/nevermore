using nevermore.ui.wpfcontrols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nevermore.ui.windows
{
    public class TaskMonitorFileUploadFactory : TaskMonitorBaseFactory
    {
        public static readonly TaskMonitorFileUploadFactory TaskMonitorFileUploadFactoryInstance = new TaskMonitorFileUploadFactory();
        private TaskMonitorDataContext TaskMonitorDataContext = null;
        private TaskMonitorFileUploadFactory()
        { }
        public override ITaskMonitorDataContext GetTaskMonitorDataContext()
        {
            if (TaskMonitorDataContext == null)
            {
                TaskMonitorDataContext = new TaskMonitorDataContext();
            }
            return TaskMonitorDataContext;
        }
        public override void RunTaskMonitor(ITaskMonitorContext taskMonitorContext,IEnumerable<TaskItemFileUpload> aTaskItems, Func<TaskItemFileUpload, object[], Task> aTaskExecuteFunc, object[] aTaskActionParams)
        {
            TaskMonitorDataContext?.RunTaskMonitor(taskMonitorContext,aTaskItems, aTaskExecuteFunc, aTaskActionParams);
        }
    }
}
