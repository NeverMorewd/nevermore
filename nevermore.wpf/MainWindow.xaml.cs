using Microsoft.Win32;
using nevermore.common;
using nevermore.wpf.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
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
            if (testWindow == null)
            {
                testWindow = new TestWindow();
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
                testWindow.Show();
                testWindow.RunTaskMonitor(openFileDialog.FileNames.ToList());
            }
        }

        private void button_Monitor_Click(object sender, RoutedEventArgs e)
        {
            if (testWindow == null) testWindow = new TestWindow(); 
            if (!testWindow.IsActive)
            {
                testWindow.Show();
            }
        }
    }
}
