using nevermore.utilities;
using nevermore.utilities.office;
using nevermore.winform;
using nevermore.wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Application_Winform =  System.Windows.Forms.Application;
using Application_WPF = System.Windows.Application;

namespace nevermore.console
{
    class Program
    {
        [DllImport("user32.dll", EntryPoint = "ShowWindow", SetLastError = true)]
        static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        /// <summary>
        /// start up  app
        /// </summary>
        /// <param name="args"></param>
        [STAThread]
        static void Main(string[] args)
        {
            //Console.Title = "Test";
            //IntPtr intptr = FindWindow("ConsoleWindowClass", "Test");
            //if (intptr != IntPtr.Zero)
            //{
            //     ShowWindow(intptr, 0);
            //}
            //Console.WriteLine(new TestRun().re);
            if (args != null && args.Length != 0)
            {
                string arg1 = args[0].Trim().ToLower();
                if(arg1.Equals("console"))
                {

                }
                else if (arg1.Equals("winform"))
                {
                    Application_Winform.EnableVisualStyles();
                    Application_Winform.SetCompatibleTextRenderingDefault(false);
                    Application_Winform.Run(new Form1());
                }
                else if (arg1.Equals("wpf"))
                {
                    Application_WPF app = new Application_WPF();
                    app.StartupUri = new Uri(@"C:\Users\Wang Dong\source\repos\nevermore\nevermore.wpf\MainWindow.xaml", UriKind.Absolute);
                    //MainWindow win = new MainWindow();
                    app.Run();
                }
            }
            else
            {
                //string[] paraArray = new string[]
                //{
                //    "ContainerPreNotification",
                //    "AllWithTarNbr",
                //    "0",
                //    "ContainerPreNotification$Id;ContainerPreNotification$Terminal;ContainerPreNotification$Status;ContainerPreNotification$OrderReference;ContainerPreNotification$ContainerId;ContainerPreNotification$LoadStatus;ContainerPreNotification$ISOCode;ContainerPreNotification$HandlingType;ContainerPreNotification$DriveThroughIndicator;ContainerPreNotification$TruckCallId;ContainerPreNotification$TruckingCompany;ContainerPreNotification$CreationDateTime;ContainerPreNotification$ChangeDateTime;ContainerPreNotification$PreGateCheckStatus;ContainerPreNotification$PreGateCheckDateTime;ContainerPreNotification$ExecutedByTruckCallId;ContainerPreNotification$ExecutedFromTruckAppointmentId;TruckCall$TAR;TruckCall$CallCardNumber;ContainerPreNotification$GateZone",
                //    "<Filter version=\"2.0\">\r\n  <CompoundFilter Type=\"And\">\r\n    <FilterLeaf Entity=\"ContainerPreNotification\" Property=\"Terminal\" Condition=\"=\" ContextType=\"string\" Context=\"K1742\" />\r\n    <CompoundFilter Type=\"Or\">\r\n      <FilterLeaf Entity=\"ContainerPreNotification\" Property=\"Status\" Condition=\"=\" ContextType=\"string\" Context=\"PRV\" />\r\n      <FilterLeaf Entity=\"ContainerPreNotification\" Property=\"Status\" Condition=\"=\" ContextType=\"string\" Context=\"ANC\" />\r\n    </CompoundFilter>\r\n  </CompoundFilter>\r\n</Filter>",
                //    "ContainerPreNotification$CreationDateTime$DESCENDING;ContainerPreNotification$CreationDateTime$DESCENDING"
                // };
                //while (!Console.ReadLine().Equals("q"))
                //{
                //    string res = DBHelper.GetDataSetUsingPagedStoredProcedure("ctcs_v53030x_csp_getEntityList", paraArray, 0, 12);
                //    Console.WriteLine(res);
                //}
                //WordReplaceTagWord.Replace(@"E:\project\TestFiles\ReplaceTags.docx");
                //new ReplaceTagWithoutWord();
                new insertPicToDoc().insert(@"C:\Users\Administrator\Desktop\mdbak.doc", @"C:\Users\Administrator\Desktop\158112890332.png", "SIGNT");
                Console.WriteLine("Enter 'q' to exit!");
                while (true)
                {
                    string path = Console.ReadLine();
                    if (path.Equals("q"))
                    {
                        break;
                    }
                    if (Directory.Exists(path))
                    {
                        Console.WriteLine("ok");
                    }
                    else
                    {
                        Console.WriteLine("Nok");
                    }
                }

            }
            Console.ReadLine();
        }
    }
}
