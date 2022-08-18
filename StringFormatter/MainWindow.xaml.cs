using StringFormatter.Models;
using StringFormatter.Mvvm;
using StringFormatter.Services;
using StringFormatter.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace StringFormatter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
            FormatterStore = new ObservableCollection<IStringFormatter>();
            UsedFormatter = new ObservableCollection<IStringFormatter>();
            FormatterStore.Add(new FixCommaFormatter());
            FormatterStore.Add(new FixMisspellFormatter());
            FormatterStore.Add(new IdentifyFormulasFomatter());
            FormatterStore.Add(new UnderlineEscapeFormatter());
            FormatterStore.Add(new MathrmFormatter());
            DeleteCommand = new RelayCommand<object>(Delete);
            this.DataContext = this;
        }

        public DropHandlerClass DropHandler { get; set; } = new DropHandlerClass();
        public ObservableCollection<IStringFormatter> FormatterStore { get; set; }
        public ObservableCollection<IStringFormatter> UsedFormatter { get; set; }
        public RelayCommand<object> DeleteCommand { get; set; }

        private string input;
        public string Input
        {
            get { return input; }
            set
            {
                input = value;
                this.RaisePropertyChanged("Input");
            }
        }
        private string output;
        public string Output
        {
            get { return output; }
            set
            {
                output = value;
                this.RaisePropertyChanged("Output");
            }
        }
        private string message;
        public string Message
        {
            get { return message; }
            set
            {
                message = value;
                this.RaisePropertyChanged("Message");
            }
        }


        public void Delete(object arg)
        {
            var item = arg as IStringFormatter;
            if (item == null) return;
            UsedFormatter.Remove(item);
        }

        private void ButtonGo_Click(object sender, RoutedEventArgs e)
        {
            if (Input == null)
            {
                Message = "输入为空";
                return;
            }
            var tmp = Input;
            foreach (var formatter in UsedFormatter)
            {
                tmp = formatter.Process(tmp);
            }
            Output = tmp;
            Clipboard.SetText(Output);
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {

        }

        #region INotifyPropertyChanged members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        #endregion


    }
}
