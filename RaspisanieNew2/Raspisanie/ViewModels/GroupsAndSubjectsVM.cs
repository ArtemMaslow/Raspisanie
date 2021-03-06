﻿using Models;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ViewModule;
using ViewModule.CSharp;
using static ViewModule.Validation.CSharp.Validators;

namespace Raspisanie.ViewModels
{
    public class GroupsAndSubjectsVM : ViewModelBase
    {
        private readonly INotifyingValue<Subject> subject;
        private readonly INotifyingValue<int> lectureHour;
        private readonly INotifyingValue<int> exerciseHour;
        private readonly INotifyingValue<int> laboratoryHour;

        private readonly INotifyCommand saveGroupsAndSubjects;

        public GroupsAndSubjectsVM(Subject[] subjects)
        {
            Subjects = subjects;
            subject = this.Factory.Backing(nameof(Subject), null, ContainedWithin(Subjects));
            lectureHour = this.Factory.Backing(nameof(LectureHour), 0, Custom<int>(NumberIsPositive,""));
            exerciseHour = this.Factory.Backing(nameof(ExerciseHour), 0,Custom<int>(NumberIsPositive, ""));
            laboratoryHour = this.Factory.Backing(nameof(LaboratoryHour), 0, Custom<int>(NumberIsPositive, ""));

            saveGroupsAndSubjects = this.Factory.CommandSyncParam<Window>(SaveAndClose);
        }

        public GroupsAndSubjectsVM(SubjectInform InformationAboutSubject, Subject[] subjects) : this(subjects)
        {
            lectureHour.Value = InformationAboutSubject.LectureHour;
            exerciseHour.Value = InformationAboutSubject.ExerciseHour;
            laboratoryHour.Value = InformationAboutSubject.LaboratoryHour;
            subject.Value = subjects.Single(s=>s.CodeOfSubject == InformationAboutSubject.Subject.CodeOfSubject);
        }

        public void SaveAndClose(Window obj)
        {
            if (Subject != null && LectureHour >= 0 && ExerciseHour >= 0 && LaboratoryHour >= 0 )
            {
                InformationAboutSubjects = new SubjectInform
                {
                    Subject = Subject,
                    LectureHour = LectureHour,
                    ExerciseHour = ExerciseHour,
                    LaboratoryHour = LaboratoryHour
                };
                obj.DialogResult = true;
                obj.Close();
            }
        }

        public bool NumberIsPositive(int number)
        {
            if (number >= 0)
            {
                return true;
            }
            return false;
        }

        public ICommand SaveCommand => saveGroupsAndSubjects;

        public Subject Subject { get { return subject.Value; } set { subject.Value = value; } }
        public int LectureHour { get { return lectureHour.Value; } set { lectureHour.Value = value; } }
        public int ExerciseHour { get { return exerciseHour.Value; } set { exerciseHour.Value = value; } }
        public int LaboratoryHour { get { return laboratoryHour.Value; } set { laboratoryHour.Value = value; } }

        public Subject[] Subjects { get; }
        public SubjectInform InformationAboutSubjects { get; set; }
        public GroupsAndSubjects GroupsAndSubjects
        {
            get; private set;
        }
    }
}
