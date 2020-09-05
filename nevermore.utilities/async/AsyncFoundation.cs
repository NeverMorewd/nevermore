using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace nevermore.learn.async
{
    public class AsyncFoundation
    {
        /// <summary>
        /// 任务超次
        /// </summary>
        /// <param name="anUri"></param>
        /// <param name="aRetryTimes"></param>
        /// <returns></returns>
        public static async Task<Stream> DownloadStreamWithRetries(string anUri, int aRetryTimes)
        {
            using (var client = new HttpClient())
            {
                var nextDelay = TimeSpan.FromSeconds(1);
                for (int i = 0; i < aRetryTimes; i++)
                {
                    try
                    {
                        return await client.GetStreamAsync(anUri);
                    }
                    catch
                    {
                        
                    }
                    await Task.Delay(nextDelay);
                    nextDelay += nextDelay;
                }
                //超过重试次数后，最后调一次 ，让调用者知道异常
                return await client.GetStreamAsync(anUri);
            }              
        }

        /// <summary>
        /// 任务超时：推荐使用CancellationToken实现超时控制
        /// </summary>
        /// <param name="anUri"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public static async Task<Stream> DownloadStreamWithTimeOut(string anUri, int timeOut)
        {
            using (var client = new HttpClient())
            {
                var downLoadTask = client.GetStreamAsync(anUri);
                var timeoutTask = Task.Delay(timeOut);

                var completedTask = Task.WhenAll(downLoadTask, timeoutTask);
                if (completedTask == timeoutTask)
                    return null;
                return await downLoadTask;
            }         
        }

        /// <summary>
        /// 报告进度
        /// </summary>
        /// <param name="aProgress"></param>
        /// <returns></returns>
        public static async Task MyMethodAsync(IProgress<double> aProgress = null)
        {
            double percentComplete = 0;
            bool done = false;
            while (!done)
            {
                //do sth
                await Task.Delay(1000);
                percentComplete++;
                aProgress?.Report(percentComplete);
            }
        }
        /// <summary>
        /// 处理进度
        /// </summary>
        /// <returns></returns>
        public static async Task CallMyMethodAsync()
        {
            var progress = new Progress<double>();
            progress.ProgressChanged += (sender, args ) =>
            {
                
            };
            var progressCallBack = new Progress<double>((ratio)=> 
            {
                
            });
            await MyMethodAsync(progress);
        }

        /// <summary>
        /// 等待所有任务完成
        /// </summary>
        /// <returns></returns>
        public static async Task WaitAllTask()
        {
            Task task1 = Task.Delay(TimeSpan.FromSeconds(1));
            Task task2 = Task.Delay(TimeSpan.FromSeconds(2));
            Task task3 = Task.Delay(TimeSpan.FromSeconds(3));
            await Task.WhenAll(task1, task2, task3);

            Task<int> task4 = Task.FromResult(1);
            Task<int> task5 = Task.FromResult(2);
            Task<int> task6 = Task.FromResult(3);
            int[] results = await Task.WhenAll(task4, task5, task6);
            //results = {1,2,3}
        }

        /// <summary>
        /// Task.WhenAll支持IEnumerable作为参数的重载，但不支持这样使用
        /// </summary>
        /// <param name="urls"></param>
        /// <returns></returns>
        public static async Task<string> DownloadAllAsync(IEnumerable<string> urls)
        {
            var httpClient = new HttpClient();
            var downloads = urls.Select(url => httpClient.GetStringAsync(url));//任务未启动
            Task<string>[] downloadTasks = downloads.ToArray();//任务启动
            string[] htmlPages = await Task.WhenAll(downloadTasks);
            return string.Concat(htmlPages);                               
        }
        /// <summary>
        /// 同上，下载文件流
        /// </summary>
        /// <param name="urls"></param>
        /// <returns></returns>
        public static async Task<Stream[]> DownloadStreamAllAsync(IEnumerable<string> urls)
        {
            var httpClient = new HttpClient();
            var downloads = urls.Select(url => httpClient.GetStreamAsync(url));//任务未启动
            Task<Stream>[] downloadTasks = downloads.ToArray();//任务启动
            Stream[] streams = await Task.WhenAll(downloadTasks);
            return streams;
        }
        /// <summary>
        /// 监听一组任务中的一个异常，通常情况下只需要监听一个
        /// </summary>
        /// <returns></returns>
        public static async Task ObserveOneExceptionAsync()
        {
            var task1 = Task.FromResult(new NullReferenceException());
            var task2 = Task.FromResult(new NotImplementedException());
            try
            {
                await Task.WhenAll(task1, task2);
            }
            catch(Exception ex)
            {
                if (ex is NullReferenceException)
                {

                }
                else if (ex is NotImplementedException)
                {
                    
                }
            }
        }
        /// <summary>
        /// 监听一组任务中的所有异常
        /// </summary>
        /// <returns></returns>
        public static async Task ObserveAllExceptionsAsync()
        {
            var task1 = Task.FromResult(new NullReferenceException());
            var task2 = Task.FromResult(new NotImplementedException());
            Task allTasks = Task.WhenAll(task1, task2);
            try
            {
                await allTasks;
            }
            catch
            {
                AggregateException exceptions = allTasks.Exception;
            }
        }
        /// <summary>
        /// 等待任意一个任务完成
        /// </summary>
        /// <param name="aUrlA"></param>
        /// <param name="aUrlB"></param>
        /// <returns></returns>
        public static async Task<Stream> FirstRespondingUrlAsync(string aUrlA,string aUrlB)
        {
            var httpClient = new HttpClient();

            Task<Stream> downloadTaskA = httpClient.GetStreamAsync(aUrlA);
            Task<Stream> downloadTaskB = httpClient.GetStreamAsync(aUrlB);

            Task<Stream> completedTask = await Task.WhenAny(downloadTaskA,downloadTaskB);

            Stream stream = await completedTask;
            return stream;
        }

        public static async Task<int> DelayAndReturnAsync(int val)
        {
            await Task.Delay(TimeSpan.FromSeconds(val));
            return val;
        }
        public static async Task<int> DelayAndReturnAndWriteAsync(int val)
        {
            await Task.Delay(TimeSpan.FromSeconds(val));
            Trace.WriteLine(val);
            return val;
        }
        public static async Task AwaitAndProcessAsync(Task<int> task)
        {
            var result = await task;
            Trace.WriteLine(result);
        }
        /// <summary>
        /// ABC按顺序执行，输出结果：2 3 1
        /// </summary>
        /// <returns></returns>
        public static async Task ProcessTasksAsync()
        {
            Task<int> taskA = DelayAndReturnAsync(2);
            Task<int> taskB = DelayAndReturnAsync(3);
            Task<int> taskC = DelayAndReturnAsync(1);

            var tasks = new[] { taskA,taskB,taskC };
            foreach(var task in tasks)
            {
                var result = await task;
                Trace.WriteLine(result);
            }
        }
        /// <summary>
        /// ABC并发执行，输出结果1 2 3
        /// </summary>
        /// <returns></returns>
        public static async Task ProcessTasksOrderAsync()
        {
            Task<int> taskA = DelayAndReturnAsync(2);
            Task<int> taskB = DelayAndReturnAsync(3);
            Task<int> taskC = DelayAndReturnAsync(1);

            var tasks = new[] { taskA, taskB, taskC };
            var processtasks = tasks.Select(async t =>
            {
                var result = await t;
                Trace.WriteLine(result);
            }).ToArray();

            await Task.WhenAll(processtasks);
        }

        /// <summary>
        /// 这样是什么结果呢
        /// </summary>
        /// <returns></returns>
        public static async Task ProcessTaskListAsync()
        {
            Task<int> taskA = DelayAndReturnAndWriteAsync(2);
            Task<int> taskB = DelayAndReturnAndWriteAsync(3);
            Task<int> taskC = DelayAndReturnAndWriteAsync(1);

            List<Task> tasks = new List<Task> { taskA, taskB, taskC };
            await Task.WhenAll(tasks);
        }

    }

    interface IMyAsyncInterface
    {
        Task<int> GetValueAsync();
    }
    /// <summary>
    /// 实现一个具有异步签名的同步方法
    /// </summary>
    class MySynchronousImplementation : IMyAsyncInterface
    {
        private static readonly Task<int> zeroTask = Task.FromResult(12);
        public Task<int> GetValueAsync()
        {
            return zeroTask;
            //return Task.FromResult(12); //同上
        }
        /// <summary>
        /// Task.FromResult() 就是TaskCompletionSource的一个简化版本
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Task<T> NotImplementedAsync<T>()
        {
            var tcs = new TaskCompletionSource<T>();
            tcs.SetException(new NotImplementedException());
            tcs.SetResult(default);
            return tcs.Task;
        }
    }
}
