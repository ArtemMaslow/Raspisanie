using Microsoft.Win32;
using Raspisanie.Models;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using ViewModule;
using ViewModule.CSharp;

namespace Raspisanie.ViewModels
{
    class WindowDepartmentVM : ViewModelBase
    {
        private readonly INotifyingValue<int> index;

        private readonly INotifyCommand addCommand;
        private readonly INotifyCommand removeCommand;
        private readonly INotifyCommand editCommand;
        private readonly INotifyCommand loadCommand;

        public WindowDepartmentVM(ObservableCollection<Department> classDepartment, ObservableCollection<Faculty> facultities)
        {
            addCommand = this.Factory.CommandSync(Add);
            removeCommand = this.Factory.CommandSync(Remove);
            editCommand = this.Factory.CommandSync(Edit);
            loadCommand = this.Factory.CommandSync(Load);

            ClassDepartment = classDepartment;
            this.facultities = facultities;
            index = this.Factory.Backing(nameof(Index), -1);
        }

        private void Add()
        {
            var context = new DepartmentVM(facultities.ToArray());
            var wind = new NewDepartment()
            {
                DataContext = context
            };
            wind.ShowDialog();
            System.Console.WriteLine(context.Department != null);
            if (context.Department != null)
            {
                if (RequestToDataBase.Instance.requestInsertIntoDepartment(context.Department))
                {
                    ClassDepartment.Add(context.Department);
                }
            }
        }

        private void Edit()
        {
            if (Index >= 0)
            {
                var department = ClassDepartment[Index];
                var context = new DepartmentVM(department, facultities.ToArray());
                var wind = new NewDepartment()
                {
                    DataContext = context
                };
                wind.ShowDialog();
                if (context.Department != null)
                {
                    if (RequestToDataBase.Instance.requestUpdateDepartment(context.Department, ClassDepartment, Index))
                        ClassDepartment[Index] = context.Department;
                }
            }
        }

        private void Remove()
        {
            if (Index >= 0)
                if (RequestToDataBase.Instance.requestDeleteFromDepartment(ClassDepartment, Index))
                {
                    ClassDepartment.RemoveAt(Index);
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
            if (File.Exists(pathToCsv))
            {
                char[] delimiters = new char[] { ',' };
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
                        foreach(var department in ClassDepartment)
                        {
                            if (department.NameOfDepartment.Equals(parts[0].Trim(' ')))
                            {
                                exist = true;
                            }
                        }
                        if (!exist)
                        {
                            Faculty Faculty = null;
                            foreach (var faculty in facultities)
                            {
                                if (faculty.NameOfFaculty.Equals(parts[1].Trim(' ')))
                                {
                                    Faculty = faculty;
                                }
                            }
                            if (Faculty != null)
                            {
                                Department department = new Department
                                {
                                    NameOfDepartment = parts[0].Trim(' '),
                                    Faculty = Faculty
                                };

                                if (RequestToDataBase.Instance.requestInsertIntoDepartment(department))
                                {
                                    ClassDepartment.Add(department);
                                }
                            }
                        }
                    }
                }
            }
        }
        private ObservableCollection<Faculty> facultities { get; }

        public ObservableCollection<Department> ClassDepartment { get; }

        public ICommand AddCommand => addCommand;
        public ICommand RemoveCommand => removeCommand;
        public ICommand EditCommand => editCommand;
        public ICommand LoadCommand => loadCommand;

        public int Index { get { return index.Value; } set { index.Value = value; } }
    }
}
