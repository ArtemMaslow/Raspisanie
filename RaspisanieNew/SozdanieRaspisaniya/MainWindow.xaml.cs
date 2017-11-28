using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Raspisanie.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SozdanieRaspisaniya
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Point startPoint;
        
        public MainWindow()
        {
            InitializeComponent();
            dropList = new ObservableCollection<TodoItem>();
            for (int i = 0; i < 76; i++)
            {
                TodoItem example = new TodoItem { NameOfSubject = "", Specifics = "", NumberOfClassroom = 0, NameOfGroup = "" };
                dropList.Add(example);
            }
            DropListt = dropList;
           
            //List<TodoItem> items = new List<TodoItem>();
            //смотрел как это будет выглядеть
            //items.Add(new TodoItem() { NameOfSubject = "Complete this WPF tutorial", Specifics = "45", NumberOfClassroom = 1, NameOfGroup = "d23" });
            //items.Add(new TodoItem() { NameOfSubject = "Complete this WPF tutorial", Specifics = "45", NumberOfClassroom = 1, NameOfGroup = "d23" });

        }
        private ObservableCollection<TodoItem> dropList;
        public ObservableCollection<TodoItem> DropListt { get; }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as ViewModel.MainVM).Close();
            this.Close();
        }


        private void List_PreviewMouseLeftButtonDownTeacher(object sender, MouseButtonEventArgs e)
        {
            // Store the mouse position
            startPoint = e.GetPosition(null);
        }

        private void List_MouseMove(object sender, MouseEventArgs e)
        {
            // Get the current mouse position
            Point mousePos = e.GetPosition(null);
            Vector diff = startPoint - mousePos;

            if (e.LeftButton == MouseButtonState.Pressed &&
                Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
            {
                // Get the dragged ListViewItem
                ListView listView = sender as ListView;
                ListViewItem listViewItem = FindAnchestor<ListViewItem>((DependencyObject)e.OriginalSource);
                
                // Find the data behind the ListViewItem
                //Teacher teacher = (Teacher)listView.ItemContainerGenerator.
                //    ItemFromContainer(listViewItem);

                //// Initialize the drag & drop operation
                //DataObject dragData = new DataObject("myFormat", teacher);
                //DragDrop.DoDragDrop(listViewItem, dragData, DragDropEffects.Move);
            }
        }

        private static T FindAnchestor<T>(DependencyObject current)
    where T : DependencyObject
        {
            do
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }
//=========================================DROP================================================
        private void DropList_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("myFormat") ||
                sender == e.Source)
            {
                e.Effects = DragDropEffects.None;
            }
        }
        private void DropList_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("myFormat"))
            {
                Teacher teacher = e.Data.GetData("myFormat") as Teacher;
                ListView listView = sender as ListView;
                listView.Items.Add(teacher);
            }
        }

        public class TodoItem
        {
            public string NameOfSubject { get; set; }
            public string Specifics { get; set; }
            public int NumberOfClassroom { get; set; }
            public string NameOfGroup { get; set; }
        }

        public enum Status
        {
            isNameOfSubject=1,
            isSpecifics,
            isNumberOfClassroom,
            isNameOfgroup
        }
    }
}
