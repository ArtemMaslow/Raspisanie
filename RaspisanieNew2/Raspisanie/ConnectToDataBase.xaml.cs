﻿using System.Windows;

namespace Raspisanie
{
    public partial class ConnectToDataBase : Window
    {
        public bool Result { get; private set; }
        public ConnectToDataBase()
        {       
            InitializeComponent();        
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Result = true;
            this.Close();
        }
    }
}
