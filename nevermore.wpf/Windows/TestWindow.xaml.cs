using nevermore.mvvm.Command;
using nevermore.wpf.Controls;
using nevermore.wpf.Interface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
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
    public partial class TestWindow : Window, ITaskMonitorContext
    {
        private ObservableCollection<TaskItem<bool>> taskCollection;
        public ObservableCollection<TaskItem<bool>> TaskCollection
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

        public ICommand CancelTaskCommand { get; set; }
        public ICommand RetryTaskCommand { get; set; }
        public TaskExcuteDelegate TaskExcuteHandler { get; set; }
        public int MaxTaskQuantity { get; set; }

        ManualResetEvent resetEvent = new ManualResetEvent(true);
        private IEnumerable<Task> progressingTasks;
        object info = null;
        public TestWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            TaskExcuteHandler = UploadFile;
            MaxTaskQuantity = 3;
            CancelTaskCommand = new NMCommand<TaskItem<bool>>(OnCancelTask);
            RetryTaskCommand = new NMCommand<TaskItem<bool>>(OnRetryTask);
            Init();
            progressingTasks = InitTaskInstance(TaskCollection);
            Task.Run(() => StartMutiTask(progressingTasks)).ConfigureAwait(false);
            //List<ObservableCollection<TaskItem<bool>>> collectionGroup = GroupByCount(TaskCollection, 3);
            //foreach (var collection in collectionGroup)
            //{
            //    progressingTasks = InitTaskInstance(collection);
            //    Task.Run(async () => await StartMutiTask(progressingTasks)).ConfigureAwait(false);
            //}
            //Task.WhenAny(collectionGroup.Select(async collection =>
            //{
            //    progressingTasks = InitTaskInstance(collection);
            //    await StartMutiTask(progressingTasks);
            //}));



        }
        private  void TaskEnumerbleExecutor(IEnumerator<Task> tasksSource)
        {
            if (tasksSource.MoveNext())
            {
                var currentTask = tasksSource.Current;
                if (currentTask != null)
                {
                    currentTask.ContinueWith(_ => TaskEnumerbleExecutor(tasksSource)); 
                }
            }
        }

        private async Task TaskEnumerbleExecutorByGroup(IEnumerator<Task> tasksSource,int aCount,int i = 1)
        {
            List<Task> tasksGourp = new List<Task>();
            if (tasksSource.MoveNext())
            {
                var currentTask = tasksSource.Current;
                if (currentTask != null)
                {
                    if (i % aCount != 0)
                    {
                        i++;
                        //await Task.Run(async () => await currentTask);
                        //await TaskEnumerbleExecutorByGroup(tasksSource, 3, i);
                        await Task.WhenAll(currentTask, TaskEnumerbleExecutorByGroup(tasksSource, aCount, i));
                    }
                    else
                    {
                        i++;
                        await currentTask.ContinueWith(async _ =>await TaskEnumerbleExecutorByGroup(tasksSource, aCount, i));
                    }
                }
            }
        }
        private async Task TaskEnumerbleExecutorByGroup(IEnumerator<IEnumerable<Task<bool>>> tasksSource)
        {
            if (tasksSource.MoveNext())
            {
                var currentTask = tasksSource.Current;
                if (currentTask != null)
                {
                   await Task.WhenAll(currentTask.ToArray()).ContinueWith(_ => TaskEnumerbleExecutorByGroup(tasksSource));
                }
            }
        }
        private async void OnRetryTask(TaskItem<bool> obj)
        {
            await Task.Run(async()=> 
            {
                while (TaskCollection.Where(x => x.TaskStatus == TaskStatusEnum.InProgress).Count() >= MaxTaskQuantity)
                {
                    obj.TaskStatus = TaskStatusEnum.Hangup;
                    await Task.Delay(1000);
                }
            });

            obj.TaskStatus = TaskStatusEnum.InProgress;
            if (!obj.TaskCancellationTokenSource.IsCancellationRequested)
            {
                obj.TaskCancellationTokenSource.Cancel();
            }
            obj.TaskCancellationTokenSource = new CancellationTokenSource();
            obj.TaskInstance = TaskExcuteHandler.Invoke(obj, obj.TaskCancellationTokenSource.Token);
            await obj.TaskInstance.ConfigureAwait(false);
            //await progressingTasks.FirstOrDefault(t => t.Id == obj.TaskInstance.Id);
            //await Task.Run(async () => { await obj.TaskInstance; }, obj.TaskCancellationTokenSource.Token).ConfigureAwait(false);
        }

        private void OnCancelTask(TaskItem<bool> obj)
        {
            obj.TaskStatus = TaskStatusEnum.Cancel;
            if (!obj.TaskCancellationTokenSource.IsCancellationRequested)
            {
                obj.TaskCancellationTokenSource.Cancel();
            }
        }
        private IEnumerable<Task> InitTaskInstance(ObservableCollection<TaskItem<bool>> aTasks)
        {
            var progressingTasks = aTasks.Select(async t =>
            {
                t.TaskInstance = TaskExcuteHandler.Invoke(t,t.TaskCancellationTokenSource.Token);
                await t.TaskInstance;
            });
            return progressingTasks;             
        }
        public void StartMutiTask(IEnumerable<Task> aProgressingTasks)
        {
            //GroupByCount(aProgressingTasks, 3);
            //await Task.Run(()=> TaskEnumerbleExecutorByGroup(progressingTaskGroups.GetEnumerator()));
            //同时执行所有task
            //await Task.WhenAll(aProgressingTasks.ToArray()).ConfigureAwait(false);
            //按顺序执行
            // await Task.Run(() => TaskEnumerbleExecutor(aProgressingTasks.GetEnumerator()));
            //分组执行
            Task.Run(() => TaskEnumerbleExecutorByGroup(aProgressingTasks.GetEnumerator(), MaxTaskQuantity));
            //bool[] res = await Task.WhenAll(aProgressingTasks.ToArray()).ConfigureAwait(false);
            //ThreadPool.SetMinThreads(1, 1);
            //ThreadPool.SetMaxThreads(9, 9);
            //Task.Factory = new TaskFactory(new LimitedConcurrencyLevelTaskScheduler(3));
            //Task.WhenAll(aProgressingTasks.ToArray()).Start(new LimitedConcurrencyLevelTaskScheduler(3));
            //Task[] tasks = new Task[] { Task.Delay(2000), Task.Delay(2000) };
            //TaskFactory tf = new TaskFactory(new LimitedConcurrencyLevelTaskScheduler(3));
            //await tf.ContinueWhenAll(aProgressingTasks.ToArray(), null);
            //if (res.Any(x => x == true))
            //{
            //await Task.Delay(2000);
            //}
        }
        private List<ObservableCollection<T>> GroupByCount<T>(ObservableCollection<T> aCollection, int aCount)
        {
            List<ObservableCollection<T>> res = new List<ObservableCollection<T>>();
            while (aCollection.Count() >= aCount)
            {
                ObservableCollection<T> group =new ObservableCollection<T>(aCollection.Take(aCount));
                aCollection = new ObservableCollection<T>(aCollection.Skip(aCount));
                res.Add(group);
            }
            if (aCollection.Count() != 0)
            {
                res.Add(aCollection);
            }
            return res;
        }
        private IEnumerable<IEnumerable<T>> GroupByCount<T>(IEnumerable<T> aCollection, int aCount)
        {
            List<IEnumerable<T>> res = new List<IEnumerable<T>>();
            while (aCollection.Count() >= aCount)
            {
                IEnumerable<T> group = aCollection.Take(aCount);
                aCollection = aCollection.Skip(aCount);
                res.Add(group);
            }
            if (aCollection.Count() != 0)
            {
                res.Add(aCollection);
            }
            return res;
        }
        //private async Task UpLoadFileAsync(TaskItem aTaskItem,CancellationToken cancellationToken)
        //{
        //    switch (aTaskItem.FileType)
        //    {
        //        case FileTypeEnum.DOC:
        //            await Task.Run(async () =>
        //            {
        //                int percentComplete = 0;
        //                aTaskItem.TaskMessage = string.Empty;
        //                while (true)
        //                {
        //                    if (cancellationToken.IsCancellationRequested)
        //                    {
        //                        percentComplete = 0;
        //                        aTaskItem.Progress.Report(percentComplete);
        //                        aTaskItem.TaskMessage = "该任务已被取消"; 
        //                        return;
        //                    }
        //                    await Task.Delay(100);
        //                    percentComplete++;
        //                    if (aTaskItem.Progress != null)
        //                    {
        //                        aTaskItem.Progress.Report(percentComplete);
        //                    }
        //                    if (percentComplete == 100)
        //                    {
        //                        aTaskItem.TaskMessage = "已完成";
        //                        break;
        //                    }
        //                }
        //            }, cancellationToken);
        //            break;
        //        case FileTypeEnum.PNG:
        //            await Task.Run(async () =>
        //            {
        //                int percentComplete = 0;
        //                aTaskItem.TaskMessage = string.Empty;
        //                while (true)
        //                {
        //                    if (cancellationToken.IsCancellationRequested)
        //                    {
        //                        percentComplete = 0;
        //                        aTaskItem.Progress.Report(percentComplete);
        //                        aTaskItem.TaskMessage = "该任务已被取消";
        //                        return;
        //                    }
        //                    await Task.Delay(200);
        //                    percentComplete++;
        //                    if (aTaskItem.Progress != null)
        //                    {
        //                        aTaskItem.Progress.Report(percentComplete);
        //                    }
        //                    if (percentComplete == 100)
        //                    {
        //                        aTaskItem.TaskMessage = "已完成";
        //                        break;
        //                    }
        //                }
        //            }, cancellationToken);
        //            break;
        //        case FileTypeEnum.PDF:
        //            await Task.Run(async () =>
        //            {
        //                int percentComplete = 0;
        //                aTaskItem.TaskMessage = string.Empty;
        //                while (true)
        //                {
        //                    if (cancellationToken.IsCancellationRequested)
        //                    {
        //                        percentComplete = 0;
        //                        aTaskItem.Progress.Report(percentComplete);
        //                        aTaskItem.TaskMessage = "该任务已被取消";
        //                        return;
        //                    }
        //                    await Task.Delay(300);
        //                    percentComplete++;
        //                    if (aTaskItem.Progress != null)
        //                    {
        //                        aTaskItem.Progress.Report(percentComplete);
        //                    }
        //                    if (percentComplete == 100)
        //                    {
        //                        aTaskItem.TaskMessage = "已完成";
        //                        break;
        //                    }
        //                }
        //            }, cancellationToken);
        //            break;
        //        case FileTypeEnum.MP3:
        //            await Task.Run(async () =>
        //            {
        //                int percentComplete = 0;
        //                aTaskItem.TaskMessage = string.Empty;
        //                while (true)
        //                {
        //                    if (cancellationToken.IsCancellationRequested)
        //                    {
        //                        percentComplete = 0;
        //                        aTaskItem.Progress.Report(percentComplete);
        //                        aTaskItem.TaskMessage = "该任务已被取消";
        //                        return;
        //                    }
        //                    await Task.Delay(90);
        //                    percentComplete++;
        //                    if (aTaskItem.Progress != null)
        //                    {
        //                        aTaskItem.Progress.Report(percentComplete);
        //                    }
        //                    if (percentComplete == 100)
        //                    {
        //                        aTaskItem.TaskMessage = "已完成";
        //                        break;
        //                    }
        //                }
        //            }, cancellationToken);
        //            break;
        //    }

        //}
        private async Task<bool> UpLoadFileAsync(TaskItem<bool> aTaskItem,CancellationToken cancellationToken)
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
                        if (cancellationToken.IsCancellationRequested)
                        {
                            percentComplete = 0;
                            aTaskItem.TaskStatus = TaskStatusEnum.Cancel;
                            aTaskItem.Progress.Report(percentComplete);
                            return;
                        }
                        await Task.Delay(100);
                        percentComplete++;
                        if (aTaskItem.Progress != null)
                        {
                            aTaskItem.Progress.Report(percentComplete);
                        }
                        if (aTaskItem.TaskProgressRatio == 66 && aTaskItem.FileType == FileTypeEnum.MP3)
                        {
                            throw new Exception("文件被占用，请关闭文件后重试！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！");
                        }
                        if (percentComplete == 100)
                        {
                            aTaskItem.TaskStatus = TaskStatusEnum.Completed;
                            break;
                        }
                    }
                }, cancellationToken).ConfigureAwait(false);
                return true;
            }            
            catch(Exception ex)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    //ignore
                }
                else
                {
                    aTaskItem.TaskStatus = TaskStatusEnum.Error;
                    aTaskItem.TaskMessage = ex.Message;
                }
            }
            return false;

        }
        private void Init()
        {
            TaskCollection = new ObservableCollection<TaskItem<bool>>
            {
                new TaskItem<bool>
                {
                     TaskId = new Random().Next(),
                     TaskName = "WCF服务编程中文版.pdf",
                     TaskProgressRatio = 0,
                     FileType = FileTypeEnum.PDF,
                     FilePath = @"F:\bak\WCF服务编程中文版.pdf",
                     TaskStatus =  TaskStatusEnum.Ready,
                },
                new TaskItem<bool>
                {
                     TaskId = new Random().Next(),
                     TaskName = "811d754fa7e44a339bdcc856e54bd7a8.png",
                     FilePath = @"F:\bak\811d754fa7e44a339bdcc856e54bd7a8.png",
                     TaskProgressRatio = 0,
                     FileType = FileTypeEnum.PNG,
                     TaskStatus =  TaskStatusEnum.Ready,
                },
                new TaskItem<bool>
                {
                     TaskId = new Random().Next(),
                     TaskName = "811d754fa7e44a339bdcc856e54bd7a8.png",
                     FilePath = @"F:\bak\811d754fa7e44a339bdcc856e54bd7a8.png",
                     TaskProgressRatio = 0,
                     FileType = FileTypeEnum.PNG,
                     TaskStatus =  TaskStatusEnum.Ready,
                },
                new TaskItem<bool>
                {
                     TaskId = new Random().Next(),
                     TaskName = "811d754fa7e44a339bdcc856e54bd7a8.png",
                     FilePath = @"F:\bak\811d754fa7e44a339bdcc856e54bd7a8.png",
                     TaskProgressRatio = 0,
                     FileType = FileTypeEnum.PNG,
                     TaskStatus =  TaskStatusEnum.Ready,
                },
                new TaskItem<bool>
                {
                     TaskId = new Random().Next(),
                     TaskName = "811d754fa7e44a339bdcc856e54bd7a8.png",
                     FilePath = @"F:\bak\811d754fa7e44a339bdcc856e54bd7a8.png",
                     TaskProgressRatio = 0,
                     FileType = FileTypeEnum.PNG,
                     TaskStatus =  TaskStatusEnum.Ready,
                },
                new TaskItem<bool>
                {
                     TaskId = new Random().Next(),
                     TaskName = "811d754fa7e44a339bdcc856e54bd7a8.png",
                     FilePath = @"F:\bak\811d754fa7e44a339bdcc856e54bd7a8.png",
                     TaskProgressRatio = 0,
                     FileType = FileTypeEnum.PNG,
                     TaskStatus =  TaskStatusEnum.Ready,
                },
                new TaskItem<bool>
                {
                     TaskId = new Random().Next(),
                     TaskName = "811d754fa7e44a339bdcc856e54bd7a8.png",
                     FilePath = @"F:\bak\811d754fa7e44a339bdcc856e54bd7a8.png",
                     TaskProgressRatio = 0,
                     FileType = FileTypeEnum.PNG,
                     TaskStatus =  TaskStatusEnum.Ready,
                },
                new TaskItem<bool>
                {
                     TaskId = new Random().Next(),
                     TaskName = "WCF服务编程中文版.pdf",
                     FilePath = @"F:\bak\WCF服务编程中文版.pdf",
                     TaskProgressRatio = 0,
                     FileType = FileTypeEnum.PNG,
                     TaskStatus =  TaskStatusEnum.Ready,
                },
                new TaskItem<bool>
                {
                     TaskId = new Random().Next(),
                     TaskName = "811d754fa7e44a339bdcc856e54bd7a8.png",
                     FilePath = @"F:\bak\811d754fa7e44a339bdcc856e54bd7a8.png",
                     TaskProgressRatio = 0,
                     FileType = FileTypeEnum.PNG,
                     TaskStatus =  TaskStatusEnum.Ready,
                },
                new TaskItem<bool>
                {
                     TaskId = new Random().Next(),
                     TaskName = "811d754fa7e44a339bdcc856e54bd7a8.png",
                     FilePath = @"F:\bak\811d754fa7e44a339bdcc856e54bd7a8.png",
                     TaskProgressRatio = 0,
                     FileType = FileTypeEnum.PNG,
                     TaskStatus =  TaskStatusEnum.Ready,
                },
            };
            info = new { url = @"http://172.18.19.101:8888/testimony/api/v1/files"};
        }
        private  void ReSetAll()
        {
            //只对同时上传有效
            //TaskCollection.ToList().ForEach(x => x.TaskCancellationTokenSource.Cancel());
            //TaskCollection.ToList().ForEach(x => x.TaskCancellationTokenSource = new CancellationTokenSource());
            //progressingTasks = InitTaskInstance(TaskCollection);
            //Task.Run(() => StartMutiTask(progressingTasks)).ConfigureAwait(false);
        }
        private  void CancelAll()
        {
            //只对同时上传有效
            //TaskCollection.ToList().ForEach(x => x.TaskCancellationTokenSource.Cancel());
            //TaskCollection.ToList().ForEach(x => x.TaskStatus = TaskStatusEnum.Cancel) ;
        }

        private async Task<bool> UploadFile(TaskItem<bool> aTaskItem, CancellationToken cancellationToken, dynamic info)
        {
            float percentComplete = 0;
            aTaskItem.Progress?.Report(percentComplete);
            aTaskItem.TaskStatus = TaskStatusEnum.InProgress;
            //await Task.Run(() => 
            //{
            //    while(true)
            //    {
            //        if (cancellationToken.IsCancellationRequested)
            //        {
            //            aTaskItem.TaskStatus = TaskStatusEnum.Cancel;
            //            aTaskItem.Progress.Report(percentComplete);
            //            return false;
            //        }
            //    }
            //});

            // 时间戳，用做boundary  
            string timeStamp = DateTime.Now.Ticks.ToString("x");

            //根据uri创建HttpWebRequest对象  
            string token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjE1OTk2ODY1NTEsInVzZXJfbmFtZSI6ImZtVFAwMnRNSkJhQVh5S3kzR1FCWUE9PSIsImp0aSI6IjY0YjBiOTkwLTYxNjgtNDA4My05ODI1LTc0NmIzNDk1ZmUzMiIsImNsaWVudF9pZCI6InByaXZhdGVfY2xpZW50X3dpbmRvd3MiLCJzY29wZSI6WyJzY29wZV9jb3JlIl19.GOcwNv2Hb7e6wS0WROzLRGaJOK07uKwxMJhK5tJnXWQ";
            HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create(new Uri(@"http://172.18.19.101:8888/testimony/api/v1/files"));
            httpReq.Method = "POST";
            httpReq.Headers.Add("Authorization", $"Bearer {token}");
            httpReq.AllowWriteStreamBuffering = false; //对发送的数据不使用缓存  
            httpReq.Timeout = 1800000; //设置获得响应的超时时间（30分钟）  

            httpReq.ContentType = "multipart/form-data; boundary=" + timeStamp;
            httpReq.KeepAlive = false;
            httpReq.ServicePoint.Expect100Continue = false;
            try
            {
                //文件  
                using (FileStream fileStream = new FileStream(aTaskItem.FilePath, FileMode.Open, FileAccess.Read))
                {
                    using (BinaryReader binaryReader = new BinaryReader(fileStream))
                    {
                        //头信息  
                        string boundary = timeStamp;
                        string startBoundary = "--" + timeStamp;
                        string dataFormat = "\r\n" + startBoundary +
                                            "\r\nContent-Disposition: form-data; name=\"{0}\";filename=\"{1}\" \r\nContent-Type:application/octet-stream\r\n\r\n";

                        string header = string.Format(dataFormat, "fileName", aTaskItem.TaskName);

                        byte[] postHeaderBytes = Encoding.UTF8.GetBytes(header);

                        //------------------------------[form-data Start]----------------------------
                        string reqFormat = startBoundary + "\r\nContent-Type:text/plain;charset=UTF-8" +
                                           "\r\nContent-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}\r\n";
                        string req = string.Format(reqFormat, "name", aTaskItem.TaskName);
                        byte[] reqBytes = Encoding.UTF8.GetBytes(req);

                        //结束边界  
                        byte[] boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + timeStamp + "--\r\n");
                        //------------------------------[form-data End]----------------------------

                        byte[] reqBytes1;
                        //------------------------------[form-data Start]----------------------------
                        reqFormat = startBoundary + "\r\nContent-Type:text/plain;charset=UTF-8" +
                                    "\r\nContent-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}\r\n";
                        req = string.Format(reqFormat, "reserveId", "dfe64873568a4894b7efab5902ebca80");
                        reqBytes1 = Encoding.UTF8.GetBytes(req);
                        //------------------------------[form-data End]----------------------------

                        //------------------------------[form-data Start]----------------------------
                        reqFormat = startBoundary + "\r\nContent-Type:text/plain;charset=UTF-8" +
                                    "\r\nContent-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}\r\n";
                        req = string.Format(reqFormat, "participantId", "edc423566078463fadb22b752a4948b0");
                        byte[] reqBytes2 = Encoding.UTF8.GetBytes(req);
                        //------------------------------[form-data End]----------------------------

                        //------------------------------[form-data Start]----------------------------
                        reqFormat = startBoundary + "\r\nContent-Type:text/plain;charset=UTF-8" +
                                    "\r\nContent-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}\r\n";
                        req = string.Format(reqFormat, "suffix", System.IO.Path.GetExtension(aTaskItem.FilePath).TrimStart('.'));
                        byte[] reqBytes3 = Encoding.UTF8.GetBytes(req);
                        //------------------------------[form-data End]----------------------------

                        //------------------------------[form-data Start]----------------------------
                        reqFormat = startBoundary + "\r\nContent-Type:text/plain;charset=UTF-8" +
                                    "\r\nContent-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}\r\n";
                        req = string.Format(reqFormat, "ascription", 2);
                        byte[] reqBytes4 = Encoding.UTF8.GetBytes(req);
                        //------------------------------[form-data End]----------------------------

                        //------------------------------[form-data Start]----------------------------
                        reqFormat = startBoundary + "\r\nContent-Type:text/plain;charset=UTF-8" +
                                    "\r\nContent-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}\r\n";
                        req = string.Format(reqFormat, "uploadFileId", aTaskItem.TaskId);
                        byte[] reqBytes5 = Encoding.UTF8.GetBytes(req);
                        //------------------------------[form-data End]----------------------------

                        //------------------------------[form-data Start]----------------------------
                        reqFormat = startBoundary + "\r\nContent-Type:text/plain;charset=UTF-8" +
                                    "\r\nContent-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}\r\n";
                        req = string.Format(reqFormat, "source", 1);
                        byte[] reqBytes6 = Encoding.UTF8.GetBytes(req);
                        //------------------------------[form-data End]----------------------------
                        //------------------------------[form-data Start]----------------------------
                        reqFormat = startBoundary + "\r\nContent-Type:text/plain;charset=UTF-8" +
                                    "\r\nContent-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}\r\n";
                        req = string.Format(reqFormat, "ajbh", "6ba1f69eab24493691562419ccc8b8b5");
                        byte[] reqBytes7 = Encoding.UTF8.GetBytes(req);
                        //------------------------------[form-data End]----------------------------
                        //------------------------------[form-data Start]----------------------------
                        reqFormat = startBoundary + "\r\nContent-Type:text/plain;charset=UTF-8" +
                                    "\r\nContent-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}\r\n";
                        req = string.Format(reqFormat, "uploader", "刘子为");
                        byte[] reqBytes8 = Encoding.UTF8.GetBytes(req);
                        //------------------------------[form-data End]----------------------------
                        //------------------------------[form-data Start]----------------------------
                        reqFormat = startBoundary + "\r\nContent-Type:text/plain;charset=UTF-8" +
                                    "\r\nContent-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}\r\n";
                        req = string.Format(reqFormat, "jbfy", "");
                        byte[] reqBytes9 = Encoding.UTF8.GetBytes(req);
                        //------------------------------[form-data End]----------------------------                
                        //------------------------------[form-data Start]----------------------------
                        reqFormat = startBoundary + "\r\nContent-Type:text/plain;charset=UTF-8" +
                                    "\r\nContent-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
                        req = string.Format(reqFormat, "reservepattern", 2);
                        byte[] reqBytes10 = Encoding.UTF8.GetBytes(req);
                        //------------------------------[form-data End]----------------------------

                        long length = fileStream.Length + postHeaderBytes.Length + boundaryBytes.Length + reqBytes.Length
                                      + reqBytes1.Length + reqBytes2.Length + reqBytes3.Length + reqBytes4.Length +
                                      reqBytes5.Length + reqBytes6.Length + reqBytes7.Length + reqBytes8.Length
                                      + reqBytes9.Length + reqBytes10.Length;


                        httpReq.ContentLength = length; //请求内容长度              

                        //每次上传8k  
                        int bufferLength = 8192;
                        byte[] buffer = new byte[bufferLength];

                        int size = binaryReader.Read(buffer, 0, bufferLength);
                        Stream postStream = httpReq.GetRequestStream();

                        //发送请求头部消息                  
                        postStream.Write(reqBytes, 0, reqBytes.Length);
                        postStream.Write(reqBytes1, 0, reqBytes1.Length);
                        postStream.Write(reqBytes2, 0, reqBytes2.Length);
                        postStream.Write(reqBytes3, 0, reqBytes3.Length);
                        postStream.Write(reqBytes4, 0, reqBytes4.Length);
                        postStream.Write(reqBytes5, 0, reqBytes5.Length);
                        postStream.Write(reqBytes6, 0, reqBytes6.Length);
                        postStream.Write(reqBytes7, 0, reqBytes7.Length);
                        postStream.Write(reqBytes8, 0, reqBytes8.Length);
                        postStream.Write(reqBytes9, 0, reqBytes9.Length);
                        postStream.Write(reqBytes10, 0, reqBytes10.Length);
                        postStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);
                        percentComplete = percentComplete+10;
                        aTaskItem.Progress?.Report(percentComplete);
                        await Task.Run(async() => 
                        {
                            float re = 0;
                            while (size > 0)
                            {
                                postStream.Write(buffer, 0, size);
                                //info.nowBytes += size;
                                re += ((float)bufferLength) / ((float)length);
                                percentComplete = re * 70;
                                aTaskItem.Progress?.Report(percentComplete+10);
                                size = binaryReader.Read(buffer, 0, bufferLength);
                                if (re.ToString("0.0") == "0.1" || re.ToString("0.0") == "0.5" || re.ToString("0.0") == "0.9")
                                {
                                    //TimeSpan.FromMilliseconds(0.1);
                                    await Task.Delay(TimeSpan.FromMilliseconds(0.5));
                                }
                            }
                        });

                        //添加尾部边界
                        postStream.Write(boundaryBytes, 0, boundaryBytes.Length);
                        postStream.Close();
                        percentComplete = percentComplete + 10;
                        aTaskItem.Progress?.Report(percentComplete);
                    }
                }
                //获取服务器端的响应  
                using (HttpWebResponse response = (HttpWebResponse)await httpReq.GetResponseAsync())
                {
                    using (Stream receiveStream = response.GetResponseStream())
                    {
                        using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                        {
                            string returnValue = readStream.ReadToEnd();
                            if (string.IsNullOrEmpty(returnValue))
                            {
                                return false;
                            }
                            else
                            {
                                percentComplete = 100;
                                aTaskItem.Progress?.Report(percentComplete);
                                aTaskItem.TaskStatus = TaskStatusEnum.Completed;
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    //ignore
                }
                else
                {
                    aTaskItem.TaskStatus = TaskStatusEnum.Error;
                    aTaskItem.TaskMessage = ex.Message;
                }
            }
            return false;
        }

        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            ReSetAll();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            CancelAll();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
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
    }
}
