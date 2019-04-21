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
    class WindowClassroomVM : ViewModelBase
    {
        private readonly INotifyingValue<int> index;

        private readonly INotifyCommand addCommand;
        private readonly INotifyCommand removeCommand;
        private readonly INotifyCommand editCommand;
        private readonly INotifyCommand loadCommand;

        public WindowClassroomVM(ObservableCollection<ClassRoom> classClassroom, ObservableCollection<Department> departments)
        {
            addCommand = this.Factory.CommandSync(Add);
            removeCommand = this.Factory.CommandSync(Remove);
            editCommand = this.Factory.CommandSync(Edit);
            loadCommand = this.Factory.CommandSync(Load);

            ClassClassroom = classClassroom;
            this.departments = departments;

            index = this.Factory.Backing(nameof(Index), -1);
        }

        private void Add()
        {
            var context = new ClassroomVM(departments.ToArray());
            var wind = new NewClassroom()
            {
                DataContext = context
            };
            wind.ShowDialog();
            if (wind.DialogResult == true)
            {
                if (context.ClassRoom != null)
                    if (RequestToDataBase.Instance.requestInsertIntoClassroom(context.ClassRoom))
                    {
                        ClassClassroom.Add(context.ClassRoom);
                    }
            }
        }

        private void Edit()
        {
            if (Index >= 0)
            {
                var classroom = ClassClassroom[Index];
                var context = new ClassroomVM(classroom, departments.ToArray());
                var wind = new NewClassroom()
                {
                    DataContext = context
                };
                wind.ShowDialog();
                if (wind.DialogResult == true)
                {
                    if (context.ClassRoom != null)
                    {
                        if (RequestToDataBase.Instance.requestUpdateClassroom(context.ClassRoom, ClassClassroom, Index))
                        {
                            ClassClassroom[Index] = context.ClassRoom;
                        }
                    }
                }
            }
        }

        private void Remove()
        {
            if (Index >= 0)
                if (RequestToDataBase.Instance.requestDeleteFromClassroom(ClassClassroom, Index))
                {
                    ClassClassroom.RemoveAt(Index);
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
                        foreach (var classroom in ClassClassroom)
                        {
                            if (classroom.NumberOfClassroom.Equals(parts[0].Trim(' ')))
                            {
                                exist = true;
                            }
                        }
                        if (!exist)
                        {
                            Department Department = null;
                            foreach (var dep in departments)
                            {
                                if (dep.NameOfDepartment.Equals(parts[2].Trim(' ')))
                                {
                                    Department = dep;
                                }
                            }
                            if (Department != null)
                            {
                                ClassRoom classroom = new ClassRoom
                                {
                                    NumberOfClassroom = parts[0].Trim(' '),
                                    Specific = parts[1].Trim(' '),
                                    Department = Department
                                };

                                if (RequestToDataBase.Instance.requestInsertIntoClassroom(classroom))
                                {
                                    ClassClassroom.Add(classroom);
                                }
                            }
                        }
                    }
                }
            }
        }

        private ObservableCollection<Department> departments { get; }

        public ObservableCollection<ClassRoom> ClassClassroom { get; }

        public ICommand AddCommand => addCommand;
        public ICommand RemoveCommand => removeCommand;
        public ICommand EditCommand => editCommand;
        public ICommand LoadCommand => loadCommand;

        public int Index { get { return index.Value; } set { index.Value = value; } }
    }
}
