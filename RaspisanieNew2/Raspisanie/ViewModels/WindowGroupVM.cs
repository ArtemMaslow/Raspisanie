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
    class WindowGroupVM : ViewModelBase
    {
        private readonly INotifyingValue<int> index;

        private readonly INotifyCommand addCommand;
        private readonly INotifyCommand removeCommand;
        private readonly INotifyCommand editCommand;
        private readonly INotifyCommand loadCommand;

        public WindowGroupVM(ObservableCollection<Group> classGroup,
            ObservableCollection<Department> departments)
        {
            addCommand = this.Factory.CommandSync(Add);
            removeCommand = this.Factory.CommandSync(Remove);
            editCommand = this.Factory.CommandSync(Edit);
            loadCommand = this.Factory.CommandSync(Load);

            ClassGroups = classGroup;
            this.departments = departments;

            index = this.Factory.Backing(nameof(Index), -1);
        }

        private void Add()
        {
            var context = new GroupVM(departments.ToArray());
            var wing = new NewGroup()
            {
                DataContext = context
            };
            wing.ShowDialog();
            System.Console.WriteLine(context.Group != null);
            if (context.Group != null)
                if (RequestToDataBase.Instance.requestInsertIntoGroup(context.Group))
                {
                    ClassGroups.Add(context.Group);
                }
        }

        private void Edit()
        {
            if (Index >= 0)
            {
                var group = ClassGroups[Index];
                var context = new GroupVM(group,departments.ToArray());
                var wind = new NewGroup()
                {
                    DataContext = context
                };
                wind.ShowDialog();
                if (context.Group != null)
                {
                    if (RequestToDataBase.Instance.requestUpdateGroup(context.Group, ClassGroups, Index))
                    {
                        ClassGroups[Index] = context.Group;
                    }
                }
            }
        }

        private void Remove()
        {
            if (Index >= 0)
                if (RequestToDataBase.Instance.requestDeleteFromGroup(ClassGroups, Index))
                {
                    ClassGroups.RemoveAt(Index);
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
                        foreach (var group in ClassGroups)
                        {
                            if (group.NameOfGroup.Equals(parts[0].Trim(' ')))
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
                                Group group = new Group
                                {
                                    NameOfGroup = parts[0].Trim(' '),
                                    Term = Convert.ToInt32(parts[1].Trim(' ')),
                                    Department = Department
                                };

                                if (RequestToDataBase.Instance.requestInsertIntoGroup(group))
                                {
                                    ClassGroups.Add(group);
                                }
                            }
                        }
                    }
                }
            }
        }

        private ObservableCollection<Department> departments { get; }

        public ObservableCollection<Group> ClassGroups { get; }

        public ICommand AddCommand => addCommand;
        public ICommand RemoveCommand => removeCommand;
        public ICommand EditCommand => editCommand;
        public ICommand LoadCommand => loadCommand;

        public int Index { get { return index.Value; } set { index.Value = value; } }
    }
}
