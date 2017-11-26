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
        private readonly INotifyingValue<int> hourOfLoad;

        private readonly INotifyCommand saveTeacher;
        
        public TeacherVM(Department[] departments)
        {
            Departments = departments;

            codeOfTeacher = this.Factory.Backing(nameof(CodeOfTeacher), 0);
            fio = this.Factory.Backing(nameof(FIO), "");
            post = this.Factory.Backing(nameof(Post), "");
            department = this.Factory.Backing<Department>(nameof(Department), null);
            hourOfLoad = this.Factory.Backing(nameof(HourOfLoad), 0);

            saveTeacher = this.Factory.CommandSyncParam<Window>(SaveAndClose);
        }

        public TeacherVM(Teacher teacher, Department[] departments) : this(departments)
        {
            codeOfTeacher.Value = teacher.CodeOfTeacher;
            fio.Value = teacher.FIO;
            post.Value = teacher.Post;
            hourOfLoad.Value = teacher.HourOfLoad;
            department.Value = departments.Single(d=>d.CodeOfDepartment==teacher.CodeOfDepartment);
        }

        private void SaveAndClose(Window obj)
        {
            if (!string.IsNullOrWhiteSpace(FIO)
                && !string.IsNullOrWhiteSpace(Post)
                && CodeOfTeacher > 0
                && HourOfLoad > 0
                && Department != null)
                Teacher = new Teacher
                {
                    CodeOfTeacher = CodeOfTeacher,
                    FIO = FIO,
                    Post = Post,
                    CodeOfDepartment = Department.CodeOfDepartment,
                    HourOfLoad = HourOfLoad
                };
            obj.Close();
        }
        
        public ICommand SaveCommand => saveTeacher;
        public int CodeOfTeacher { get { return codeOfTeacher.Value; }set { codeOfTeacher.Value = value; } }
        public string FIO { get { return fio.Value; } set { fio.Value = value; } }
        public string Post { get { return post.Value; } set { post.Value = value; } }
        public Department Department { get { return department.Value; } set { department.Value = value; } }
        public int HourOfLoad { get { return hourOfLoad.Value; } set { hourOfLoad.Value = value; } }
        
        public Teacher Teacher
        {
            get; private set;
        }

        public Department[] Departments { get; }
    }
}
