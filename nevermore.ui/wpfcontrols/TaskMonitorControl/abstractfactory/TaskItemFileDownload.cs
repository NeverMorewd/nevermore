using nevermore.ui.wpfcontrols;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace nevermore.ui.wpfcontrols
{
    public class TaskItemFileDownload : ITaskItemContext
    {
        public string TaskName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int TaskId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public float TaskProgressRatio { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public TaskStatusEnum TaskStatus { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Task TaskInstance { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string TaskMessage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IProgress<float> Progress { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string FilePath { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string FileLength { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public CancellationTokenSource TaskCancellationTokenSource { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
