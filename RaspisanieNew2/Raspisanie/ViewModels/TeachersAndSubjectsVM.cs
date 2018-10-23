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

namespace Raspisanie.ViewModels
{
    public class TeachersAndSubjectsVM : ViewModelBase
    {
        private readonly INotifyCommand saveTeachersAndSubjects;
        private readonly INotifyCommand removeElemet;

        public TeachersAndSubjectsVM(TeachersAndSubjects teachersAndSubjects, Subject[] subjects, DayOfWeek[] days)
        {
            HashSet<int> sj = new HashSet<int>(subjects.Select(s => s.CodeOfSubject));
            HashSet<DayOfWeek> dw = new HashSet<DayOfWeek>(days);

            Subjects = subjects.Select(s => new TeachersAndSubjectsViewHelper<Subject>
            {
                IsSelected = sj.Contains(s.CodeOfSubject),
                Value = s
            }).ToArray();
            Days = days.Select(d => new TeachersAndSubjectsViewHelper<DayOfWeek>
            {
                IsSelected = dw.Contains(d),
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
            // TODO: add validation logic here
            SelectedDays = Days.Where(d => d.IsSelected).Select(d => d.Value).ToArray();
            SelectedSubjects = Subjects.Where(s => s.IsSelected).Select(s => s.Value).ToArray();
            obj.Close();
        }
        ICommand SaveCommand => saveTeachersAndSubjects;
    }
}
