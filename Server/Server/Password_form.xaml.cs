using MahApps.Metro.Controls;
using Server.UserSettings;
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
using System.Windows.Shapes;

namespace Server
{
    public partial class Password_form : MetroWindow
    {
        private LdpUserSettings settings;
        public Password_form()
        {
            InitializeComponent();
            settings = new LdpUserSettings();
            ShowPassword();
        }

        private void ShowPassword()
        {
            txtPassword.Password = settings.GetPassword;
        }

        private void btnChangePassword_Click(object sender, RoutedEventArgs e)
        {
            string password = txtPassword.Password;
            if (!String.IsNullOrWhiteSpace(password))
            {
                settings.SetPassword(password);
                string success = "Password successfully changed.";
                MessageBox.Show(success, "Info", MessageBoxButton.OK,
                MessageBoxImage.Information);
                this.Close();
            }
            else
            {
                string warning = "Password contains white spaces or empty.";
                MessageBox.Show(warning, "Warning", MessageBoxButton.OK,
                MessageBoxImage.Warning);
            }
        }

        private void btnDeletepassword_Click(object sender, RoutedEventArgs e)
        {
            string password = String.Empty;
            settings.SetPassword(password);
            string success = "Password cleared.";
            MessageBox.Show(success, "Info", MessageBoxButton.OK,
            MessageBoxImage.Information);
            this.Close();
        }
    }
}
