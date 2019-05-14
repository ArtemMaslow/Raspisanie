using Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SozdanieRaspisaniya.ViewModel.Rules
{
    public class NoOverlay : IRule
    {
        private Dictionary<int, DropInformation> teacherInform = new Dictionary<int, DropInformation>();
        private Dictionary<int, DropInformation> classroomInform = new Dictionary<int, DropInformation>();
        private string message = "";

        public void Check(ObservableCollection<ObservableCollection<DropItem>> filtered, ref List<string> listOfErrors)
        {
            for (int i = 0; i < filtered.Count; i++)
            {
                teacherInform.Clear();
                classroomInform.Clear();
                for (int j = 0; j < filtered[0].Count; j++)
                {

                    if (filtered[i][j].Item.Group != null && filtered[i][j].Item.Teacher != null && filtered[i][j].Item.Subject != null
                        && filtered[i][j].Item.Specifics != null && filtered[i][j].Item.NumberOfClassroom != null)
                    {
                        if (teacherInform.ContainsKey(filtered[i][j].Item.Teacher.CodeOfTeacher) && teacherInform[filtered[i][j].Item.Teacher.CodeOfTeacher].Subject != filtered[i][j].Item.Subject)
                        {
                            message = string.Format("Преподователь {0} не может вести одновременно занятия {1} и {2} у групп {3} и {4}!",
                                filtered[i][j].Item.Teacher.FIO, filtered[i][j].Item.Subject.NameOfSubject, teacherInform[filtered[i][j].Item.Teacher.CodeOfTeacher].Subject.NameOfSubject,
                                filtered[i][j].Item.Group.Single().NameOfGroup, teacherInform[filtered[i][j].Item.Teacher.CodeOfTeacher].Group.Single().NameOfGroup);
                            listOfErrors.Add(message);
                        }
                        else if (teacherInform.ContainsKey(filtered[i][j].Item.Teacher.CodeOfTeacher) && teacherInform[filtered[i][j].Item.Teacher.CodeOfTeacher].Specifics != filtered[i][j].Item.Specifics)
                        {
                            message = string.Format("Преподователь {0} не может вести одновременно {1} и {2} по занятию {3} у групп {4} и {5}!",
                               filtered[i][j].Item.Teacher.FIO, filtered[i][j].Item.Specifics, teacherInform[filtered[i][j].Item.Teacher.CodeOfTeacher].Specifics, filtered[i][j].Item.Subject.NameOfSubject,
                               filtered[i][j].Item.Group.Single().NameOfGroup, teacherInform[filtered[i][j].Item.Teacher.CodeOfTeacher].Group.Single().NameOfGroup);
                            listOfErrors.Add(message);
                        }
                        else if (teacherInform.ContainsKey(filtered[i][j].Item.Teacher.CodeOfTeacher) && teacherInform[filtered[i][j].Item.Teacher.CodeOfTeacher].NumberOfClassroom != filtered[i][j].Item.NumberOfClassroom)
                        {
                            message = string.Format("Преподователь {0} не может находиться одновременно в аудиториях {1} и {2} у групп {3} и {4}!",
                               filtered[i][j].Item.Teacher.FIO, filtered[i][j].Item.NumberOfClassroom, teacherInform[filtered[i][j].Item.Teacher.CodeOfTeacher].NumberOfClassroom,
                               filtered[i][j].Item.Group.Single().NameOfGroup, teacherInform[filtered[i][j].Item.Teacher.CodeOfTeacher].Group.Single().NameOfGroup);
                            listOfErrors.Add(message);
                        }
                        else
                        {
                            teacherInform[filtered[i][j].Item.Teacher.CodeOfTeacher] = filtered[i][j].Item;
                        }

                        if (classroomInform.ContainsKey(filtered[i][j].Item.NumberOfClassroom.CodeOfClassroom) && classroomInform[filtered[i][j].Item.NumberOfClassroom.CodeOfClassroom].Subject != filtered[i][j].Item.Subject)
                        {
                            message = string.Format("В аудитории {0} не может быть занятий {1} и {2} у групп {3} и {4} одновременно!",
                                filtered[i][j].Item.NumberOfClassroom, filtered[i][j].Item.Subject.NameOfSubject, classroomInform[filtered[i][j].Item.NumberOfClassroom.CodeOfClassroom].Subject.NameOfSubject,
                                filtered[i][j].Item.Group.Single().NameOfGroup, classroomInform[filtered[i][j].Item.NumberOfClassroom.CodeOfClassroom].Group.Single().NameOfGroup);
                            listOfErrors.Add(message);
                        }
                        else if (classroomInform.ContainsKey(filtered[i][j].Item.NumberOfClassroom.CodeOfClassroom) && classroomInform[filtered[i][j].Item.NumberOfClassroom.CodeOfClassroom].Specifics != filtered[i][j].Item.Specifics)
                        {
                            message = string.Format("В аудитории {0} не может быть {1} и {2} по занятию {3} у групп {4} и {5} одновременно!",
                                filtered[i][j].Item.NumberOfClassroom, filtered[i][j].Item.Specifics, classroomInform[filtered[i][j].Item.NumberOfClassroom.CodeOfClassroom].Specifics,
                                filtered[i][j].Item.Subject.NameOfSubject, filtered[i][j].Item.Group.Single().NameOfGroup, classroomInform[filtered[i][j].Item.NumberOfClassroom.CodeOfClassroom].Group.Single().NameOfGroup);
                            listOfErrors.Add(message);
                        }
                        else
                        {
                            classroomInform[filtered[i][j].Item.NumberOfClassroom.CodeOfClassroom] = filtered[i][j].Item;
                        }
                    }

                    if (filtered[i][j].ItemTwo.Group != null && filtered[i][j].ItemTwo.Teacher != null && filtered[i][j].ItemTwo.Subject != null
                       && filtered[i][j].ItemTwo.Specifics != null && filtered[i][j].ItemTwo.NumberOfClassroom != null)
                    {
                        if (teacherInform.ContainsKey(filtered[i][j].ItemTwo.Teacher.CodeOfTeacher) && teacherInform[filtered[i][j].ItemTwo.Teacher.CodeOfTeacher].Subject != filtered[i][j].ItemTwo.Subject)
                        {
                            message = string.Format("Преподователь {0} не может вести одновременно занятия {1} и {2} у групп {3} и {4}!",
                                filtered[i][j].ItemTwo.Teacher.FIO, filtered[i][j].ItemTwo.Subject.NameOfSubject, teacherInform[filtered[i][j].ItemTwo.Teacher.CodeOfTeacher].Subject.NameOfSubject,
                                filtered[i][j].ItemTwo.Group.Single().NameOfGroup, teacherInform[filtered[i][j].ItemTwo.Teacher.CodeOfTeacher].Group.Single().NameOfGroup);
                            listOfErrors.Add(message);
                        }
                        else if (teacherInform.ContainsKey(filtered[i][j].ItemTwo.Teacher.CodeOfTeacher) && teacherInform[filtered[i][j].ItemTwo.Teacher.CodeOfTeacher].Specifics != filtered[i][j].ItemTwo.Specifics)
                        {
                            message = string.Format("Преподователь {0} не может вести одновременно {1} и {2} по занятию {3} у групп {4} и {5}!",
                               filtered[i][j].ItemTwo.Teacher.FIO, filtered[i][j].ItemTwo.Specifics, teacherInform[filtered[i][j].ItemTwo.Teacher.CodeOfTeacher].Specifics, filtered[i][j].ItemTwo.Subject.NameOfSubject,
                               filtered[i][j].ItemTwo.Group.Single().NameOfGroup, teacherInform[filtered[i][j].ItemTwo.Teacher.CodeOfTeacher].Group.Single().NameOfGroup);
                            listOfErrors.Add(message);
                        }
                        else if (teacherInform.ContainsKey(filtered[i][j].ItemTwo.Teacher.CodeOfTeacher) && teacherInform[filtered[i][j].ItemTwo.Teacher.CodeOfTeacher].NumberOfClassroom != filtered[i][j].ItemTwo.NumberOfClassroom)
                        {
                            message = string.Format("Преподователь {0} не может находиться одновременно в аудиториях {1} и {2} у групп {3} и {4}!",
                               filtered[i][j].ItemTwo.Teacher.FIO, filtered[i][j].ItemTwo.NumberOfClassroom, teacherInform[filtered[i][j].ItemTwo.Teacher.CodeOfTeacher].NumberOfClassroom,
                               filtered[i][j].ItemTwo.Group.Single().NameOfGroup, teacherInform[filtered[i][j].ItemTwo.Teacher.CodeOfTeacher].Group.Single().NameOfGroup);
                            listOfErrors.Add(message);
                        }
                        else
                        {
                            teacherInform[filtered[i][j].ItemTwo.Teacher.CodeOfTeacher] = filtered[i][j].ItemTwo;
                        }

                        if (classroomInform.ContainsKey(filtered[i][j].ItemTwo.NumberOfClassroom.CodeOfClassroom) && classroomInform[filtered[i][j].ItemTwo.NumberOfClassroom.CodeOfClassroom].Subject != filtered[i][j].ItemTwo.Subject)
                        {
                            message = string.Format("В аудитории {0} не может быть занятий {1} и {2} у групп {3} и {4} одновременно!",
                                filtered[i][j].ItemTwo.NumberOfClassroom, filtered[i][j].ItemTwo.Subject.NameOfSubject, classroomInform[filtered[i][j].ItemTwo.NumberOfClassroom.CodeOfClassroom].Subject.NameOfSubject,
                                filtered[i][j].ItemTwo.Group.Single().NameOfGroup, classroomInform[filtered[i][j].ItemTwo.NumberOfClassroom.CodeOfClassroom].Group.Single().NameOfGroup);
                            listOfErrors.Add(message);
                        }
                        else if (classroomInform.ContainsKey(filtered[i][j].ItemTwo.NumberOfClassroom.CodeOfClassroom) && classroomInform[filtered[i][j].ItemTwo.NumberOfClassroom.CodeOfClassroom].Specifics != filtered[i][j].ItemTwo.Specifics)
                        {
                            message = string.Format("В аудитории {0} не может быть {1} и {2} по занятию {3} у групп {4} и {5} одновременно!",
                                filtered[i][j].ItemTwo.NumberOfClassroom, filtered[i][j].ItemTwo.Specifics, classroomInform[filtered[i][j].ItemTwo.NumberOfClassroom.CodeOfClassroom].Specifics,
                                filtered[i][j].ItemTwo.Subject.NameOfSubject, filtered[i][j].ItemTwo.Group.Single().NameOfGroup, classroomInform[filtered[i][j].ItemTwo.NumberOfClassroom.CodeOfClassroom].Group.Single().NameOfGroup);
                            listOfErrors.Add(message);
                        }
                        else
                        {
                            classroomInform[filtered[i][j].ItemTwo.NumberOfClassroom.CodeOfClassroom] = filtered[i][j].ItemTwo;
                        }
                    }
                }
            }
        }
    }
}