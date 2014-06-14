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
        private LdpLabelStatus labelStatus;
        public ServerWindow()
        {
            
            InitializeComponent();
            server = LdpServer.GetInstance();
            labelStatus = LdpLabelStatus.GetInstance();
            lblConnectionStatus.DataContext = labelStatus;
            //LdpUtils.CheckStartupWindowsVersion(this);
            StartServer();

            //Binding LdpDisplayedConnectionInfo to label
            
            
            //LdpProtoGenerator.GenerateProtoJava();
        }

        private void StartServer()
        {
            if (server != null)
                server.Start();
        }

        private void StopServer()
        {
            if (server != null)
                server.Stop();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            StopServer();
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
