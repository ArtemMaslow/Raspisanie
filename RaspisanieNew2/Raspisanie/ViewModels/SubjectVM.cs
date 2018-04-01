using Raspisanie.Models;
using System.Windows.Input;
using ViewModule;
using ViewModule.CSharp;
using System.Windows;
using System.Linq;

namespace Raspisanie.ViewModels
{
    class SubjectVM : ViewModelBase
    {
        private readonly INotifyingValue<int> codeOfSubject;
        private readonly INotifyingValue<string> nameOfSubject;
        private readonly INotifyingValue<string> specifics;
        private readonly INotifyingValue<int> commonHours;
        private readonly INotifyingValue<int> lectureHours;
        private readonly INotifyingValue<int> laboratoryHours;
        private readonly INotifyingValue<int> exerciseHours;
        private readonly INotifyingValue<Department> department;

        private readonly INotifyCommand saveSubject;

        public SubjectVM(Department[] departments)
        {
            Departments = departments;

            codeOfSubject = this.Factory.Backing(nameof(CodeOfSubject), 0);
            nameOfSubject = this.Factory.Backing(nameof(NameOfSubject), "");
            specifics = this.Factory.Backing(nameof(Specifics), "");
            commonHours = this.Factory.Backing(nameof(CommonHours), 0);
            lectureHours = this.Factory.Backing(nameof(LectureHours), 0);
            laboratoryHours = this.Factory.Backing(nameof(LaboratoryHours), 0);
            exerciseHours = this.Factory.Backing(nameof(ExerciseHours), 0);
            department = this.Factory.Backing<Department>(nameof(Department), null);

            saveSubject = this.Factory.CommandSyncParam<Window>(SaveAndClose);
        }

        public SubjectVM(Subject subject, Department[] departments) : this(departments)
        {
            codeOfSubject.Value = subject.CodeOfSubject;
            nameOfSubject.Value = subject.NameOfSubject;
           // specifics.Value = subject.Specifics;
            department.Value = departments.Single(s => s.CodeOfDepartment == subject.CodeOfDepartment);
        }

        private void SaveAndClose(Window obj)
        {
            if (!string.IsNullOrWhiteSpace(NameOfSubject)
                && LectureHours > 0
                && (LaboratoryHours > 0 || ExerciseHours > 0)
                && Department != null)
                Subject = new Subject
                {
                    CodeOfSubject = CodeOfSubject,
                    NameOfSubject = NameOfSubject,
                    //Specifics = Specifics,
                    CodeOfDepartment = Department.CodeOfDepartment
                };
            obj.Close();
        }

        public ICommand SaveCommand => saveSubject;
        public int CodeOfSubject { get { return codeOfSubject.Value; } set { codeOfSubject.Value = value; } }
        public string NameOfSubject { get { return nameOfSubject.Value; } set { nameOfSubject.Value = value; } }
        public string Specifics { get { return specifics.Value; } set { specifics.Value = value; } }
        public int CommonHours { get { return commonHours.Value = LectureHours+LaboratoryHours+ExerciseHours ; } set { commonHours.Value = value; } }
        public int LectureHours { get { return lectureHours.Value; } set { lectureHours.Value = value; } }
        public int LaboratoryHours { get { return laboratoryHours.Value; } set { laboratoryHours.Value = value; } }
        public int ExerciseHours { get { return exerciseHours.Value; } set { exerciseHours.Value = value; } }
        public Department Department { get { return department.Value; } set { department.Value = value; } }

        public Subject Subject
        {
            get; private set;
        }

        public Department[] Departments { get; }
    }
}
