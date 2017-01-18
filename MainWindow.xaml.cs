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
            var args = Environment.GetCommandLineArgs();
            if (args.Length <= 1) {
                MessageBox.Show("Не задан параметр для запуска", "Параметр", MessageBoxButton.OK, MessageBoxImage.Warning);
                this.Close();
            }
            else
            {
                callKeeper.cWebRequest.saveRecordCall(args[1]);
                this.Close();
            }
            

            
        }
        private void button_Click(object sender, RoutedEventArgs e)
        {

           
        }
    }
}
