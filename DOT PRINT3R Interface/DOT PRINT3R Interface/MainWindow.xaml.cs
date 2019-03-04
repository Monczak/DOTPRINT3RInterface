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

using Core;

namespace DOT_PRINT3R_Interface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoadFileBtn_Click(object sender, RoutedEventArgs e)
        {
            FileLoader.LoadImage();
        }

        private void SendFileBtn_Click(object sender, RoutedEventArgs e)
        {
            FileConverter.ConvertFile("dummy");
            FileSender.ConnectAndSend();
        }
    }
}
