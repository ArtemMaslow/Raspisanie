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
    public class TeachersAndDepartmentsVM : ViewModelBase
    {
        private readonly INotifyingValue<List<Subject>> subjectList;
        private readonly INotifyingValue<List<DayOfWeek>> dayList;
        private readonly INotifyingValue<int> subjectsIndex;
        private readonly INotifyingValue<int> allSubjectsIndex;
        private readonly INotifyingValue<int> daysIndex;

        private readonly INotifyCommand saveTeachersAndSubjects;
        private readonly INotifyCommand removeElemet;

        public TeachersAndDepartmentsVM(Teacher teacher,List<Subject> subectsList, List<DayOfWeek> daysList)
        {
            subjectList = this.Factory.Backing<List<Subject>>(nameof(SubjectList), subectsList);
            dayList = this.Factory.Backing<List<DayOfWeek>>(nameof(DayList), daysList);
            subjectsIndex = this.Factory.Backing(nameof(SubjectsIndex), -1);
            daysIndex = this.Factory.Backing(nameof(DaysIndex), -1);
            allSubjectsIndex = this.Factory.Backing(nameof(AllSubjectsIndex), -1);
            Teacher = teacher;
            saveTeachersAndSubjects = this.Factory.CommandSyncParam<Window>(SaveAndClose);
        }

        public TeachersAndDepartmentsVM(TeachersAndSubjectsView teacherAndsubject,List<Subject> subectsList, List<DayOfWeek> daysList) : this(subectsList, daysList)
        {
            
        }

        public void SaveAndClose(Window obj)
        {
            if (SubjectsList.Count > 0 && DaysList.Count > 0)
            {
                TeachersAndSubjectsView = new TeachersAndSubjectsView
                {
                    subjectList = SubjectList,
                    dayList = DayList,
                    teacher = Teacher
                };
            }
            obj.Close();
        }

        public void RemoveElement()
        {
        }
        
        ICommand SaveCommand => saveTeachersAndSubjects;
        public List<Subject> SubjectList { get { return subjectList.Value; } set { subjectList.Value = value; } }
        public List<DayOfWeek> DayList { get { return dayList.Value; } set { dayList.Value = value; } }
        public int SubjectsIndex { get { return subjectsIndex.Value; } set { subjectsIndex.Value = value; } }
        public int AllSubjectsIndex { get { return allSubjectsIndex.Value; } set { allSubjectsIndex.Value = value; } }
        public int DaysIndex { get { return daysIndex.Value; } set { daysIndex.Value = value; } }

        public TeachersAndSubjectsView TeachersAndSubjectsView
        {
            get; private set;
        }

        public List<Subject> SubjectsList { get; }
        public List<Subject> AllSubjectsList { get; }
        public List<DayOfWeek> DaysList { get; }
        public Teacher Teacher { get; }

    }
}
