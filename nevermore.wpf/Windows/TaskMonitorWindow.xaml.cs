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

namespace nevermore.wpf.Windows
{
    /// <summary>
    /// TastMonitorWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TaskMonitorWindow : Window,ITaskMonitorContext
    {
        readonly TaskMonitorDataContext taskMonitorContext = null;

        public ICommand CancelTaskCommand { get; set; }
        public ICommand RetryTaskCommand { get; set; }

        public TaskMonitorWindow()
        {
            InitializeComponent();
            taskMonitorContext = new TaskMonitorDataContext(this);
            this.DataContext = taskMonitorContext;
            CancelTaskCommand = new THCommand<TaskItem>(taskMonitorContext.OnCancelTask);
            RetryTaskCommand = new THCommand<TaskItem>(taskMonitorContext.OnRetryTask);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
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
            //throw new NotImplementedException();
        }

        public void RunTaskMonitor(List<string> fileNames, TaskExcuteDelegate aTaskExcuteDelegate, params object[] optionalParams)
        {
            taskMonitorContext.RunTaskMonitor(fileNames, aTaskExcuteDelegate, optionalParams);
            //throw new NotImplementedException();
        }
    }
}
