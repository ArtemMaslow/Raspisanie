using Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SozdanieRaspisaniya.ViewModel.Rules
{
    public class PlanCompleted : IRule
    {
        List<GroupsAndSubjects> allGroupsAndSubjects;
        string message = "";
        string lecHour = "";
        string exHour = "";
        string labHour = "";
               
        public PlanCompleted(ObservableCollection<GroupsAndSubjects> allGroupsAndSubjects)
        {
            this.allGroupsAndSubjects = new List<GroupsAndSubjects>();
            foreach (var item in allGroupsAndSubjects)
            {
                this.allGroupsAndSubjects.Add((GroupsAndSubjects)item.Clone());
            }
        }

        public void Check(ObservableCollection<ObservableCollection<DropItem>> filtered, ref List<string> listOfErrors)
        {
            for (int i = 0; i < filtered[0].Count; i++)
            {
                for (int j = 0; j < filtered.Count; j++)
                {
                    foreach (var group in allGroupsAndSubjects)
                    {
                        foreach (var subject in group.InformationAboutSubjects)
                        {
                            if (filtered[j][i].Item.Group != null && filtered[j][i].Item.Teacher != null && filtered[j][i].Item.Subject != null
                            && filtered[j][i].Item.Specifics != null && filtered[j][i].Item.NumberOfClassroom != null)
                            {
                                if (group.Group.CodeOfGroup == filtered[j][i].Item.Group.First().CodeOfGroup)
                                {
                                    if (subject.Subject.CodeOfSubject == filtered[j][i].Item.Subject.CodeOfSubject)
                                    {
                                        if (filtered[j][i].Item.Specifics == SheduleSettings.specifics[0])
                                        {
                                            if (filtered[j][i].Item.Ndindex == 0)
                                                subject.LectureHour -= 2;
                                            else
                                                subject.LectureHour -= 1;
                                        }
                                        if (filtered[j][i].Item.Specifics == SheduleSettings.specifics[1])
                                        {
                                            if (filtered[j][i].Item.Ndindex == 0)
                                                subject.ExerciseHour -= 2;
                                            else
                                                subject.ExerciseHour -= 1;
                                        }
                                        if (filtered[j][i].Item.Specifics == SheduleSettings.specifics[2])
                                        {
                                            if (filtered[j][i].Item.Ndindex == 0)
                                                subject.LaboratoryHour -= 2;
                                            else
                                                subject.LaboratoryHour -= 1;
                                        }
                                    }
                                }
                            }
                            if (filtered[j][i].ItemTwo.Group != null && filtered[j][i].ItemTwo.Teacher != null && filtered[j][i].ItemTwo.Subject != null
                                        && filtered[j][i].ItemTwo.Specifics != null && filtered[j][i].ItemTwo.NumberOfClassroom != null)
                            {
                                if (group.Group.CodeOfGroup == filtered[j][i].ItemTwo.Group.First().CodeOfGroup)
                                {
                                    if (subject.Subject.CodeOfSubject == filtered[j][i].ItemTwo.Subject.CodeOfSubject)
                                    {
                                        if (filtered[j][i].ItemTwo.Specifics == SheduleSettings.specifics[0])
                                        {
                                            subject.LectureHour -= 1;
                                        }
                                        if (filtered[j][i].ItemTwo.Specifics == SheduleSettings.specifics[1])
                                        {
                                                subject.ExerciseHour -= 1;
                                        }
                                        if (filtered[j][i].ItemTwo.Specifics == SheduleSettings.specifics[2])
                                        {
                                                subject.LaboratoryHour -= 1;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                foreach (var group in allGroupsAndSubjects)
                {
                    foreach(var subject in group.InformationAboutSubjects)
                    {
                        if (filtered[0][i].Item.Group.First().CodeOfGroup == group.Group.CodeOfGroup)
                        {
                            if (subject.LectureHour > 0 || subject.ExerciseHour > 0 || subject.LaboratoryHour > 0)
                            {
                                if (subject.LectureHour > 0)
                                {
                                    lecHour = "лекц: "+ subject.LectureHour + " ";
                                }

                                if (subject.ExerciseHour > 0)
                                {
                                    exHour = "упр: " + subject.ExerciseHour + " ";
                                }

                                if (subject.LaboratoryHour > 0)
                                {
                                    labHour = "лаб: " + subject.LaboratoryHour + " ";
                                }

                                message = string.Format("У группы {0} не хватает {1}{2}{3} часов по предмету {4} ",group.Group.NameOfGroup, lecHour,exHour,labHour, subject.Subject.NameOfSubject);
                                listOfErrors.Add(message);
                                lecHour = "";
                                exHour = "";
                                labHour = "";
                            }
                        }
                    }
                }
                message = "";
                lecHour = "";
                exHour = "";
                labHour = "";
            }
        }
    }
}

