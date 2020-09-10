using nevermore.wpf.Interface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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
    public partial class TaskMonitorControl : UserControl
    {
        public static readonly DependencyProperty TaskCollectionProperty = DependencyProperty.Register("TaskCollection", typeof(ObservableCollection<TaskItem<bool>>), typeof(TaskMonitorControl), new PropertyMetadata(default(ObservableCollection<TaskItem<bool>>)));
        public static readonly DependencyProperty TaskMonitorContextProperty = DependencyProperty.Register("TaskMonitorContext", typeof(ITaskMonitorContext), typeof(TaskMonitorControl), new PropertyMetadata(default(nevermore.wpf.Interface.TaskMonitorContext)),new ValidateValueCallback(IsValidValue));

        private static bool IsValidValue(object value)
        {
            return true;
        }

        public ObservableCollection<TaskItem<bool>> TaskCollection
        {
            get { return (ObservableCollection<TaskItem<bool>>)GetValue(TaskCollectionProperty); }
            set { SetValue(TaskCollectionProperty, value); }
        }
        public ITaskMonitorContext TaskMonitorContext
        {
            get { return (ITaskMonitorContext)GetValue(TaskMonitorContextProperty); }
            set { SetValue(TaskMonitorContextProperty, value); }
        }
        public TaskMonitorControl()
        {
            InitializeComponent();
            //TaskMonitorContextProperty.OverrideMetadata(typeof(TaskMonitorControl), new FrameworkPropertyMetadata(new PropertyChangedCallback(TaskMonitorContextPropertyChanged)));
            //this.DataContext = TaskMonitorContext;       
        }

        private void TaskMonitorContextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }
    }
    public class TaskItem<T> : INotifyPropertyChanged
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
        public Task<T> TaskInstance { get; set; }
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
    public enum TaskStatusEnum
    {
        Ready = 1,
        InProgress = 2,
        Completed = 3,
        Error = 4,
        Cancel = 5,
        Hangup = 6,
        OutOfControl = 7,
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
        MP4 =9,
        OTHER = 0,
    }
}
