using Raspisanie.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using ViewModule;
using ViewModule.CSharp;

namespace Raspisanie.ViewModels
{
    class WindowTeacherVM : ViewModelBase
    {
        private readonly INotifyingValue<int> index;

        private readonly INotifyCommand addCommand;
        private readonly INotifyCommand removeCommand;
        private readonly INotifyCommand editCommand;

        public WindowTeacherVM(ObservableCollection<Teacher> classTeacher, ObservableCollection<Department> departments)
        {
            addCommand = this.Factory.CommandSync(Add);
            removeCommand = this.Factory.CommandSync(Remove);
            editCommand = this.Factory.CommandSync(Edit);

            ClassTeacher = classTeacher;
            this.departments = departments;
            index = this.Factory.Backing(nameof(Index), -1);
        }

        private void Add()
        {
            var context = new TeacherVM(departments.ToArray());
            var wint = new NewTeacher()
            {
                DataContext = context
            };
            wint.ShowDialog();
            System.Console.WriteLine(context.Teacher != null);
            if (context.Teacher != null)
                ClassTeacher.Add(context.Teacher);
        }

        private void Edit()
        {
            if (Index >= 0)
            {
                var teacher = ClassTeacher[Index];
                var context = new TeacherVM(teacher, departments.ToArray());
                var wint = new NewTeacher()
                {
                    DataContext = context
                };
                wint.ShowDialog();
                if (context.Teacher != null)
                {
                    ClassTeacher[Index] = context.Teacher;
                }
            }
        }

        private void Remove()
        {
            if (Index >= 0)
                ClassTeacher.RemoveAt(Index);
        }

        private ObservableCollection<Department> departments { get; }

        public ObservableCollection<Teacher> ClassTeacher { get; }

        public ICommand AddCommand => addCommand;
        public ICommand RemoveCommand => removeCommand;
        public ICommand EditCommand => editCommand;

        public int Index { get { return index.Value; } set { index.Value = value; } }
    }
}
