using Raspisanie.Models;
using System;
using System.Windows.Input;
using ViewModule;
using ViewModule.CSharp;
using System.Windows;
using Raspisanie;
using FirebirdSql.Data.FirebirdClient;
using static ViewModule.Validation.CSharp.Validators;
using System.ComponentModel;

namespace Raspisanie.ViewModels
{
    class FacultyVM : ViewModelBase
    {
        private readonly INotifyingValue<int> codeOfFaculty;
        private readonly INotifyingValue<string> nameOfFaculty;
        private readonly INotifyCommand saveFaculty;

        public FacultyVM()
        {
            nameOfFaculty = this.Factory.Backing(nameof(NameOfFaculty), "", NotNullOrWhitespace.Then(HasLengthNotLongerThan(35)));
            codeOfFaculty = this.Factory.Backing(nameof(CodeOfFaculty), 0);

            saveFaculty = this.Factory.CommandSyncParam<Window>(SaveAndClose);
        }

        public FacultyVM(Faculty faculty) : this()
        {
            codeOfFaculty.Value = faculty.CodeOfFaculty;
            nameOfFaculty.Value = faculty.NameOfFaculty;
        }

        private void SaveAndClose(Window obj)
        {
            if (!string.IsNullOrWhiteSpace(NameOfFaculty))
            {
                Faculty = new Faculty
                {
                    NameOfFaculty = NameOfFaculty,
                    CodeOfFaculty = CodeOfFaculty
                };
                obj.DialogResult = true;
                obj.Close();
            }
        }

        public ICommand SaveCommand => saveFaculty;
        public int CodeOfFaculty { get { return codeOfFaculty.Value; } set { codeOfFaculty.Value = value; } }
        public string NameOfFaculty { get { return nameOfFaculty.Value; } set { nameOfFaculty.Value = value; } }

        public Faculty Faculty
        {
            get; private set;
        }

    }
}
