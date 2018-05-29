using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace SozdanieRaspisaniya
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
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
