using nevermore.ui.wpfcontrols;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace nevermore.ui.windows
{
    /// <summary>
    /// TastMonitorWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TaskMonitorWindow : Window, ITaskMonitorContext, ITaskMonitorWindowContext
    {
        private ITaskMonitorDataContext taskMonitorDataContext = null;
        private readonly ITaskMonitorContext taskMonitorContext = null;
        public ICommand CancelTaskCommand { get; set; }
        public ICommand RetryTaskCommand { get; set; }
        public FactoryEnum FactoryType { get; set; }

        public TaskMonitorWindow()
        {
            InitializeComponent();
            FactoryType = FactoryEnum.FileUploadFactory;
            taskMonitorContext = this;
            Initialize();
        }
        public TaskMonitorWindow(ITaskMonitorContext aTaskMonitorContext)
        {
            InitializeComponent();
            taskMonitorContext = aTaskMonitorContext;
            Initialize();

        }

        private void Initialize()
        {
            taskMonitorDataContext = TaskMonitorFactoryFacade.CreateFactory(taskMonitorContext.FactoryType).GetTaskMonitorDataContext();
            this.DataContext = taskMonitorDataContext;
            CancelTaskCommand = new THCommand<TaskItemFileUpload>(taskMonitorDataContext.OnCancelTask);
            RetryTaskCommand = new THCommand<TaskItemFileUpload>(taskMonitorDataContext.OnRetryTask);
        }
        public void Run(IEnumerable<ITaskItemContext> aTaskItems, Func<ITaskItemContext, object[], Task> aTaskExecuteAction, object[] aTaskActionParams)
        {
            TaskMonitorFactoryFacade.CreateFactory(taskMonitorContext.FactoryType).RunTaskMonitor(taskMonitorContext, aTaskItems, aTaskExecuteAction, aTaskActionParams);
        }
        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void Border_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        public void OnWhenAllTaskComplete(bool isCompleteAll)
        {
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
    }
}
