using Raspisanie.Models;
using System.Windows.Input;
using ViewModule;
using ViewModule.CSharp;
using System.Windows;
using System.Linq;
using System.Collections.Generic;
using System;
using ModelLibrary;
using Raspisanie.ViewModels;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;

namespace Raspisanie.ViewModels
{
    public class TeachersAndSubjectsVM : ViewModelBase
    {
        private readonly INotifyCommand saveTeachersAndSubjects;
        //private readonly INotifyCommand removeElemet;

        public TeachersAndSubjectsVM(TeachersAndSubjects teachersAndSubjects, Subject[] subjects, DayOfWeek[] days)
        {
            HashSet<int> sj = new HashSet<int>(teachersAndSubjects.SubjectList.Select(s => s.CodeOfSubject ));
            HashSet<DayOfWeek> dw = new HashSet<DayOfWeek>(teachersAndSubjects.DayList);
            Subjects = subjects.Where(d=>d.Department.CodeOfDepartment == teachersAndSubjects.Teacher.Department.CodeOfDepartment).OrderBy(n=>n.NameOfSubject).Select(s => new TeachersAndSubjectsViewHelper<Subject>
            {
                IsSelected = /*sj.Count == 0 ||*/ sj.Contains(s.CodeOfSubject), //по умолчанию выделить всё элементы.
                Value = s
            }).ToArray();
            Days = days.Select(d => new TeachersAndSubjectsViewHelper<DayOfWeek>
            {
                IsSelected = dw.Count == 0 || dw.Contains(d),
                Value = d
            }).ToArray();

            saveTeachersAndSubjects = this.Factory.CommandSyncParam<Window>(SaveAndClose);
        }

        public TeachersAndSubjectsViewHelper<Subject>[] Subjects { get; }
        public TeachersAndSubjectsViewHelper<DayOfWeek>[] Days { get; }

        public Subject[] SelectedSubjects { get; private set; }
        public DayOfWeek[] SelectedDays { get; private set; }
        public void SaveAndClose(Window obj)
        {
            if (Subjects.Where(s => s.IsSelected).Count() > 0 && Days.Where(d => d.IsSelected).Count() > 0)
            {
                SelectedDays = Days.Where(d => d.IsSelected).Select(d => d.Value).ToArray();
                SelectedSubjects = Subjects.Where(s => s.IsSelected).Select(s => s.Value).ToArray();

                obj.DialogResult = true;
                obj.Close();
            }
            
        }
        public ICommand SaveCommand => saveTeachersAndSubjects;
    }
}
