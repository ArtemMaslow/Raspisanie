﻿using Models;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ViewModule;
using ViewModule.CSharp;
using static ViewModule.Validation.CSharp.Validators;
namespace Raspisanie.ViewModels
{
    class TeacherVM : ViewModelBase
    {
        private readonly INotifyingValue<int> codeOfTeacher;
        private readonly INotifyingValue<string> fio;
        private readonly INotifyingValue<string> post;
        private readonly INotifyingValue<string> postTwo;
        private readonly INotifyingValue<string> mail;
        private readonly INotifyingValue<bool> isReadLecture;

        private readonly INotifyingValue<Department> department;
        private readonly INotifyingValue<Department> departmentTwo;
        private readonly INotifyCommand saveTeacher;
        
        public TeacherVM(Department[] departments)
        {
            Departments = departments;
            DepartmentsTwo = departments;
              
            codeOfTeacher = this.Factory.Backing(nameof(CodeOfTeacher), 0);
            fio = this.Factory.Backing(nameof(FIO), "", NotNullOrWhitespace.Then(HasLengthNotLongerThan(50)));
            post = this.Factory.Backing(nameof(Post), "", NotNullOrWhitespace.Then(HasLengthNotLongerThan(25)));
            postTwo = this.Factory.Backing(nameof(PostTwo), "");
            mail = this.Factory.Backing(nameof(Mail), "", NotNullOrWhitespace.Then(HasLengthNotLongerThan(50)));
            isReadLecture = this.Factory.Backing(nameof(IsReadLecture), false);

            department = this.Factory.Backing(nameof(Department), null, ContainedWithin(Departments));
            departmentTwo = this.Factory.Backing<Department>(nameof(DepartmentTwo), null);
            
            saveTeacher = this.Factory.CommandSyncParam<Window>(SaveAndClose);
        }

        public TeacherVM(Teacher teacher, Department[] departments) : this(departments)
        {
            codeOfTeacher.Value = teacher.CodeOfTeacher;
            fio.Value = teacher.FIO;
            post.Value = teacher.Post;
            mail.Value = teacher.Mail;
            isReadLecture.Value = teacher.IsReadLecture;
            department.Value = departments.Single(f => f.CodeOfDepartment == teacher.Department.CodeOfDepartment);
        }

        private void SaveAndClose(Window obj)
        {
            if (!string.IsNullOrWhiteSpace(FIO) && !string.IsNullOrWhiteSpace(Post) && !string.IsNullOrWhiteSpace(Mail) && Department != null)
            {
                Teacher = new Teacher
                {
                    CodeOfTeacher = CodeOfTeacher,
                    FIO = FIO,
                    Post = Post,
                    PostTwo = PostTwo,
                    Mail = Mail,
                    IsReadLecture = IsReadLecture,
                    Department = Department,
                    DepartmentTwo = DepartmentTwo
                };
                obj.DialogResult = true;
                obj.Close();
            }
        }
        
        public ICommand SaveCommand => saveTeacher;
        public int CodeOfTeacher { get { return codeOfTeacher.Value; }set { codeOfTeacher.Value = value; } }
        public string FIO { get { return fio.Value; } set { fio.Value = value; } }
        public string Post { get { return post.Value; } set { post.Value = value; } }
        public string PostTwo { get { return postTwo.Value; } set { postTwo.Value = value; } }
        public string Mail { get { return mail.Value; } set { mail.Value = value; } }
        public bool IsReadLecture { get { return isReadLecture.Value; } set { isReadLecture.Value = value; } }

        public Department Department { get { return department.Value; } set { department.Value = value; } }
        public Department DepartmentTwo { get { return departmentTwo.Value; } set { departmentTwo.Value = value; } }
    
        public Teacher Teacher
        {
            get; private set;
        }

        public Department[] Departments { get; }
        public Department[] DepartmentsTwo { get; }

    }
}
