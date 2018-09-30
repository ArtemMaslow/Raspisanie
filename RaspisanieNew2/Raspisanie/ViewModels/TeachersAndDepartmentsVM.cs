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
        private readonly INotifyingValue<int> index;

        private readonly INotifyCommand saveTeachersAndSubjects;
        private readonly INotifyCommand removeElemet;

        public TeachersAndDepartmentsVM(List<Subject> subectsList, List<DayOfWeek> daysList)
        {
            SubjectsList = subectsList;
            DaysList = daysList;

            subjectList = this.Factory.Backing<List<Subject>>(nameof(SubjectList),null);
            dayList = this.Factory.Backing<List<DayOfWeek>>(nameof(DayList), null);

            saveTeachersAndSubjects = this.Factory.CommandSyncParam<Window>(SaveAndClose);
        }

        public TeachersAndDepartmentsVM(TeachersAndSubjectsView teacherAndsubject,List<Subject> subectsList, List<DayOfWeek> daysList) : this(subectsList, daysList)
        {
            foreach(var value in subectsList)
                subjectList.Value.Add(value);

            foreach (var value in daysList)
                dayList.Value.Add(value);
        }

        public void SaveAndClose(Window obj)
        {
            if (SubjectsList.Count > 0 && DaysList.Count > 0)
            {
                TeachersAndSubjectsView = new TeachersAndSubjectsView
                {
                    subjectList = SubjectList,
                    dayList = DayList
                };
            }
            obj.Close();
        }

        public void RemoveElement()
        {
            foreach(var value in IndexList)
            SubjectsList.RemoveAt(value);
        }
        
        ICommand SaveCommand => saveTeachersAndSubjects;
        public List<Subject> SubjectList { get { return subjectList.Value; } set { subjectList.Value = value; } }
        public List<DayOfWeek> DayList { get { return dayList.Value; } set { dayList.Value = value; } }
        public int Index { get { return index.Value; } set { index.Value = value; } }

        public TeachersAndSubjectsView TeachersAndSubjectsView
        {
            get; private set;
        }

        public List<Subject> SubjectsList { get; }
        public List<DayOfWeek> DaysList { get; }

        public List<int> IndexList;
    }
}
