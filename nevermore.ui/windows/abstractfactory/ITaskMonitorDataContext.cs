using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nevermore.ui.windows
{
    public interface ITaskMonitorDataContext
    {
        Action<ITaskItemContext> OnRetryTask { get;  }
        Action<ITaskItemContext> OnCancelTask { get;  }
    }
}
