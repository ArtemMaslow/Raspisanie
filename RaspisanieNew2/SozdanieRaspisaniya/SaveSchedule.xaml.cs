using System.Windows;

namespace SozdanieRaspisaniya
{
    public partial class SaveSchedule : Window
    {
        public SaveSchedule()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }
    }
}
