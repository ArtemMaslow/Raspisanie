using Raspisanie.Models;
using System.Windows.Input;
using ViewModule;
using ViewModule.CSharp;
using System.Windows;
using System.Linq;
using System.Collections.Generic;
using System;
using ModelLibrary;
using ViewModule;

namespace Raspisanie.ViewModels
{
    public class GroupsAndSubjectsVM : ViewModelBase
    {
        private readonly INotifyingValue<Subject> subject;
        private readonly INotifyingValue<int> lectureHour;
        private readonly INotifyingValue<int> exerciseHour;
        private readonly INotifyingValue<int> laboratoryHour;
        private readonly INotifyingValue<int> semester;

        private readonly INotifyCommand saveGroupsAndSubjects;

        public GroupsAndSubjectsVM(Subject[] subjects)
        {
            Subjects = subjects;
            subject = this.Factory.Backing<Subject>(nameof(Subject),null);
            lectureHour = this.Factory.Backing(nameof(LectureHour), 0);
            exerciseHour = this.Factory.Backing(nameof(ExerciseHour), 0);
            laboratoryHour = this.Factory.Backing(nameof(LaboratoryHour), 0);
            semester = this.Factory.Backing(nameof(Semester), 0);

            saveGroupsAndSubjects = this.Factory.CommandSyncParam<Window>(SaveAndClose);
        }

        public GroupsAndSubjectsVM(GroupsAndSubjects groupsAndSubjects, Subject[] subjects)
        {
            SubjectInform = groupsAndSubjects.SubjectInform.Select(s => new SubjectInform
            {
                Subject = s.Subject,
                LectureHour = s.LectureHour,
                ExerciseHour = s.ExerciseHour,
                LaboratoryHour = s.LaboratoryHour
            }).ToArray();
            semester.Value = groupsAndSubjects.Semester;
        }

        public void SaveAndClose(Window obj)
        {
            if (Subject != null && LectureHour >= 0 && ExerciseHour >= 0 && LaboratoryHour >= 0 && (Semester == 1 || Semester == 2))
            {
                SubjectCons = new SubjectInform
                {
                    Subject = Subject,
                    LectureHour = LectureHour,
                    ExerciseHour = ExerciseHour,
                    LaboratoryHour = LaboratoryHour
                };
                Semester = Semester;
            }
            obj.Close();
        }

        public ICommand SaveCommand => saveGroupsAndSubjects;

        public Subject Subject { get { return subject.Value; } set { subject.Value = value; } }
        public int LectureHour { get { return lectureHour.Value; } set { lectureHour.Value = value; } }
        public int ExerciseHour { get { return exerciseHour.Value; } set { exerciseHour.Value = value; } }
        public int LaboratoryHour { get { return laboratoryHour.Value; } set { laboratoryHour.Value = value; } }
        public int Semester { get { return semester.Value; } set { semester.Value = value; } }

        public Subject[] Subjects { get; }
        public SubjectInform[] SubjectInform { get; }

        public SubjectInform SubjectCons
        {
            get; private set;
        }
    }
}
