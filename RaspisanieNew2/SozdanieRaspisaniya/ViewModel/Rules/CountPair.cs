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
            
            for (int i = 0; i < filtered.Count; i++)
            {
                day = i / 6;
                for (int j = 0; j < filtered[0].Count; j++)
                {
                    if (day != tempday)
                    {
                        if (countPair > 5)
                        {
                            message = string.Format("У группы {0} по превышено кол-во пар {1}!", filtered[i][j].Item.Group.Single().NameOfGroup, dayValue[day]);
                            listOfErrors.Add(message);
                        }

                        if (countPair + countPairNum > 5)
                        {
                            message = string.Format("У группы {0} по нечетным неделям превышено кол-во пар {1}!", filtered[i][j].Item.Group.Single().NameOfGroup, dayValue[day]);
                            listOfErrors.Add(message);
                        }

                        if (countPair + countPairDenum > 5)
                        {
                            message = string.Format("У группы {0} по четным неделям превышено кол-во пар {1}!", filtered[i][j].Item.Group.Single().NameOfGroup, dayValue[day]);
                            listOfErrors.Add(message);
                        }

                        if (countLecturePair + countLecturePairNum > 3)
                        {
                            message = string.Format("У группы {0} по нечетным неделям превышено кол-во лекционных пар {1}!", filtered[i][j].Item.Group.Single().NameOfGroup, dayValue[day]);
                            listOfErrors.Add(message);
                        }

                        if (countLecturePair + countLecturePairDenum > 3)
                        {
                            message = string.Format("У группы {0} по четным неделям превышено кол-во лекционных пар {1}!", filtered[i][j].Item.Group.Single().NameOfGroup, dayValue[day]);
                            listOfErrors.Add(message);
                        }

                        tempday = day;
                        countPair = 0;
                        countPairNum = 0;
                        countPairDenum = 0;
                        countLecturePair = 0;
                        countLecturePairNum = 0;
                        countLecturePairDenum = 0;
                    }

                    if (filtered[i][j].Item.Ndindex == 0)
                    {
                        if (filtered[i][j].Item.Group != null && filtered[i][j].Item.Teacher != null && filtered[i][j].Item.Subject != null
                            && filtered[i][j].Item.Specifics != null && filtered[i][j].Item.NumberOfClassroom != null)
                        {
                            countPair++;
                            if (filtered[i][j].Item.Specifics.Equals(SheduleSettings.specifics[0]))
                            {
                                countLecturePair++;
                            }
                        }
                    }
                    if (filtered[i][j].Item.Ndindex == 1)
                    {
                        if (filtered[i][j].Item.Group != null && filtered[i][j].Item.Teacher != null && filtered[i][j].Item.Subject != null
                            && filtered[i][j].Item.Specifics != null && filtered[i][j].Item.NumberOfClassroom != null)
                        {
                            countPairNum++;
                            if (filtered[i][j].Item.Specifics.Equals(SheduleSettings.specifics[0]))
                            {
                                countLecturePairNum++;
                            }
                        }
                    }
                    if (filtered[i][j].ItemTwo.Ndindex == -1)
                    {
                        if (filtered[i][j].ItemTwo.Group != null && filtered[i][j].ItemTwo.Teacher != null && filtered[i][j].ItemTwo.Subject != null && filtered[i][j].ItemTwo.Specifics != null && filtered[i][j].ItemTwo.NumberOfClassroom != null)
                        {
                            countPairDenum++;
                            if (filtered[i][j].ItemTwo.Specifics.Equals(SheduleSettings.specifics[0]))
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
