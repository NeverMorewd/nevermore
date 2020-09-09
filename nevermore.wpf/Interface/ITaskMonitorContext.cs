using nevermore.wpf.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace nevermore.wpf.Interface
{
    public interface ITaskMonitorContext : INotifyPropertyChanged
    {
        ICommand CancelTaskCommand { get; set; }
        ICommand RetryTaskCommand { get; set; }
        ObservableCollection<TaskItem<bool>> TaskCollection { get; set; }
        TaskExcuteDelegate TaskExcuteHandler { get; set; }
        int MaxTaskQuantity { get; set; }
    }
    public delegate Task<bool> TaskExcuteDelegate(TaskItem<bool> aTaskItem, CancellationToken cancellationToken, params object[] paras);
    public abstract class TaskMonitorContext : ITaskMonitorContext
    {
        public ICommand CancelTaskCommand { get; set; }
        public ICommand RetryTaskCommand { get; set; }
        public ObservableCollection<TaskItem<bool>> TaskCollection { get; set; }

        public TaskExcuteDelegate TaskExcuteHandler { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int MaxTaskQuantity { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
