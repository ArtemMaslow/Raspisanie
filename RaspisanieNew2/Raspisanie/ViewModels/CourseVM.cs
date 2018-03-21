using Raspisanie.Models;
using System.Windows.Input;
using ViewModule;
using ViewModule.CSharp;
using System.Windows;
using System.Linq;

namespace Raspisanie.ViewModels
{
    class CourseVM : ViewModelBase
    {
        private readonly INotifyingValue<Teacher> teacher;
        private readonly INotifyingValue<Group> group;
        private readonly INotifyingValue<Subject> subject;

        private readonly INotifyCommand saveCourse;

        public CourseVM(Teacher[] teachers, Group[] groups, Subject[] subjects)
        {
            Teachers = teachers;
            Groups = groups;
            Subjects = subjects;

            teacher = this.Factory.Backing<Teacher>(nameof(Teacher), null);
            group = this.Factory.Backing<Group>(nameof(Group), null);
            subject = this.Factory.Backing<Subject>(nameof(Subject), null);

            saveCourse = this.Factory.CommandSyncParam<Window>(SaveAndClose);
        }

        public CourseVM(Course course, Teacher[] teachers, Group[] groups, Subject[] subjects) : this(teachers,  groups, subjects)
        {
            teacher.Value = teachers.Single(t=>t.CodeOfTeacher==course.CodeOfTeacher);
            group.Value = groups.Single(g => g.CodeOfGroup == course.CodeOfGroup);
            subject.Value = subjects.Single(s => s.CodeOfSubject == course.CodeOfSubject);

        }

        private void SaveAndClose(Window obj)
        {
            if (Teacher != null
                && Group != null
                && Subject != null)
                Course = new Course
                {
                    CodeOfTeacher = Teacher.CodeOfTeacher,
                    CodeOfGroup = Group.CodeOfGroup,
                    CodeOfSubject = Subject.CodeOfSubject
                };
            obj.Close();
        }

        public ICommand SaveCommand => saveCourse;
        public Teacher Teacher { get { return teacher.Value; } set { teacher.Value = value; } }
        public Group Group { get { return group.Value; } set { group.Value = value; } }
        public Subject Subject { get { return subject.Value; } set { subject.Value = value; } }

        public Course Course
        {
            get; private set;
        }

        public Teacher[] Teachers { get; }
        public Group[] Groups { get; }
        public Subject[] Subjects { get; }
    }
}
