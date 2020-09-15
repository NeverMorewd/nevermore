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
    public class TaskItemFileUpload : ITaskItemContext
    {
        private float taskProgressRatio;
        private TaskStatusEnum taskStatus = TaskStatusEnum.Ready;
        private string taskMessage;
        public string TaskName { get; set; }
        public int TaskId { get; set; } = new Random().Next();
        public float TaskProgressRatio
        {
            get
            {
                return taskProgressRatio;
            }
            set
            {
                if (Equals(taskProgressRatio, value)) return;
                taskProgressRatio = value;
                OnPropertyChanged();
            }
        }
        public TaskStatusEnum TaskStatus
        {
            get
            {
                return taskStatus;
            }
            set
            {
                if (Equals(value, taskStatus)) return;
                taskStatus = value;
                OnPropertyChanged();
            }
        }
        public FileTypeEnum FileType { get; set; }
        public Task TaskInstance { get; set; }
        public string TaskMessage
        {
            get
            {
                return taskMessage;
            }
            set
            {
                if (Equals(value, taskMessage)) return;
                taskMessage = value;
                OnPropertyChanged();
            }
        }
        public IProgress<float> Progress
        {
            get
            {
                return new Progress<float>(ratio =>
                {
                    TaskProgressRatio = ratio;
                });
            }
            set { }
        }
        public string FilePath { get; set; }
        public string FileLength { get; set; }
        public string FileUploadId { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public CancellationTokenSource TaskCancellationTokenSource { get; set; }
        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
