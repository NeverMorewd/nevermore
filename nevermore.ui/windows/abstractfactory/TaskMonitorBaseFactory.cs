using nevermore.ui.wpfcontrols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace nevermore.ui.windows
{
    public abstract class TaskMonitorBaseFactory
    {
        public abstract ITaskMonitorDataContext GetTaskMonitorDataContext();
        public abstract void RunTaskMonitor(ITaskMonitorContext taskMonitorContext,IEnumerable<TaskItemFileUpload> aTaskItems, Func<TaskItemFileUpload, object[],Task> aTaskExecuteFunc, object[] aTaskActionParams);
    }
}
