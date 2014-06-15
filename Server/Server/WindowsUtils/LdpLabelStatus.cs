using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.WindowsUtils
{
    public class LdpLabelStatus : INotifyPropertyChanged 
    {
        private LdpLabelStatus() { }

        public static LdpLabelStatus GetInstance()
        {
            return LdpSingletonInstance.singleInstance;
        }

        class LdpSingletonInstance
        {
            internal static readonly LdpLabelStatus singleInstance 
                = new LdpLabelStatus();
            static LdpSingletonInstance() { }
        }

        private string stateText;
        public string StateText
        {
            get { return stateText; }
            set
            {
                stateText = value;
                OnPropertyChanged("StateText");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
