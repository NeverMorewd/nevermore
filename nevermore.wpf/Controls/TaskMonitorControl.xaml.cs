using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace nevermore.wpf.Controls
{
    /// <summary>
    /// TaskMonitorControl.xaml 的交互逻辑
    /// </summary>
    public partial class TaskMonitorControl : ItemsControl
    {
        public TaskMonitorControl()
        {
            InitializeComponent();
        }
    }
    public class TaskItem : INotifyPropertyChanged
    {
        private float taskProgressRatio;
        private TaskStatusEnum taskStatus = TaskStatusEnum.Ready;
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
        public event PropertyChangedEventHandler PropertyChanged;    
        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public enum TaskStatusEnum
    {
        Ready = 1,
        InProgress = 2,
        Completed = 3,
        Error = 4,
        Cancel = 5,
    }
}
