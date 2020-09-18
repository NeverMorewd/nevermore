using nevermore.ui.windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace nevermore.ui.wpfcontrols
{
    public  class TaskMonitorFileUploadDataContext : INotifyPropertyChanged,ITaskMonitorDataContext
    {
        public ITaskMonitorContext taskMonitorContext;
        private bool isRunning = false;
        private Func<ITaskItemContext, object[],Task> TaskExcuteHandler;
        private object[] commonParams;
        private ObservableCollection<ITaskItemContext> taskCollection;
        public ObservableCollection<ITaskItemContext> TaskCollection
        {
            get
            {
                return taskCollection;
            }
            set
            {
                if (Equals(value, taskCollection)) return;
                taskCollection = value;
                OnPropertyChanged();
            }
        }
        private Visibility nullTaskNoteVisibility;
        public Visibility NullTaskNoteVisibility
        {
            get
            {
                return nullTaskNoteVisibility;
            }
            set
            {
                if (Equals(nullTaskNoteVisibility, value)) return;
                nullTaskNoteVisibility = value;
                OnPropertyChanged();
            }
        }
        public Action<ITaskItemContext> OnRetryTask { get { return OnRetryTaskExecute; } }
        public Action<ITaskItemContext> OnCancelTask { get { return OnCancelTaskExecute; } }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public TaskMonitorFileUploadDataContext()
        {
            NullTaskNoteVisibility = Visibility.Visible;
        }
        public void RunTaskMonitor(ITaskMonitorContext aTaskMonitorContext,IEnumerable<ITaskItemContext> aTaskItem, Func<ITaskItemContext, object[], Task> aTaskExcuteDelegate, params object[] optionalParams)
        {
            taskMonitorContext = aTaskMonitorContext;
            if (TaskCollection == null)
            {
                TaskCollection = new ObservableCollection<ITaskItemContext>();
            }
            TaskExcuteHandler = aTaskExcuteDelegate;
            aTaskItem.ToList().ForEach(x =>
            {
                x.TaskId = new Random().Next();
                x.TaskProgressRatio = 0;
                x.TaskStatus = TaskStatusEnum.Ready;
                x.TaskCancellationTokenSource = new CancellationTokenSource();
                if (new FileInfo(x.FilePath).Length > 104857600)
                {
                    x.TaskStatus = TaskStatusEnum.Error;
                    x.TaskMessage = "文件大小超过100M，暂不支持";
                }
                else if (new FileInfo(x.FilePath).Length >= 1048576)
                {
                    x.FileLength = System.Math.Ceiling(new FileInfo(x.FilePath).Length / 1048576.0) + "MB";
                }
                else if (new FileInfo(x.FilePath).Length == 0)
                {
                    x.TaskStatus = TaskStatusEnum.Error;
                    x.TaskMessage = "禁止上传0KB文件";
                }
                else
                {
                    x.FileLength = System.Math.Ceiling(new FileInfo(x.FilePath).Length / 1024.0) + "KB";
                }
                TaskCollection.Add((TaskItemFileUpload)x);
            });
            commonParams = optionalParams;
            RunTaskCollection();
        }
        private void OnRetryTaskExecute(ITaskItemContext obj)
        {
            obj.TaskStatus = TaskStatusEnum.Hangup;
            if (!obj.TaskCancellationTokenSource.IsCancellationRequested)
            {
                obj.TaskCancellationTokenSource.Cancel();
            }
            RunTaskCollection();
        }
        private void OnCancelTaskExecute(ITaskItemContext obj)
        {
            obj.TaskStatus = TaskStatusEnum.Cancel;
            if (!obj.TaskCancellationTokenSource.IsCancellationRequested)
            {
                obj.TaskCancellationTokenSource.Cancel();
            }
        }
        private void RunTaskCollection()
        {
            if (!isRunning)
            {
                TaskCollectionOnRun();
            }
        }
        private async void TaskCollectionOnRun()
        {
            isRunning = true;
            taskMonitorContext?.OnWhenAllTaskComplete(false);
            NullTaskNoteVisibility = Visibility.Collapsed;
            await Task.Run(async () =>
            {
                await Task.Delay(500);
                TaskCollection = new ObservableCollection<ITaskItemContext>(TaskCollection.Where(x => x.TaskStatus != TaskStatusEnum.Completed));
                if (TaskCollection.FirstOrDefault(x => x.TaskStatus == TaskStatusEnum.InProgress) != null)
                {
                    await Task.Delay(100);
                    TaskCollectionOnRun();
                }
                else
                {
                    var task = TaskCollection.FirstOrDefault(t => t.TaskStatus == TaskStatusEnum.Ready || t.TaskStatus == TaskStatusEnum.Hangup);
                    if (task != null)
                    {
                        if (task.TaskStatus == TaskStatusEnum.Hangup)
                        {
                            task.TaskCancellationTokenSource = new CancellationTokenSource();
                        }
                        task.TaskInstance = TaskExcuteHandler?.Invoke(task, commonParams);
                        await Task.Delay(100);
                        await task.TaskInstance.ContinueWith(_ => TaskCollectionOnRun());
                    }
                    else
                    {
                        isRunning = false;
                        if (TaskCollection.Count() == 0)
                        {
                            NullTaskNoteVisibility = Visibility.Visible;
                            taskMonitorContext?.OnWhenAllTaskComplete(true);
                        }
                    }
                }
            });
        }

    }
}
