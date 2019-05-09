using System.Windows;

namespace SozdanieRaspisaniya
{

    public partial class MainWindow : Window
    {
      
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as ViewModel.MainVM).Close();
            this.Close();
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
           
        }
    }
}
