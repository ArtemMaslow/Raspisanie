using Raspisanie.Models;
using System;
using System.Windows.Input;
using ViewModule;
using ViewModule.CSharp;
using System.Windows;
using System.Linq;
using static ViewModule.Validation.CSharp.Validators;

namespace Raspisanie.ViewModels
{
    class ClassroomVM : ViewModelBase
    {
        private readonly INotifyingValue<string> numberOfClassroom;
        private readonly INotifyingValue<int> codeOfClassroom;
        private readonly INotifyingValue<string> specifics;
        private readonly INotifyingValue<Department> department;

        private readonly INotifyCommand saveClassroom;

       

        public ClassroomVM(Department[] departments)
        {
            Departments = departments;

            numberOfClassroom = this.Factory.Backing(nameof(NumberOfClassroom),"", NotNullOrWhitespace.Then(HasLengthNotLongerThan(10)));
            codeOfClassroom = this.Factory.Backing(nameof(CodeOfClassroom), 0);
            specifics = this.Factory.Backing(nameof(Specifics),"", NotNullOrWhitespace.Then(HasLengthNotLongerThan(20)));
            department = this.Factory.Backing<Department>(nameof(Department.CodeOfDepartment), null);
            saveClassroom = this.Factory.CommandSyncParam<Window>(SaveAndClose);
        }

        public ClassroomVM(ClassRoom classroom, Department[] departments) : this(departments)
        {
            numberOfClassroom.Value = classroom.NumberOfClassroom;
            codeOfClassroom.Value = classroom.CodeOfClassroom;
            specifics.Value = classroom.Specifics;
            department.Value = departments.Single(f => f.CodeOfDepartment == classroom.Department.CodeOfDepartment);
        }

        private void SaveAndClose(Window obj)
        {
            if (!string.IsNullOrWhiteSpace(Specifics) && CodeOfClassroom > 0 && !string.IsNullOrWhiteSpace(NumberOfClassroom))
                ClassRoom = new ClassRoom {
                    NumberOfClassroom = NumberOfClassroom,
                    CodeOfClassroom = CodeOfClassroom,
                    Specifics = Specifics,
                    Department = Department
                };
            obj.Close();
        }

        public ICommand SaveCommand => saveClassroom;
        public string NumberOfClassroom { get { return numberOfClassroom.Value; } set { numberOfClassroom.Value = value; } }
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
