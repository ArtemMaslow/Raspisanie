using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Raspisanie.Models;
using System.Collections.Generic;

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
        
        }
}
