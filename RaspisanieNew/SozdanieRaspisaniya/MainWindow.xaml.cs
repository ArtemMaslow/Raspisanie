using System;
using System.Windows;
using System.Collections.ObjectModel;
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

        private void btnExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            ExportToExcel();
        }

        private void ExportToExcel()
        {
            dgDisplay.SelectAllCells();
            dgDisplay.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
            ApplicationCommands.Copy.Execute(null, dgDisplay);
            String resultat = (string)Clipboard.GetData(DataFormats.CommaSeparatedValue);
            String result = (string)Clipboard.GetData(DataFormats.Text);
            dgDisplay.UnselectAllCells();
            System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\Artem\Desktop\1.xls");
            file.WriteLine(result.Replace(',', ' '));
            file.Close();

            MessageBox.Show(" Exporting DataGrid data to Excel file created");
        }

        //private void LoadExcel(object sender, EventArgs e)
        //{
        //    OpenFile();
        //}
        //public void OpenFile()
        //{
        //    Excel excel = new Excel(@"C:\\Users\\Artem\\Documents\\Шаблон группы1.xlsx", 1);

        //    MessageBox.Show(excel.ReadCell(0, 2));
        //}

    }
}
