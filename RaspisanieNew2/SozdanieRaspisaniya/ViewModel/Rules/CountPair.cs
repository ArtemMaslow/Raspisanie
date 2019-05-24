using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SozdanieRaspisaniya.ViewModel.Rules
{
    public class CountPair : IRule
    {
        string message = "";
        string[] dayValue = { "в понедельник", "во вторник", "в среду ", "в четверг", "в пятницу", "в субботу" };
        int maxPair = 5;
        int maxLecturePair = 3;
        public void Check(ObservableCollection<ObservableCollection<DropItem>> filtered, ref List<string> listOfErrors)
        {
            int day;
            int tempday = 0;
            int countPair = 0;
            int countPairNum = 0;
            int countPairDenum = 0;
            int countLecturePair = 0;
            int countLecturePairNum = 0;
            int countLecturePairDenum = 0;
            
            for (int i = 2; i < filtered[0].Count; i++)
            {
                for (int j = 0; j < filtered.Count; j++)
                {
                    day = j / SheduleSettings.WeekDayMaxCount;
                    if (day != tempday)
                    {
                        if (countPair > maxPair)
                        {
                            message = string.Format("У группы {0} по превышено кол-во пар {1}!", filtered[j][i].Item.Group.First().NameOfGroup, dayValue[day-1]);
                            listOfErrors.Add(message);
                        }

                        if (countPair + countPairNum > maxPair)
                        {
                            message = string.Format("У группы {0} по нечетным неделям превышено кол-во пар {1}!", filtered[j][i].Item.Group.First().NameOfGroup, dayValue[day-1]);
                            listOfErrors.Add(message);
                        }

                        if (countPair + countPairDenum > maxPair)
                        {
                            message = string.Format("У группы {0} по четным неделям превышено кол-во пар {1}!", filtered[j][i].Item.Group.First().NameOfGroup, dayValue[day-1]);
                            listOfErrors.Add(message);
                        }

                        if (countLecturePair + countLecturePairNum > maxLecturePair)
                        {
                            message = string.Format("У группы {0} по нечетным неделям превышено кол-во лекционных пар {1}!", filtered[j][i].Item.Group.First().NameOfGroup, dayValue[day-1]);
                            listOfErrors.Add(message);
                        }

                        if (countLecturePair + countLecturePairDenum > maxLecturePair)
                        {
                            message = string.Format("У группы {0} по четным неделям превышено кол-во лекционных пар {1}!", filtered[j][i].Item.Group.First().NameOfGroup, dayValue[day-1]);
                            listOfErrors.Add(message);
                        }

                        tempday = day;
                        countPair = 0;
                        countPairNum = 0;
                        countPairDenum = 0;
                        countLecturePair = 0;
                        countLecturePairNum = 0;
                        countLecturePairDenum = 0;
                        //j += 1;
                    }

                    if (filtered[j][i].Item.Ndindex == 0)
                    {
                        if (filtered[j][i].Item.Group != null && filtered[j][i].Item.Teacher != null && filtered[j][i].Item.Subject != null
                            && filtered[j][i].Item.Specifics != null && filtered[j][i].Item.NumberOfClassroom != null)
                        {
                            countPair++;
                            if (filtered[j][i].Item.Specifics.Equals(SheduleSettings.specifics[0]))
                            {
                                countLecturePair++;
                            }
                        }
                    }
                    if (filtered[j][i].Item.Ndindex == 1)
                    {
                        if (filtered[j][i].Item.Group != null && filtered[j][i].Item.Teacher != null && filtered[j][i].Item.Subject != null
                            && filtered[j][i].Item.Specifics != null && filtered[j][i].Item.NumberOfClassroom != null)
                        {
                            countPairNum++;
                            if (filtered[j][i].Item.Specifics.Equals(SheduleSettings.specifics[0]))
                            {
                                countLecturePairNum++;
                            }
                        }
                    }
                    if (filtered[j][i].ItemTwo.Ndindex == -1)
                    {
                        if (filtered[j][i].ItemTwo.Group != null && filtered[j][i].ItemTwo.Teacher != null && filtered[j][i].ItemTwo.Subject != null 
                            && filtered[j][i].ItemTwo.Specifics != null && filtered[j][i].ItemTwo.NumberOfClassroom != null)
                        {
                            countPairDenum++;
                            if (filtered[j][i].ItemTwo.Specifics.Equals(SheduleSettings.specifics[0]))
                            {
                                countLecturePairDenum++;
                            }
                        }
                    }
                }
            }
        }
    }
}
