using Server.ScreenGrabber;
using System;
using System.Collections.Generic;
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
using System.Diagnostics;
using System.IO;
using Server.WindowsUtils;
using Server.Network;
using ProtoBuf;
using Server.Properties;
using Server.LdpThreads;
using Server.ProtoGeneration;
using System.Text.RegularExpressions;

namespace Server
{
    public partial class ServerWindow : Window
    {
        public ServerWindow()
        {
            InitializeComponent();
            //CheckWindowsVersion();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //LdpProtoGenerator.GenerateProtoJava();
        }

        private void CheckWindowsVersion()
        {
            if (LdpUtils.IsWindows7)
            {
                MessageBox.Show("7");
                return;
            }
            else if (LdpUtils.IsWindows8)
            {
                MessageBox.Show("8");
                return;
            }
            else
            {
                MessageBox.Show("Unsuported windows version.\nWorks only on Windows 7 or higher..",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
                
        }
    }
}
