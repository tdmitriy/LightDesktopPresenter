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
using Server.Properties;
using Server.ProtoGeneration;
using System.Text.RegularExpressions;
using MahApps.Metro.Controls;

namespace Server
{
    public partial class ServerWindow : MetroWindow
    {
        private LdpServer server;
        string text = "Try to use one of the folowing ip to connect:\n" +
                        "192.168.0.48\n";
        public ServerWindow()
        {
            InitializeComponent();
            server = LdpServer.GetInstance();
            //LdpUtils.CheckStartupWindowsVersion(this);
            lblConnectionStatus.Text = text;
            server.Start();
            //LdpProtoGenerator.GenerateProtoJava();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            /*server.Start();
            txt.Text = "";
            txt.Text = "Started";*/
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            server.Stop();
        }

        private void btnAbout_Click(object sender, RoutedEventArgs e)
        {
            About about = new About();
            about.ShowDialog();
        }

        private void btnSetPassword_Click(object sender, RoutedEventArgs e)
        {
            Password_form password_form = new Password_form();
            password_form.ShowDialog();
        }
    }
}
