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

        private async void button_FileSelect_Click(object sender, RoutedEventArgs e)
        {
            if (!isTaskCompleted)
            {
                return;
            }
            using (testWindow = new TestWindow())
            {
                isTaskCompleted = false;
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    InitialDirectory = @"C:\desktop",
                    Filter = "所有文件(*.*)|*.*",
                    Multiselect = true //是否可以多选true=ok/false=no
                };
                if (openFileDialog.ShowDialog() == true)
                {
                    testWindow.Show();
                    await testWindow.RunTaskMonitor(openFileDialog.FileNames.ToList()).ContinueWith(_=>
                    {
                        isTaskCompleted = true;
                    });
                }
            }
        }

        private void button_Monitor_Click(object sender, RoutedEventArgs e)
        {
            if (!isTaskCompleted)
            {
                testWindow?.Show();
            }
        }
    }
}
