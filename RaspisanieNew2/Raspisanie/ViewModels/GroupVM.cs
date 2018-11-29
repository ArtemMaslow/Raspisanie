using Raspisanie.Models;
using System.Windows.Input;
using ViewModule;
using ViewModule.CSharp;
using System.Windows;
using System.Linq;
using static ViewModule.Validation.CSharp.Validators;

namespace Raspisanie.ViewModels
{
    public class GroupVM : ViewModelBase
    {
        private readonly INotifyingValue<string> nameOfGroup;
        private readonly INotifyingValue<int> codeOfGroup;
        private readonly INotifyingValue<Department> department;
        private readonly INotifyingValue<int> semestr;

        private readonly INotifyCommand saveGroup;

        public GroupVM(Department[] departments)
        {
            Departments = departments;
            NumberSemestr = new int[] { 1, 2 };

            nameOfGroup = this.Factory.Backing(nameof(NameOfGroup), "", NotNullOrWhitespace.Then(HasLengthNotLongerThan(50)));
            codeOfGroup = this.Factory.Backing(nameof(CodeOfGroup), 0);
            semestr = this.Factory.Backing(nameof(Semestr), 0);
            department = this.Factory.Backing<Department>(nameof(Department),null);

            saveGroup = this.Factory.CommandSyncParam<Window>(SaveAndClose);
        }

        public GroupVM(Group group, Department[] departments) : this(departments)
        {
            nameOfGroup.Value = group.NameOfGroup;
            codeOfGroup.Value = group.CodeOfGroup;
            semestr.Value = NumberSemestr.Single(s=>s == group.Semester);
            department.Value = departments.Single(d=>d.CodeOfDepartment==group.Department.CodeOfDepartment);
        }

        private void SaveAndClose(Window obj)
        {
            if (!string.IsNullOrWhiteSpace(NameOfGroup) 
                && Department!=null && (Semestr == 1 || Semestr==2))
                Group = new Group {
                    NameOfGroup = NameOfGroup,
                    CodeOfGroup = CodeOfGroup,
                    Department = Department,
                    Semester = Semestr
                };
            obj.Close();
        }

        public ICommand SaveCommand => saveGroup;
        
        public string NameOfGroup { get { return nameOfGroup.Value; } set { nameOfGroup.Value = value; } }
        public int CodeOfGroup { get { return codeOfGroup.Value; } set { codeOfGroup.Value = value; } }
        public Department Department { get {return department.Value; } set { department.Value = value; } }
        public int Semestr { get { return semestr.Value; } set { semestr.Value = value; } }

        public Group Group
        {
            get; private set;
        }

        public Department[] Departments { get; }
        public int[] NumberSemestr { get; set; }
    }
}
