using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SozdanieRaspisaniya.ViewModel.Rules
{
    public class Windows : IRule
    {
        string message = "";
        string[] dayValue = { "в понедельник", "во вторник", "в среду ", "в четверг", "в пятницу", "в субботу" };

        public void Check(ObservableCollection<ObservableCollection<DropItem>> filtered, ref List<string> listOfErrors)
        {
            int day;
            int tempday = 0;
            for (int i = 0; i < filtered[0].Count; i++)
            {
                for (int j = 2; j < filtered.Count; j++)
                {
                    day = j / 6;
                    if (day != tempday)
                    {
                        tempday = day;
                        j += 2;
                    }

                    if (filtered[j][i].Item.Subject != null && filtered[j - 1][i].Item.Subject == null && filtered[j - 2][i].Item.Subject != null && filtered[j][i].Item.Ndindex == 0 && filtered[j - 2][i].Item.Ndindex == 0)
                    {
                        message = string.Format("У группы {0} есть окно {1}!", filtered[j][i].Item.Group.Single().NameOfGroup, dayValue[day]);
                        listOfErrors.Add(message);
                    }

                    //if (filtered[j][i].Item.Subject != null && filtered[j - 1][i].ItemTwo.Subject == null && filtered[j - 2][i].Item.Subject != null && filtered[j][i].Item.Ndindex == 0 && filtered[j - 2][i].Item.Ndindex == 0)
                    //{
                    //    message = string.Format("У группы {0} есть окно по четным неделям {1}!", filtered[j][i].Item.Group.Single().NameOfGroup, dayValue[day]);
                    //    listOfErrors.Add(message);
                    //}
                }
            }
        }
    }
}
