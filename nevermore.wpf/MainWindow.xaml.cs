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
        private Window Window;
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            Window = new TestWindow();
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(PathText))
            {
                ResultText = "OK!";
            }
            else
            {
                ResultText = "NOK!";
            }
            new TestWindow().Show();

        }

        private void button_Copy_Click(object sender, RoutedEventArgs e)
        {
            Window?.Show();
        }

        private void button_Click_1(object sender, RoutedEventArgs e)
        {
            new SecurityTestWindow().Show();
        }
    }
}
