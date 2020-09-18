using nevermore.ui.windows;
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

namespace nevermore.ui.wpfcontrols
{
    /// <summary>
    /// TaskMonitorControl.xaml 的交互逻辑
    /// </summary>
    public partial class TaskMonitorControl : UserControl
    {
        public static readonly DependencyProperty TaskCollectionProperty = DependencyProperty.Register("TaskCollection", typeof(ObservableCollection<ITaskItemContext>), typeof(TaskMonitorControl), new PropertyMetadata(default(ObservableCollection<ITaskItemContext>))); 

        public ObservableCollection<ITaskItemContext> TaskCollection
        {
            get { return (ObservableCollection<ITaskItemContext>)GetValue(TaskCollectionProperty); }
            set { SetValue(TaskCollectionProperty, value); }
        }
        public TaskMonitorControl()
        {
            InitializeComponent(); 
        }
    }
}
