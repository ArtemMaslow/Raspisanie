using System.Windows;

namespace Raspisanie
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as ViewModels.MainVM).Close();
            this.Close();
        }
    }
}
