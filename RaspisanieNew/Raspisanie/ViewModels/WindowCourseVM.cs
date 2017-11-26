using Raspisanie.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using ViewModule;
using ViewModule.CSharp;

namespace Raspisanie.ViewModels
{
    class WindowCourseVM :ViewModelBase
    {
        private readonly INotifyingValue<int> index;

        private readonly INotifyCommand addCommand;
        private readonly INotifyCommand removeCommand;
        private readonly INotifyCommand editCommand;

        public WindowCourseVM(ObservableCollection<Course> classCourse, ObservableCollection<Teacher> teachers,
            ObservableCollection<Group> groups, ObservableCollection<Subject> subjects)
        {
            addCommand = this.Factory.CommandSync(Add);
            removeCommand = this.Factory.CommandSync(Remove);
            editCommand = this.Factory.CommandSync(Edit);

            ClassCourse = classCourse;

            this.teachers = teachers;
            this.groups = groups;
            this.subjects = subjects;
            index = this.Factory.Backing(nameof(Index), -1);
        }

        private void Add()
        {
            var context = new CourseVM(teachers.ToArray(), groups.ToArray(), subjects.ToArray());
            var winc = new NewCourse()
            {
                DataContext = context
            };
            winc.ShowDialog();
            System.Console.WriteLine(context.Course != null);
            if (context.Course != null)
                ClassCourse.Add(context.Course);
        }

        private void Edit()
        {
            if (Index >= 0)
            {
                var course = ClassCourse[Index];
                var context = new CourseVM(course, teachers.ToArray(), groups.ToArray(), subjects.ToArray());
                var wint = new NewCourse()
                {
                    DataContext = context
                };
                wint.ShowDialog();
                if (context.Teacher != null)
                {
                    ClassCourse[Index] = context.Course;
                }
            }
        }

        private void Remove()
        {
            if (Index >= 0)
                ClassCourse.RemoveAt(Index);
        }

        private ObservableCollection<Teacher> teachers { get; }
        private ObservableCollection<Group> groups { get; }
        private ObservableCollection<Subject> subjects { get; }

        public ObservableCollection<Course> ClassCourse { get; }

        public ICommand AddCommand => addCommand;
        public ICommand RemoveCommand => removeCommand;
        public ICommand EditCommand => editCommand;

        public int Index { get { return index.Value; } set { index.Value = value; } }
    }
}
