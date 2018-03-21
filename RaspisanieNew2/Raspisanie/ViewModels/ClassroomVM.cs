using Raspisanie.Models;
using System;
using System.Windows.Input;
using ViewModule;
using ViewModule.CSharp;
using System.Windows;
using System.Linq;

namespace Raspisanie.ViewModels
{
    class ClassroomVM : ViewModelBase
    {
        private readonly INotifyingValue<string> numberOfClassroom;
        private readonly INotifyingValue<int> capacity;
        private readonly INotifyingValue<int> codeOfClassroom;
        private readonly INotifyingValue<string> specifics;
        private readonly INotifyingValue<Department> department;

        private readonly INotifyCommand saveClassroom;

       

        public ClassroomVM(Department[] departments)
        {
            Departments = departments;

            numberOfClassroom = this.Factory.Backing(nameof(NumberOfClassroom),"");
            capacity = this.Factory.Backing(nameof(Capacity), 0);
            codeOfClassroom = this.Factory.Backing(nameof(CodeOfClassroom), 0);
            specifics = this.Factory.Backing(nameof(Specifics),"");
            department = this.Factory.Backing<Department>(nameof(Department), null);
            saveClassroom = this.Factory.CommandSyncParam<Window>(SaveAndClose);
        }

        public ClassroomVM(ClassRoom classroom, Department[] departments) : this(departments)
        {
            numberOfClassroom.Value = classroom.NumberOfClassroom;
            codeOfClassroom.Value = classroom.CodeOfClassroom;
            specifics.Value = classroom.Specifics;
            department.Value = departments.Single(d => d.CodeOfDepartment == classroom.CodeOfDepartment);
        }

        private void SaveAndClose(Window obj)
        {
            if (!string.IsNullOrWhiteSpace(Specifics) && CodeOfClassroom > 0 && Capacity > 0 && !string.IsNullOrWhiteSpace(NumberOfClassroom))
                ClassRoom = new ClassRoom {
                    NumberOfClassroom = NumberOfClassroom,
                    CodeOfClassroom = CodeOfClassroom,
                    Specifics = Specifics,
                    CodeOfDepartment = Department.CodeOfDepartment,
                };
            obj.Close();
        }

        public ICommand SaveCommand => saveClassroom;
        public string NumberOfClassroom { get { return numberOfClassroom.Value; } set { numberOfClassroom.Value = value; } }
        public int Capacity { get { return capacity.Value; } set { capacity.Value = value; } }
        public int CodeOfClassroom { get { return codeOfClassroom.Value; } set { codeOfClassroom.Value = value; } }
        public string Specifics { get { return specifics.Value; } set { specifics.Value = value; } }
        public Department Department { get { return department.Value; } set { department.Value = value; } }

        public ClassRoom ClassRoom
        {
            get; private set;
        }

        public Department[] Departments { get; }
    }
}
