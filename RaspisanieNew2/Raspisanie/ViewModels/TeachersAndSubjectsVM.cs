using Raspisanie.Models;
using System.Windows.Input;
using ViewModule;
using ViewModule.CSharp;
using System.Windows;
using System.Linq;
using System.Collections.Generic;
using System;

namespace Raspisanie.ViewModels
{
    public class TeachersAndSubjectsVM : ViewModelBase
    {
        private readonly INotifyingValue<List<Subject>> subjectList;
        private readonly INotifyingValue<int> subjectIndex;

        private readonly INotifyingValue<List<DayOfWeek>> dayList;
        private readonly INotifyingValue<int> dayIndex;

        private readonly INotifyingValue<List<Subject>> allSubjectsList;
        private readonly INotifyingValue<int> allSubjectIndex;

        private readonly INotifyingValue<List<DayOfWeek>> allDayList;
        private readonly INotifyingValue<int> allDayIndex;
        

        private readonly INotifyCommand saveTeachersAndSubjects;
        private readonly INotifyCommand removeElemet;

        public TeachersAndSubjectsVM(Teacher teacher,List<Subject> allSubectsList, List<DayOfWeek> allDaysList)
        {
            subjectList = this.Factory.Backing<List<Subject>>(nameof(SubjectList), null);
            subjectIndex = this.Factory.Backing(nameof(SubjectIndex), -1);

            dayList = this.Factory.Backing<List<DayOfWeek>>(nameof(DayList), null);            
            dayIndex = this.Factory.Backing(nameof(DayIndex), -1);

            allSubjectsList = this.Factory.Backing<List<Subject>>(nameof(AllSubjectList), allSubectsList);
            allSubjectIndex = this.Factory.Backing(nameof(AllSubjectIndex), -1);

            allDayList = this.Factory.Backing<List<DayOfWeek>>(nameof(AllDayList), allDaysList);
            allDayIndex = this.Factory.Backing(nameof(AllDayIndex), -1);

            Teacher = teacher;
            saveTeachersAndSubjects = this.Factory.CommandSyncParam<Window>(SaveAndClose);
        }

        public TeachersAndSubjectsVM(TeachersAndSubjectsView teacherAndsubject)
        {
            subjectList.Value = teacherAndsubject.SubjectList;
            dayList.Value = teacherAndsubject.DayList;
            Teacher = teacherAndsubject.Teacher;            
        }

        public void SaveAndClose(Window obj)
        {
            if (SubjectsList.Count > 0 && DaysList.Count > 0)
            {
                TeachersAndSubjectsView = new TeachersAndSubjectsView
                {
                    SubjectList = SubjectList,
                    DayList = DayList,
                    Teacher = Teacher
                };
            }
            obj.Close();
        }

        public void RemoveElement()
        {

        }
        
        ICommand SaveCommand => saveTeachersAndSubjects;
        public List<Subject> SubjectList { get { return subjectList.Value; } set { subjectList.Value = value; } }
        public int SubjectIndex { get { return subjectIndex.Value; } set { subjectIndex.Value = value; } }

        public List<DayOfWeek> DayList { get { return dayList.Value; } set { dayList.Value = value; } }
        public int DayIndex { get { return dayIndex.Value; } set { dayIndex.Value = value; } }

        public List<Subject> AllSubjectList { get { return allSubjectsList.Value; } set { allSubjectsList.Value = value; } }
        public int AllSubjectIndex { get { return allSubjectIndex.Value; } set { allSubjectIndex.Value = value; } }

        public List<DayOfWeek> AllDayList { get { return allDayList.Value; } set { allDayList.Value = value; } }
        public int AllDayIndex { get { return allDayIndex.Value; } set { allDayIndex.Value = value; } }
        
        public TeachersAndSubjectsView TeachersAndSubjectsView
        {
            get; private set;
        }

        public List<Subject> SubjectsList { get; }
        public List<Subject> AllSubjectsList { get; }
        public List<DayOfWeek> AllDaysList { get; }
        public List<DayOfWeek> DaysList { get; }

        public Teacher Teacher { get; }

    }
}
