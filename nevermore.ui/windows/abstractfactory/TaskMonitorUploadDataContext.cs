using nevermore.ui.wpfcontrols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nevermore.ui.windows
{
    public class TaskMonitorUploadDataContext : TaskMonitorDataContext
    {
        public override void RunTaskMonitor(IEnumerable<ITaskItemContext> aTaskItem, Func<ITaskItemContext, object[], Task> aTaskExcuteDelegate, params object[] optionalParams)
        {
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
                TaskCollection.Add(x);
            });
            commonParams = optionalParams;
            RunTaskCollection();
        }
    }
}
