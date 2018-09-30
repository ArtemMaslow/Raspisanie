using Raspisanie.Models;
using Raspisanie.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using ViewModule;
using ViewModule.CSharp;

namespace Raspisanie.ViewModels
{
    class WindowTeachersAndDepartmentsVM : ViewModelBase
    {
        private readonly INotifyingValue<int> teacherIndex;

        private readonly INotifyCommand addCommand;
        private readonly INotifyCommand removeCommand;
        private readonly INotifyCommand editCommand;

        public WindowTeachersAndDepartmentsVM()
        {
            addCommand = this.Factory.CommandSync(Add);
            removeCommand = this.Factory.CommandSync(Remove);
            editCommand = this.Factory.CommandSync(Edit);
            
            teacherIndex = this.Factory.Backing(nameof(TeacherIndex), -1);
        }

        public void Add()
        {
            var context = new TeachersAndDepartmentsVM(SubjectsList, DaysList);
            var wintands = new NewTeachersAndSubjects()
            {
                DataContext = context
            };
            wintands.ShowDialog();
        }

        public void Edit()
        {
            var context = new TeachersAndDepartmentsVM(SubjectsList, DaysList);
            var wintands = new NewTeachersAndSubjects()
            {
                DataContext = context
            };
            wintands.ShowDialog();
        }

        public void Remove()
        {

        }

        public ICommand AddCommand => addCommand;
        public ICommand RemoveCommand => removeCommand;
        public ICommand EditCommand => editCommand;

        public int TeacherIndex { get { return teacherIndex.Value; } set { teacherIndex.Value = value; } }
        public ObservableCollection<Teacher> ClassTeachers { get; }
        public List<Subject> SubjectsList { get; }
        public List<DayOfWeek> DaysList { get; }
    }
}
