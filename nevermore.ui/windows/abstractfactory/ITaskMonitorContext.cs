using nevermore.ui.windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace nevermore.ui.wpfcontrols
{
    public interface ITaskMonitorContext
    {
        FactoryEnum FactoryType { get; set; }
        void OnWhenAllTaskComplete(bool isCompleteAll);
    }
    public interface ITaskMonitorWindowContext
    {
        ICommand CancelTaskCommand { get; set; }
        ICommand RetryTaskCommand { get; set; }
    }
    public enum TaskRunModelEnum
    {
        SINGLE = 0,
        PARALLEL = 1,
        GROUP = 2,
    }
    public class TaskItem : INotifyPropertyChanged
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
        }
        public string FilePath { get; set; }
        public string FileLength { get; set; }
        public object[] TaskDelegateParams { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public CancellationTokenSource TaskCancellationTokenSource { get; set; } = new CancellationTokenSource();
        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public TaskItem()
        {

        }
    }
    //public class TaskItemFileUpload : TaskItem
    //{
    //    public string FileUploadId { get; set; }
    //}
    public enum TaskStatusEnum
    {
        Ready = 1,
        InProgress = 2,
        Completed = 3,
        Error = 4,
        Cancel = 5,
        Hangup = 6,
        OutOfControl = 7,
        ErrorCanRetry = 8,
    }
    public enum FileTypeEnum
    {
        [Description(".doc")]
        DOC = 1,
        [Description(".docx")]
        DOCX,
        [Description(".pdf")]
        PDF = 2,
        [Description(".png")]
        PNG = 3,
        [Description(".jpg")]
        JPG = 4,
        [Description(".bmp")]
        BMP = 5,
        [Description(".gif")]
        GIF = 6,
        [Description(".jpeg")]
        JPEG = 7,
        [Description(".mp3")]
        MP3 = 8,
        [Description(".mp4")]
        MP4 = 9,
        OTHER = 0,
    }
    public delegate Task TaskExcuteDelegate(TaskItemFileUpload aTaskItem, CancellationToken cancellationToken,params object[] optionalParams);
    //public delegate Task TaskExcuteDelegate(TaskItem aTaskItem, CancellationToken cancellationToken, params object[] optionalParams);
    public class THCommand<T> : ICommand
    {
        private readonly Func<T, bool> _canExecute;
        private readonly Action<T> _execute;
        public THCommand(Action<T> execute)
            : this(execute, null)
        {
        }
        public THCommand(Action<T> execute, Func<T, bool> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }
            this._execute = new Action<T>(execute);
            if (canExecute != null)
            {
                this._canExecute = new Func<T, bool>(canExecute);
            }
        }
        public bool CanExecute(object parameter)
        {
            if (this._canExecute == null)
            {
                return true;
            }
            if ((parameter == null) && typeof(T).IsValueType)
            {
                return this._canExecute(default);
            }
            if ((parameter == null) || (parameter is T))
            {
                return this._canExecute((T)parameter);
            }
            return false;
        }
        public event EventHandler CanExecuteChanged;
        public void Execute(object parameter)
        {
            object obj2 = parameter;
            if (this.CanExecute(obj2) && (this._execute != null))
            {
                if (obj2 == null)
                {
                    if (typeof(T).IsValueType)
                    {
                        this._execute(default);
                    }
                    else
                    {
                        this._execute((T)obj2);
                    }
                }
                else
                {
                    this._execute((T)obj2);
                }
            }
        }
        public void RaiseCanExecuteChanged()
        {
            this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
