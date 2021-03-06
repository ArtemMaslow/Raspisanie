﻿using Models;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ViewModule;
using ViewModule.CSharp;
using static ViewModule.Validation.CSharp.Validators;

namespace Raspisanie.ViewModels
{
    class DepartmentVM : ViewModelBase
    {
        private readonly INotifyingValue<int> codeOfDepartment;
        private readonly INotifyingValue<string> nameOfDepartment;
        private readonly INotifyingValue<Faculty> faculty;

        private readonly INotifyCommand saveDepartment;

        public DepartmentVM(Faculty[] facultities)
        {
            Facultities = facultities;

            codeOfDepartment = this.Factory.Backing(nameof(CodeOfDepartment), 0);
            nameOfDepartment = this.Factory.Backing(nameof(NameOfDepartment), "", NotNullOrWhitespace.Then(HasLengthNotLongerThan(50)));
            faculty = this.Factory.Backing(nameof(Faculty), null, ContainedWithin(Facultities));

            saveDepartment = this.Factory.CommandSyncParam<Window>(SaveAndClose);
        }

        public DepartmentVM(Department department, Faculty[] facultities) : this(facultities)
        {
            codeOfDepartment.Value = department.CodeOfDepartment;
            nameOfDepartment.Value = department.NameOfDepartment;
            faculty.Value = facultities.Single(d => d.CodeOfFaculty == department.Faculty.CodeOfFaculty);
        }

        private void SaveAndClose(Window obj)
        {
            if (!string.IsNullOrWhiteSpace(NameOfDepartment) && Faculty != null)
            {
                Department = new Department
                {
                    CodeOfDepartment = CodeOfDepartment,
                    NameOfDepartment = NameOfDepartment,
                    Faculty = Faculty
                };
                obj.DialogResult = true;
                obj.Close();
            }
        }
        public ICommand SaveCommand => saveDepartment;
        public int CodeOfDepartment { get { return codeOfDepartment.Value; } set { codeOfDepartment.Value = value; } }
        public string NameOfDepartment { get { return nameOfDepartment.Value; } set { nameOfDepartment.Value = value; } }
        public Faculty Faculty { get { return faculty.Value; } set { faculty.Value = value; } }

        public Department Department
        {
            get; private set;
        }

        public Faculty[] Facultities { get; }
    }
}
