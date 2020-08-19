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
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
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

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {

        }
    }
}
