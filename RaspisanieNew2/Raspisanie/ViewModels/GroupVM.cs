using Models;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ViewModule;
using ViewModule.CSharp;
using static ViewModule.Validation.CSharp.Validators;

namespace Raspisanie.ViewModels
{
    public class GroupVM : ViewModelBase
    {
        private readonly INotifyingValue<string> nameOfGroup;
        private readonly INotifyingValue<int> codeOfGroup;
        private readonly INotifyingValue<int> term;
        private readonly INotifyingValue<Department> department;

        private readonly INotifyCommand saveGroup;

        public GroupVM(Department[] departments)
        {
            Departments = departments;

            Terms = new int[] { 1, 2 };

            nameOfGroup = this.Factory.Backing(nameof(NameOfGroup), "", NotNullOrWhitespace.Then(HasLengthNotLongerThan(50)));
            codeOfGroup = this.Factory.Backing(nameof(CodeOfGroup), 0);
            term = this.Factory.Backing(nameof(Term), 0);
            department = this.Factory.Backing<Department>(nameof(Department), null);

            saveGroup = this.Factory.CommandSyncParam<Window>(SaveAndClose);
        }

        public GroupVM(Group group, Department[] departments) : this(departments)
        {
            nameOfGroup.Value = group.NameOfGroup;
            codeOfGroup.Value = group.CodeOfGroup;
            term.Value = Terms.Single(t => t == group.Term);
            department.Value = departments.Single(d => d.CodeOfDepartment == group.Department.CodeOfDepartment);
        }

        private void SaveAndClose(Window obj)
        {
            if (!string.IsNullOrWhiteSpace(NameOfGroup) && Term>0 && Department != null)
            {
                Group = new Group
                {
                    NameOfGroup = NameOfGroup,
                    CodeOfGroup = CodeOfGroup,
                    Department = Department,
                    Term = Term
                };
                obj.DialogResult = true;
                obj.Close();
            }
        }

        public ICommand SaveCommand => saveGroup;

        public string NameOfGroup { get { return nameOfGroup.Value; } set { nameOfGroup.Value = value; } }
        public int CodeOfGroup { get { return codeOfGroup.Value; } set { codeOfGroup.Value = value; } }
        public int Term { get { return term.Value; } set { term.Value = value; } }
        public Department Department { get { return department.Value; } set { department.Value = value; } }

        public Group Group
        {
            get; private set;
        }

        public int[] Terms { get; }
        public Department[] Departments { get; }
    }
}
