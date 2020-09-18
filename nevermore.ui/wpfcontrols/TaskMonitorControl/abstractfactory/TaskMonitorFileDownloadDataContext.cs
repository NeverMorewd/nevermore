using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace nevermore.ui.wpfcontrols
{
    public class TaskMonitorFileDownloadDataContext : ITaskMonitorDataContext
    {
        public Action<ITaskItemContext> OnRetryTask => throw new NotImplementedException();

        public Action<ITaskItemContext> OnCancelTask => throw new NotImplementedException();

        public ObservableCollection<ITaskItemContext> TaskCollection { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Visibility NullTaskNoteVisibility { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void RunTaskMonitor(ITaskMonitorContext aTaskMonitorContext, IEnumerable<ITaskItemContext> aTaskItem, Func<ITaskItemContext, object[], Task> aTaskExcuteDelegate, params object[] optionalParams)
        {
            throw new NotImplementedException();
        }
    }
}
