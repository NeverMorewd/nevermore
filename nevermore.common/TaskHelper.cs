using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nevermore.common
{
    public class TaskHelper
    {
        public static async Task TaskAsync(IProgress<float> progress = null)
        {
            float percentComplete = 0;
            bool done = false;
            while(!done)
            {
                //todo
                await Task.Run(()=>
                {
                    Task.Delay(200);
                    percentComplete++;
                });
                if (progress != null)
                {
                    progress.Report(percentComplete);
                }
            }
        }
    }
}
