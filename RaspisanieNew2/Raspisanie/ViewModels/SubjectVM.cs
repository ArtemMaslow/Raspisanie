﻿using Models;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ViewModule;
using ViewModule.CSharp;
using static ViewModule.Validation.CSharp.Validators;
namespace Raspisanie.ViewModels
{
    class SubjectVM : ViewModelBase
    {
        private readonly INotifyingValue<int> codeOfSubject;
        private readonly INotifyingValue<string> nameOfSubject;
        private readonly INotifyingValue<Department> department;

        private readonly INotifyCommand saveSubject;

        public SubjectVM(Department[] departments)
        {
            Departments = departments;

            codeOfSubject = this.Factory.Backing(nameof(CodeOfSubject), 0);
            nameOfSubject = this.Factory.Backing(nameof(NameOfSubject), "", NotNullOrWhitespace.Then(HasLengthNotLongerThan(100)));
            department = this.Factory.Backing(nameof(Department), null, ContainedWithin(Departments));

            saveSubject = this.Factory.CommandSyncParam<Window>(SaveAndClose);
        }

        public SubjectVM(Subject subject, Department[] departments) : this(departments)
        {
            codeOfSubject.Value = subject.CodeOfSubject;
            nameOfSubject.Value = subject.NameOfSubject;
            department.Value = departments.Single(s => s.CodeOfDepartment == subject.Department.CodeOfDepartment);
        }

        private void SaveAndClose(Window obj)
        {
            if (!string.IsNullOrWhiteSpace(NameOfSubject) && Department != null)
            {
                Subject = new Subject
                {
                    CodeOfSubject = CodeOfSubject,
                    NameOfSubject = NameOfSubject,
                    Department = Department
                };
                obj.DialogResult = true;
                obj.Close();
            }
        }

        public ICommand SaveCommand => saveSubject;
        public int CodeOfSubject { get { return codeOfSubject.Value; } set { codeOfSubject.Value = value; } }
        public string NameOfSubject { get { return nameOfSubject.Value; } set { nameOfSubject.Value = value; } }
        public Department Department { get { return department.Value; } set { department.Value = value; } }

        public Subject Subject
        {
            get; private set;
        }

        public Department[] Departments { get; }
    }
}
