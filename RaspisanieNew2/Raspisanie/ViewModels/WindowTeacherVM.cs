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
    class WindowTeacherVM : ViewModelBase
    {
        private readonly INotifyingValue<int> index;

        private readonly INotifyCommand addCommand;
        private readonly INotifyCommand removeCommand;
        private readonly INotifyCommand editCommand;
        private readonly INotifyCommand loadCommand;

        public WindowTeacherVM(ObservableCollection<Teacher> classTeacher, ObservableCollection<Department> departments)
        {
            addCommand = this.Factory.CommandSync(Add);
            removeCommand = this.Factory.CommandSync(Remove);
            editCommand = this.Factory.CommandSync(Edit);
            loadCommand = this.Factory.CommandSync(Load);

            ClassTeacher = classTeacher;
            this.departments = departments;
            index = this.Factory.Backing(nameof(Index), -1);
        }

        private void Add()
        {
            var context = new TeacherVM(departments.ToArray());
            var wint = new NewTeacher()
            {
                DataContext = context
            };
            wint.ShowDialog();
            bool res = false;
            if (context.Teacher != null)
                res = RequestToDataBase.Instance.requestInsertIntoTeacher(context.Teacher);
            if (context.Teacher.DepartmentTwo != null)
                res = RequestToDataBase.Instance.requestInsertIntoTeacherDepartmentTwo(context.Teacher) || res;
            if (res)
            {
                ClassTeacher.Clear();
                foreach (var value in RequestToDataBase.Instance.ReadTeachers())
                    ClassTeacher.Add(value);
            }
        }

        private void Edit()
        {
            if (Index >= 0)
            {
                var teacher = ClassTeacher[Index];
                var context = new TeacherVM(teacher, departments.ToArray());
                var wint = new NewTeacher()
                {
                    DataContext = context
                };
                wint.DepTwo.Visibility = System.Windows.Visibility.Collapsed;
                wint.TextBlockDepTwo.Visibility = System.Windows.Visibility.Collapsed;
                wint.ShowDialog();
                if (context.Teacher != null)
                {
                    if (RequestToDataBase.Instance.requestUpdateTeacher(context.Teacher, ClassTeacher, Index))
                    {
                        ClassTeacher[Index] = context.Teacher;
                    }
                }
            }
        }

        private void Remove()
        {
            if (Index >= 0)
                if (RequestToDataBase.Instance.requestDeleteFromTeacher(ClassTeacher, Index))
                {
                    ClassTeacher.Clear();
                    foreach (var value in RequestToDataBase.Instance.ReadTeachers())
                        ClassTeacher.Add(value);
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
                        Console.WriteLine("Кол-во парам в документе" + parts.Length);
                        bool exist = false;
                        foreach (var teacher in ClassTeacher)
                        {
                            if (teacher.FIO.Equals(parts[0].Trim(' ')) && (teacher.Department.NameOfDepartment.Equals(parts[3].Trim(' ')) /*|| teacher.DepartmentTwo.NameOfDepartment.Equals(parts[3].Trim(' '))*/))
                            {
                                exist = true;
                            }
                        }
                        if (!exist)
                        {
                            Department Department = null;
                            foreach (var dep in departments)
                            {
                                if (dep.NameOfDepartment.Equals(parts[3].Trim(' ')))
                                {
                                    Department = dep;
                                }
                            }
                            if (Department != null)
                            {
                                Teacher teacher = new Teacher
                                {
                                    FIO = parts[0].Trim(' '),
                                    Post = parts[1].Trim(' '),
                                    Mail = parts[2].Trim(' '),
                                    Department = Department
                                };

                                if (RequestToDataBase.Instance.requestInsertIntoTeacher(teacher))
                                {
                                    ClassTeacher.Add(teacher);
                                }

                                if (parts.Length == 5)
                                {
                                    Department DepartmentTwo = null;
                                    foreach (var dep in departments)
                                    {
                                        if (dep.NameOfDepartment.Equals(parts[4].Trim(' ')))
                                        {
                                            DepartmentTwo = dep;
                                        }
                                    }
                                    if (DepartmentTwo != null)
                                    {
                                        teacher.DepartmentTwo = DepartmentTwo;
                                        if (RequestToDataBase.Instance.requestInsertIntoTeacherDepartmentTwo(teacher))
                                        {
                                            ClassTeacher.Clear();
                                            foreach (var value in RequestToDataBase.Instance.ReadTeachers())
                                                ClassTeacher.Add(value);
                                        }
                                    }

                                }
                            }
                        }
                    }
                }
            }
        }

        private ObservableCollection<Department> departments { get; }

        public ObservableCollection<Teacher> ClassTeacher { get; }

        public ICommand AddCommand => addCommand;
        public ICommand RemoveCommand => removeCommand;
        public ICommand EditCommand => editCommand;
        public ICommand LoadCommand => loadCommand;

        public int Index { get { return index.Value; } set { index.Value = value; } }
    }
}
