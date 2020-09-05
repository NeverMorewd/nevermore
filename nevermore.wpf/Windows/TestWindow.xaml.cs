﻿using nevermore.wpf.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
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
using System.Windows.Threading;

namespace nevermore.wpf
{
    /// <summary>
    /// TestWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TestWindow : Window,INotifyPropertyChanged
    {
        readonly IProgress<float> Progress;
        private ObservableCollection<TaskItem> taskCollection;
        public ObservableCollection<TaskItem> TaskCollection
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
        static bool done = false;
        public TestWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            Progress = new Progress<float>(ProgressHandler);
            Init();
            Start();
        }

        public TestWindow(List<string> uploadFiles)
        {
            InitializeComponent();
            this.DataContext = this;
            Progress = new Progress<float>(ProgressHandler);

            Task[] tasks = new Task[10];
            foreach (string fullFileName in uploadFiles)
            {
                tasks.Append(Task.FromResult(fullFileName));
            }

            Init();
            Start();
        }
        private async Task UpLoadFileAsync(string aFileName)
        {
            var htttpClient = new HttpClient();
            //htttpClient.PostAsync("", aFileName);
        }
        private void Init()
        {
            TaskCollection = new ObservableCollection<TaskItem>
            {
                new TaskItem
                {
                     TaskId = new Random().Next(),
                     TaskName = "证据文件-1.doc",
                     TaskProgressRatio = 0,
                     TaskStatus =  TaskStatusEnum.Ready,
                },
                new TaskItem
                {
                     TaskId = new Random().Next(),
                     TaskName = "证据文件-2.doc",
                     TaskProgressRatio = 0,
                     TaskStatus =  TaskStatusEnum.Ready,
                },
                new TaskItem
                {
                     TaskId = new Random().Next(),
                     TaskName = "证据文件-3.doc",
                     TaskProgressRatio = 0,
                     TaskStatus =  TaskStatusEnum.Ready,
                },
                new TaskItem
                {
                     TaskId = new Random().Next(),
                     TaskName = "证据文件-4.doc",
                     TaskProgressRatio = 0,
                     TaskStatus =  TaskStatusEnum.Ready,
                },
                new TaskItem
                {
                     TaskId = new Random().Next(),
                     TaskName = "证据文件-5.doc",
                     TaskProgressRatio = 0,
                     TaskStatus =  TaskStatusEnum.Ready,
                },
                new TaskItem
                {
                     TaskId = new Random().Next(),
                     TaskName = "证据文件-6.doc",
                     TaskProgressRatio = 0,
                     TaskStatus =  TaskStatusEnum.Ready,
                },
                new TaskItem
                {
                     TaskId = new Random().Next(),
                     TaskName = "证据文件-7.doc",
                     TaskProgressRatio = 0,
                     TaskStatus =  TaskStatusEnum.Ready,
                },
                new TaskItem
                {
                     TaskId = new Random().Next(),
                     TaskName = "证据文件-8.doc",
                     TaskProgressRatio = 0,
                     TaskStatus =  TaskStatusEnum.Ready,
                },
                new TaskItem
                {
                     TaskId = new Random().Next(),
                     TaskName = "证据文件-9.doc",
                     TaskProgressRatio = 0,
                     TaskStatus =  TaskStatusEnum.Ready,
                },
                new TaskItem
                {
                     TaskId = new Random().Next(),
                     TaskName = "证据文件-10.doc",
                     TaskProgressRatio = 0,
                     TaskStatus =  TaskStatusEnum.Ready,
                },
            };
        }
        private async void Start()
        {
            await TaskAsync(Progress).ConfigureAwait(false);
        }
        private async void ReSet()
        {
            done = true;
            foreach (var task in TaskCollection)
            {
                task.TaskProgressRatio = 0;
                await TaskAsync(Progress);
            }
        }
        private void ProgressHandler(float taskRatio)
        {
            Application.Current.Dispatcher?.BeginInvoke(DispatcherPriority.Background, new Action(delegate
            {
                foreach (var task in TaskCollection)
                {
                    task.TaskProgressRatio = taskRatio;
                }
            }));
        }
        public static async Task TaskAsync(IProgress<float> progress = null)
        {
            float percentComplete = 0;
            done = false;
            while (!done)
            {
                //todo
                await Task.Run(async() =>
                {
                    await Task.Delay(200);
                    percentComplete++;
                    if (percentComplete == 100)
                        done = true;
                });
                if (progress != null)
                {
                    progress.Report(percentComplete);
                }
            }
        }






        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            ReSet();
        }
    }
}