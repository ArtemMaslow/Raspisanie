using Microsoft.Win32;
using Models;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using ViewModule;
using ViewModule.CSharp;

namespace Raspisanie.ViewModels
{
    class WindowFacultyVM : ViewModelBase
    {
        private readonly INotifyingValue<int> index;

        private readonly INotifyCommand addCommand;
        private readonly INotifyCommand removeCommand;
        private readonly INotifyCommand editCommand;
        private readonly INotifyCommand loadCommand;

        public WindowFacultyVM(ObservableCollection<Faculty> classFaculty)
        {
            addCommand = this.Factory.CommandSync(Add);
            removeCommand = this.Factory.CommandSync(Remove);
            editCommand = this.Factory.CommandSync(Edit);
            loadCommand = this.Factory.CommandSync(Load);
            ClassFaculty = classFaculty;

            index = this.Factory.Backing(nameof(Index), -1);
        }

        private void Add()
        {
            var context = new FacultyVM();
            var wind = new NewFaculty()
            {
                DataContext = context
            };
            wind.ShowDialog();
            if (wind.DialogResult == true)
            {
                if (context.Faculty != null)
                {
                    if (RequestToDataBase.Instance.requestInsertIntoFaculty(context.Faculty))
                    {
                        ClassFaculty.Add(context.Faculty);
                    }
                }
            }
        }

        private void Edit()
        {
            if (Index >= 0)
            {
                var faculty = ClassFaculty[Index];
                var context = new FacultyVM(faculty);
                var wind = new NewFaculty()
                {
                    DataContext = context
                };
                wind.ShowDialog();
                if (wind.DialogResult == true)
                {
                    if (context.Faculty != null)
                    {
                        if (RequestToDataBase.Instance.requestUpdateFaculty(context.Faculty, ClassFaculty, Index))
                        {
                            ClassFaculty[Index] = context.Faculty;
                        }
                    }
                }
            }
        }

        private void Remove()
        {
            if (Index >= 0)
            {
                if (RequestToDataBase.Instance.requestDeleteFromFaculty(ClassFaculty, Index))
                {
                    ClassFaculty.RemoveAt(Index);
                }
            }
        }

        private void Load()
        {
            string pathToCsv = "";
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Файл csv|*.csv";
            if (openFileDialog.ShowDialog() == true)
            {
                pathToCsv = openFileDialog.FileName;
            }
            if (!String.IsNullOrEmpty(pathToCsv))
            {
                char[] delimiters = new char[] { ';' };
                using (StreamReader reader = new StreamReader(pathToCsv, System.Text.Encoding.Default))
                {
                    while (true)
                    {
                        string line = reader.ReadLine();
                        if (line == null)
                        {
                            break;
                        }
                        string[] parts = line.Split(delimiters);
                        bool exist = false;
                        foreach (var faculty in ClassFaculty)
                        {
                            if (faculty.NameOfFaculty.Equals(parts[0].Trim(' ')))
                            {
                                exist = true;
                            }
                        }
                        if (!exist)
                        {
                            Faculty faculty = new Faculty { NameOfFaculty = parts[0].Trim(' ') };
                            if (RequestToDataBase.Instance.requestInsertIntoFaculty(faculty))
                            {
                                ClassFaculty.Add(faculty);
                            }
                        }
                    }
                    reader.Close();
                }
            }
        }

        public ObservableCollection<Faculty> ClassFaculty { get; }
        public ICommand AddCommand => addCommand;
        public ICommand RemoveCommand => removeCommand;
        public ICommand EditCommand => editCommand;
        public ICommand LoadCommand => loadCommand;

        public int Index { get { return index.Value; } set { index.Value = value; } }
    }
}
