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
    class WindowTeachersAndSubjectsVM : ViewModelBase
    {
        private readonly INotifyingValue<int> teacherIndex;

        private readonly INotifyCommand addCommand;
        private readonly INotifyCommand removeCommand;
        private readonly INotifyCommand editCommand;

        public WindowTeachersAndSubjectsVM(ObservableCollection<Teacher> classTeachers, List<Subject> allSubjectsList, List<DayOfWeek> allDayList)
        {
            ClassTeachers = classTeachers;
            AllSubjectList = allSubjectsList;
            AllDayList = allDayList;

            addCommand = this.Factory.CommandSync(Add);
            removeCommand = this.Factory.CommandSync(Remove);
            editCommand = this.Factory.CommandSync(Edit);

            teacherIndex = this.Factory.Backing(nameof(TeacherIndex), -1);
        }

        public void Add()
        {
            var context = new TeachersAndSubjectsVM(ClassTeachers[TeacherIndex], AllSubjectList, AllDayList);
            var wintands = new NewTeachersAndSubjects()
            {
                DataContext = context
            };
            wintands.ShowDialog();
        }

        public void Edit()
        {
            var context = new TeachersAndSubjectsVM(ClassTeachers[TeacherIndex], AllSubjectList, AllDayList);
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
        public List<Subject> SubjectList { get; }
        public List<DayOfWeek> DayList { get; }
        public List<Subject> AllSubjectList { get; }
        public List<DayOfWeek> AllDayList { get; }

    }
}
