using nevermore.ui.wpfcontrols;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace nevermore.ui.windows
{
    public interface ITaskItemContext:INotifyPropertyChanged
    {
         string TaskName { get; set; }
         int TaskId { get; set; }
         float TaskProgressRatio
        {
            get;set;
        }
        TaskStatusEnum TaskStatus
        {
            get;set;
        }
        Task TaskInstance { get; set; }
        string TaskMessage
        {
            get; set;
        }
        IProgress<float> Progress
        {
            get;
            set;
        }
         string FilePath { get; set; }
         string FileLength { get; set; }
         CancellationTokenSource TaskCancellationTokenSource { get; set; }
    }
}
