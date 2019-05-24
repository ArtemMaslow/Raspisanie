using Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SozdanieRaspisaniya.ViewModel.Rules
{
    public class NoOverlay : IRule
    {
        private Dictionary<int, DropInformation> teacherInform = new Dictionary<int, DropInformation>();
        private Dictionary<int, DropInformation> teacherInformNum = new Dictionary<int, DropInformation>();
        private Dictionary<int, DropInformation> teacherInformDenum = new Dictionary<int, DropInformation>();

        private Dictionary<int, DropInformation> classroomInform = new Dictionary<int, DropInformation>();
        private Dictionary<int, DropInformation> classroomInformNum = new Dictionary<int, DropInformation>();
        private Dictionary<int, DropInformation> classroomInformDenum = new Dictionary<int, DropInformation>();

        private string message = "";

        public void Check(ObservableCollection<ObservableCollection<DropItem>> filtered, ref List<string> listOfErrors)
        {
            for (int i = 0; i < filtered.Count; i++)
            {
                teacherInform.Clear();
                teacherInformNum.Clear();
                teacherInformDenum.Clear();
                classroomInform.Clear();
                classroomInformNum.Clear();
                classroomInformDenum.Clear();
                for (int j = 0; j < filtered[0].Count; j++)
                {
                    var group = filtered[i][j].Item.Group;
                    var teacher = filtered[i][j].Item.Teacher;
                    var subject = filtered[i][j].Item.Subject;
                    var specific = filtered[i][j].Item.Specifics;
                    var classroom = filtered[i][j].Item.NumberOfClassroom;
                    var item = filtered[i][j].Item;

                    if (group != null && teacher != null && subject != null && specific != null && classroom != null)
                    {
                        if (teacherInform.ContainsKey(teacher.CodeOfTeacher) && teacherInformNum.ContainsKey(teacher.CodeOfTeacher))
                        {
                            message = string.Format("Преподователь {0} не может быть одновременно у групп {1} и {2}!",
                                    teacher.FIO, teacherInform[teacher.CodeOfTeacher].Group.First().NameOfGroup, teacherInformNum[teacher.CodeOfTeacher].Group.First().NameOfGroup);
                            listOfErrors.Add(message);
                        }
                        else
                        {
                            if (teacherInform.ContainsKey(teacher.CodeOfTeacher) && teacherInform[teacher.CodeOfTeacher].Subject != subject)
                            {
                                message = string.Format("Преподователь {0} не может вести одновременно занятия {1} и {2} у групп {3} и {4}!",
                                    teacher.FIO, subject.NameOfSubject, teacherInform[teacher.CodeOfTeacher].Subject.NameOfSubject,
                                    group.First().NameOfGroup, teacherInform[teacher.CodeOfTeacher].Group.First().NameOfGroup);
                                listOfErrors.Add(message);
                            }
                            else if (teacherInform.ContainsKey(teacher.CodeOfTeacher) && teacherInform[teacher.CodeOfTeacher].Specifics != specific)
                            {
                                message = string.Format("Преподователь {0} не может вести одновременно {1} и {2} по занятию {3} у групп {4} и {5}!",
                                   teacher.FIO, specific, teacherInform[teacher.CodeOfTeacher].Specifics, subject.NameOfSubject,
                                   group.First().NameOfGroup, teacherInform[teacher.CodeOfTeacher].Group.First().NameOfGroup);
                                listOfErrors.Add(message);
                            }
                            else if (teacherInform.ContainsKey(teacher.CodeOfTeacher) && teacherInform[teacher.CodeOfTeacher].NumberOfClassroom != classroom)
                            {
                                message = string.Format("Преподователь {0} не может находиться одновременно в аудиториях {1} и {2} у групп {3} и {4}!",
                                   teacher.FIO, classroom, teacherInform[teacher.CodeOfTeacher].NumberOfClassroom,
                                   group.First().NameOfGroup, teacherInform[teacher.CodeOfTeacher].Group.First().NameOfGroup);
                                listOfErrors.Add(message);
                            }
                            else
                            {
                                if (item.Ndindex == 0)
                                {
                                    teacherInform[teacher.CodeOfTeacher] = item;
                                }
                                else if (item.Ndindex == 1)
                                {
                                    teacherInformNum[teacher.CodeOfTeacher] = item;
                                }
                            }
                        }

                        if (classroomInform.ContainsKey(classroom.CodeOfClassroom) && classroomInformNum.ContainsKey(classroom.CodeOfClassroom))
                        {
                            message = string.Format("В аудитории {0} не может быть занятий {1} и {2} у групп {3} и {4} одновременно!",
                                    classroom, subject.NameOfSubject, classroomInform[classroom.CodeOfClassroom].Subject.NameOfSubject,
                                    group.First().NameOfGroup, classroomInformNum[classroom.CodeOfClassroom].Group.First().NameOfGroup);
                            listOfErrors.Add(message);
                        }
                        else
                        {
                            if (classroomInform.ContainsKey(classroom.CodeOfClassroom) && classroomInform[classroom.CodeOfClassroom].Subject != subject)
                            {
                                message = string.Format("В аудитории {0} не может быть занятий {1} и {2} у групп {3} и {4} одновременно!",
                                    classroom, subject.NameOfSubject, classroomInform[classroom.CodeOfClassroom].Subject.NameOfSubject,
                                    group.First().NameOfGroup, classroomInform[classroom.CodeOfClassroom].Group.First().NameOfGroup);
                                listOfErrors.Add(message);
                            }
                            else if (classroomInform.ContainsKey(classroom.CodeOfClassroom) && classroomInform[classroom.CodeOfClassroom].Specifics != specific)
                            {
                                message = string.Format("В аудитории {0} не может быть {1} и {2} по занятию {3} у групп {4} и {5} одновременно!",
                                    classroom, specific, classroomInform[classroom.CodeOfClassroom].Specifics,
                                    subject.NameOfSubject, group.First().NameOfGroup, classroomInform[classroom.CodeOfClassroom].Group.First().NameOfGroup);
                                listOfErrors.Add(message);
                            }
                            else
                            {
                                if (item.Ndindex == 0)
                                {
                                    classroomInform[classroom.CodeOfClassroom] = item;
                                }
                                else if (item.Ndindex == 1)
                                {
                                    classroomInformNum[classroom.CodeOfClassroom] = item;
                                }
                            }
                        }
                    }

                    var groupTwo = filtered[i][j].ItemTwo.Group;
                    var teacherTwo = filtered[i][j].ItemTwo.Teacher;
                    var subjectTwo = filtered[i][j].ItemTwo.Subject;
                    var specificTwo = filtered[i][j].ItemTwo.Specifics;
                    var classroomTwo = filtered[i][j].ItemTwo.NumberOfClassroom;
                    var itemTwo = filtered[i][j].ItemTwo;

                    if (groupTwo != null && teacherTwo != null && subjectTwo != null && specificTwo != null && classroomTwo != null)
                    {
                        if (teacherInform.ContainsKey(teacherTwo.CodeOfTeacher) && teacherInformDenum.ContainsKey(teacherTwo.CodeOfTeacher))
                        {
                            message = string.Format("Преподователь {0} не может быть одновременно у групп {1} и {2}!",
                                   teacherTwo.FIO, teacherInform[teacher.CodeOfTeacher].Group.First().NameOfGroup, teacherInformNum[teacher.CodeOfTeacher].Group.First().NameOfGroup);
                            listOfErrors.Add(message);
                        }
                        else
                        {
                            if (teacherInformDenum.ContainsKey(teacherTwo.CodeOfTeacher) && teacherInformDenum[teacherTwo.CodeOfTeacher].Subject != subject)
                            {
                                message = string.Format("Преподователь {0} не может вести одновременно занятия {1} и {2} у групп {3} и {4}!",
                                    teacherTwo.FIO, subjectTwo.NameOfSubject, teacherInform[teacher.CodeOfTeacher].Subject.NameOfSubject,
                                    groupTwo.First().NameOfGroup, teacherInformDenum[teacherTwo.CodeOfTeacher].Group.First().NameOfGroup);
                                listOfErrors.Add(message);
                            }
                            else if (teacherInformDenum.ContainsKey(teacherTwo.CodeOfTeacher) && teacherInformDenum[teacherTwo.CodeOfTeacher].Specifics != specificTwo)
                            {
                                message = string.Format("Преподователь {0} не может вести одновременно {1} и {2} по занятию {3} у групп {4} и {5}!",
                                   teacherTwo.FIO, specificTwo, teacherInformDenum[teacherTwo.CodeOfTeacher].Specifics, subject.NameOfSubject,
                                   groupTwo.First().NameOfGroup, teacherInformDenum[teacherTwo.CodeOfTeacher].Group.First().NameOfGroup);
                                listOfErrors.Add(message);
                            }
                            else if (teacherInformDenum.ContainsKey(teacherTwo.CodeOfTeacher) && teacherInformDenum[teacherTwo.CodeOfTeacher].NumberOfClassroom != classroom)
                            {
                                message = string.Format("Преподователь {0} не может находиться одновременно в аудиториях {1} и {2} у групп {3} и {4}!",
                                   teacherTwo.FIO, classroomTwo, teacherInformDenum[teacherTwo.CodeOfTeacher].NumberOfClassroom,
                                   groupTwo.First().NameOfGroup, teacherInformDenum[teacherTwo.CodeOfTeacher].Group.First().NameOfGroup);
                                listOfErrors.Add(message);
                            }
                            else
                            {
                                teacherInformDenum[teacherTwo.CodeOfTeacher] = itemTwo;
                            }
                        }
                        if (classroomInform.ContainsKey(classroomTwo.CodeOfClassroom) && classroomInformDenum.ContainsKey(classroomTwo.CodeOfClassroom))
                        {
                            message = string.Format("В аудитории {0} не может быть занятий {1} и {2} у групп {3} и {4} одновременно!",
                                    classroom, subject.NameOfSubject, classroomInform[classroom.CodeOfClassroom].Subject.NameOfSubject,
                                    group.First().NameOfGroup, classroomInformDenum[classroom.CodeOfClassroom].Group.First().NameOfGroup);
                            listOfErrors.Add(message);
                        }
                        else
                        {
                            if (classroomInformDenum.ContainsKey(classroomTwo.CodeOfClassroom) && classroomInformDenum[classroomTwo.CodeOfClassroom].Subject != subject)
                            {
                                message = string.Format("В аудитории {0} не может быть занятий {1} и {2} у групп {3} и {4} одновременно!",
                                    classroomTwo, subjectTwo.NameOfSubject, classroomInformDenum[classroomTwo.CodeOfClassroom].Subject.NameOfSubject,
                                    groupTwo.First().NameOfGroup, classroomInformDenum[classroomTwo.CodeOfClassroom].Group.First().NameOfGroup);
                                listOfErrors.Add(message);
                            }
                            else if (classroomInformDenum.ContainsKey(classroomTwo.CodeOfClassroom) && classroomInformDenum[classroomTwo.CodeOfClassroom].Specifics != specific)
                            {
                                message = string.Format("В аудитории {0} не может быть {1} и {2} по занятию {3} у групп {4} и {5} одновременно!",
                                    classroomTwo, specificTwo, classroomInformDenum[classroomTwo.CodeOfClassroom].Specifics,
                                    subjectTwo.NameOfSubject, groupTwo.First().NameOfGroup, classroomInformDenum[classroomTwo.CodeOfClassroom].Group.First().NameOfGroup);
                                listOfErrors.Add(message);
                            }
                            else
                            {
                                classroomInformDenum[classroomTwo.CodeOfClassroom] = itemTwo;
                            }
                        }
                    }
                }
            }
        }
    }
}