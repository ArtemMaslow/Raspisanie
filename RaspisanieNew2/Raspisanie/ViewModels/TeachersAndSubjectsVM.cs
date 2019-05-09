using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ViewModule;
using ViewModule.CSharp;

namespace Raspisanie.ViewModels
{
    public class TeachersAndSubjectsVM : ViewModelBase
    {
        private readonly INotifyCommand saveTeachersAndSubjects;

        public TeachersAndSubjectsVM(TeachersAndSubjects teachersAndSubjects, Subject[] subjects, DayOfWeek[] days)
        {
            HashSet<int> sj = new HashSet<int>(teachersAndSubjects.SubjectList.Select(s => s.CodeOfSubject ));
            HashSet<DayOfWeek> dw = new HashSet<DayOfWeek>(teachersAndSubjects.DayList);
            Subjects = subjects.Where(d=>d.Department.CodeOfDepartment == teachersAndSubjects.Teacher.Department.CodeOfDepartment).OrderBy(n=>n.NameOfSubject).Select(s => new TeachersAndSubjectsViewHelper<Subject>
            {
                IsSelected = sj.Contains(s.CodeOfSubject), 
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
