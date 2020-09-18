using Microsoft.Win32;
using nevermore.common;
using nevermore.ui.windows;
using nevermore.ui.wpfcontrols;
using nevermore.wpf.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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

namespace nevermore.wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        private string pathText;
        private string resultText;
        private bool isTaskCompleted = true;
        TestWindow testWindow;
        private Window taskMonitorWindow;
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            //Window = new TestWindow();
        }
        public string PathText
        {
            get
            {
                return pathText;
            }
            set
            {
                pathText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PathText"));
            }
        }
        public string ResultText
        {
            get
            {
                return resultText;
            }
            set
            {
                resultText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ResultText"));
            }
        }
       public event PropertyChangedEventHandler PropertyChanged;

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(PathText))
            {
                ResultText = "OK!";
            }
            else
            {
                ResultText = "NOK!";
            }

            //new TestWindow().Show();
            FileInfo fileInfo = new FileInfo("hha");
            string re = System.Math.Ceiling(fileInfo.Length / 1024.0) + "KB";

            FileInfo fileInfoBig = new FileInfo("hha");
            string reBig = System.Math.Ceiling(fileInfoBig.Length / 1048576.0) + "MB";

        }

        private async void button_Copy_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void button_Click_1(object sender, RoutedEventArgs e)
        {
            new SecurityTestWindow().Show();
        }

        private async void button_Copy1_Click(object sender, RoutedEventArgs e)
        {
            await Pinger.TestPing("172.18.19.101");
        }

        private void button_FileSelect_Click(object sender, RoutedEventArgs e)
        {
            if (taskMonitorWindow == null)
            {
                taskMonitorWindow = new TaskMonitorWindow();
            }
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = @"C:\desktop",
                Filter = "所有文件(*.*)|*.*",
                Multiselect = true, //是否可以多选true=ok/false=no
                AddExtension = true,
                Title = "请选择要上传的材料，每次最多可选10个",
                CheckFileExists = true,
            };
            if (openFileDialog.ShowDialog() == true)
            {
                if (openFileDialog.FileNames.Length > 10)
                {
                    MessageBox.Show("每次至多选择10个文件，请重新选择！");
                    return;
                }
                List<TaskItemFileUpload> taskItems = new List<TaskItemFileUpload>();
                openFileDialog.FileNames.ToList().ForEach(f =>
                {
                    TaskItemFileUpload t = new TaskItemFileUpload
                    {
                        TaskId = new Random().Next(),
                        TaskName = System.IO.Path.GetFileName(f),
                        TaskProgressRatio = 0,
                        FilePath = f,
                        TaskStatus = TaskStatusEnum.Ready,
                        FileUploadId = new Random().Next(0, 999).ToString("D6"),
                    };
                    taskItems.Add(t);
                    Thread.Sleep(10);
                });
                taskMonitorWindow.Show();
                ((TaskMonitorWindow)taskMonitorWindow).Run(taskItems, UpLoadFileAsync,null);
            }
        }

        private void button_Monitor_Click(object sender, RoutedEventArgs e)
        {
            if (taskMonitorWindow == null) taskMonitorWindow = new TaskMonitorWindow();
            if (!taskMonitorWindow.IsActive)
            {
                taskMonitorWindow.Show();
            }
            //new TastMonitorWindow().Show();
        }
        private async Task UpLoadFileAsync(ITaskItemContext aTaskItem,object[] paramArray)
        {
            try
            {
                await Task.Run(async () =>
                {
                    int percentComplete = 0;
                    aTaskItem.TaskMessage = string.Empty;
                    while (true)
                    {
                        aTaskItem.TaskStatus = TaskStatusEnum.InProgress;
                        if (aTaskItem.TaskCancellationTokenSource.IsCancellationRequested)
                        {
                            percentComplete = 0;
                            aTaskItem.TaskStatus = TaskStatusEnum.Cancel;
                            aTaskItem.Progress.Report(percentComplete);
                            return;
                        }
                        await Task.Delay(10);
                        percentComplete++;
                        if (aTaskItem.Progress != null)
                        {
                            aTaskItem.Progress.Report(percentComplete);
                        }
                        if (percentComplete == 100)
                        {
                            aTaskItem.TaskStatus = TaskStatusEnum.Completed;
                            break;
                        }
                    }
                }, aTaskItem.TaskCancellationTokenSource.Token).ConfigureAwait(false);
                return;
            }
            catch (Exception ex)
            {
                if (aTaskItem.TaskCancellationTokenSource.IsCancellationRequested)
                {
                    //ignore
                }
                else
                {
                    aTaskItem.TaskStatus = TaskStatusEnum.Error;
                    aTaskItem.TaskMessage = ex.Message;
                }
            }
            return;

        }
    }
}
