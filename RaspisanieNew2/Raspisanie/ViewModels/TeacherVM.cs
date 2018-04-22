using Raspisanie.Models;
using System.Windows.Input;
using ViewModule;
using ViewModule.CSharp;
using System.Windows;
using System.Linq;

namespace Raspisanie.ViewModels
{
    class TeacherVM : ViewModelBase
    {
        private readonly INotifyingValue<int> codeOfTeacher;
        private readonly INotifyingValue<string> fio;
        private readonly INotifyingValue<string> post;
        private readonly INotifyingValue<Department> department;
        private readonly INotifyingValue<Department> departmentTwo;
        private readonly INotifyingValue<Department> departmentThree;

        private readonly INotifyCommand saveTeacher;
        
        public TeacherVM(Department[] departments)
        {
            Departments = departments;
            DepartmentsTwo = departments;
            DepartmentsThree = departments;


            codeOfTeacher = this.Factory.Backing(nameof(CodeOfTeacher), 0);
            fio = this.Factory.Backing(nameof(FIO), "");
            post = this.Factory.Backing(nameof(Post), "");
            department = this.Factory.Backing<Department>(nameof(Department), null);
            departmentTwo = this.Factory.Backing<Department>(nameof(DepartmentTwo), null);
            departmentThree = this.Factory.Backing<Department>(nameof(DepartmentThree), null);
            saveTeacher = this.Factory.CommandSyncParam<Window>(SaveAndClose);
        }

        public TeacherVM(Teacher teacher, Department[] departments) : this(departments)
        {
            codeOfTeacher.Value = teacher.CodeOfTeacher;
            fio.Value = teacher.FIO;
            post.Value = teacher.Post;
        }

        private void SaveAndClose(Window obj)
        {
            if (!string.IsNullOrWhiteSpace(FIO)
                && !string.IsNullOrWhiteSpace(Post)
                && CodeOfTeacher > 0
                && Department != null)
                Teacher = new Teacher
                {
                    CodeOfTeacher = CodeOfTeacher,
                    FIO = FIO,
                    Post = Post,
                
                };
            obj.Close();
        }
        
        public ICommand SaveCommand => saveTeacher;
        public int CodeOfTeacher { get { return codeOfTeacher.Value; }set { codeOfTeacher.Value = value; } }
        public string FIO { get { return fio.Value; } set { fio.Value = value; } }
        public string Post { get { return post.Value; } set { post.Value = value; } }
        public Department Department { get { return department.Value; } set { department.Value = value; } }
        public Department DepartmentTwo { get { return departmentTwo.Value; } set { departmentTwo.Value = value; } }
        public Department DepartmentThree { get { return departmentThree.Value; } set { departmentThree.Value = value; } }


        public Teacher Teacher
        {
            get; private set;
        }

        public Department[] Departments { get; }
        public Department[] DepartmentsTwo { get; }
        public Department[] DepartmentsThree { get; }
    }
}
