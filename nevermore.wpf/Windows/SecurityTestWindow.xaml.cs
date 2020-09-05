using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace nevermore.wpf.Windows
{
    /// <summary>
    /// SecurityTestWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SecurityTestWindow : Window
    {
        private static readonly string key = "smkldospdosldaaa";
        private static readonly string iv = "0392039203920300";
        public SecurityTestWindow()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            textBlock.Text = Decrypt(textBox.Text);
        }

        private  string Decrypt(string toDecrypt)
        {
            var keyArray = Encoding.UTF8.GetBytes(key);
            var ivArray = Encoding.UTF8.GetBytes(iv);
            var toEncryptArray = Convert.FromBase64String(toDecrypt);
            var rDel = new RijndaelManaged
            {
                Key = keyArray,
                IV = ivArray,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7
            };
            var cTransform = rDel.CreateDecryptor();
            var resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Encoding.UTF8.GetString(resultArray);
        }
    }
}
