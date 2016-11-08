using System;
using System.IO;
using System.Text;
using System.Windows;


namespace callKeeper
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
           // InitializeComponent();
            callKeeper.cWebRequest.saveRecordCall();
            this.Close();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {

           
        }
    }
}
