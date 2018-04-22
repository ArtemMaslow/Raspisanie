﻿using Raspisanie.Models;
using System.Collections.ObjectModel;
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

        public WindowDepartmentVM(ObservableCollection<Department> classDepartment, ObservableCollection<Faculty> facultities )
        {
            addCommand = this.Factory.CommandSync(Add);
            removeCommand = this.Factory.CommandSync(Remove);
            editCommand = this.Factory.CommandSync(Edit);

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
                    if(RequestToDataBase.Instance.requestUpdateDepartment(context.Department,ClassDepartment,Index))
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

        private ObservableCollection<Faculty> facultities { get; }

        public ObservableCollection<Department> ClassDepartment { get; }

        public ICommand AddCommand => addCommand;
        public ICommand RemoveCommand => removeCommand;
        public ICommand EditCommand => editCommand;

        public int Index { get { return index.Value; } set { index.Value = value; } }
    }
}
