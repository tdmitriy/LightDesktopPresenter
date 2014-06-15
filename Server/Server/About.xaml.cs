using MahApps.Metro.Controls;
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
   
    public partial class About : MetroWindow
    {
        public About()
        {
            InitializeComponent();
            SetAbountText();
        }

        private void SetAbountText()
        {
            string text = "This diploma project was created by the student\n";
            text += "of Zaporizhzhya State Engineering Academy\n";
            text += "Group : SP-13-1M\n";
            text += "Developer : Tatarenko Dmitriy Olegovich";

            txtAbout.Text = text;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
