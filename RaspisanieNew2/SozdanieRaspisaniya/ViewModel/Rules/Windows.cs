using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SozdanieRaspisaniya.ViewModel.Rules
{
    public class Windows : IRule
    {
        string message = "";
        string[] dayValue = { "в понедельник", "во вторник", "в среду ", "в четверг", "в пятницу", "в субботу" };
        HashSet<(string, string, int)> win = new HashSet<(string, string, int)>();

        bool HasSubject(DropItem item)
        {
            return item.Item.Subject != null;
        }

        bool HasSubjectOnSecondWeek(DropItem item)
            => item.State != 0 ? item.ItemTwo.Subject != null : item.Item.Subject != null;

        public void Check(ObservableCollection<ObservableCollection<DropItem>> filtered, ref List<string> listOfErrors)
        {
            bool hadPairs = false;
            bool isWindowOrEmpty = false;

            for (int i = 0; i < filtered[0].Count; i++)
            {
                for (int j = 0; j < filtered.Count; j++)
                {
                    var info = filtered[j][i].Info;
                    if (info.Pair == 1)
                    {
                        isWindowOrEmpty = false;
                        hadPairs = false;
                    }
                    if (HasSubject(filtered[j][i]))
                    {
                        if (hadPairs && isWindowOrEmpty)
                        {
                            win.Add((filtered[j][i].Item.Group.First().NameOfGroup, dayValue[(int)info.Day - 1], info.Pair));
                            listOfErrors.Add(string.Format("У группы {0} есть окно {1}!", filtered[j][i].Item.Group.First().NameOfGroup, dayValue[(int)info.Day - 1]));
                        }
                        hadPairs = true;
                        isWindowOrEmpty = false;
                    }
                    else
                    {
                        isWindowOrEmpty = true;
                    }
                }
            }

            for (int i = 0; i < filtered[0].Count; i++)
            {
                for (int j = 0; j < filtered.Count; j++)
                {
                    var info = filtered[j][i].Info;
                    if (info.Pair == 1)
                    {
                        isWindowOrEmpty = false;
                        hadPairs = false;
                    }
                    if (HasSubjectOnSecondWeek(filtered[j][i]))
                    {
                        if (hadPairs && isWindowOrEmpty && !win.Contains((filtered[j][i].Item.Group.First().NameOfGroup, dayValue[(int)info.Day - 1], info.Pair)))
                        {
                            win.Add((filtered[j][i].Item.Group.First().NameOfGroup, dayValue[(int)info.Day - 1], info.Pair));
                            listOfErrors.Add(string.Format("У группы {0} есть окно {1} по знаменателю!", filtered[j][i].Item.Group.First().NameOfGroup, dayValue[(int)info.Day - 1]));
                        }

                        hadPairs = true;
                        isWindowOrEmpty = false;
                    }
                    else
                    {
                        isWindowOrEmpty = true;
                    }
                }
            }
        }
    }
}
