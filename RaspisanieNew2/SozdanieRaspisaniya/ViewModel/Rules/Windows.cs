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
            int tempday = SheduleSettings.WeekDayMaxCount - 1;
            int prevday = 0;
            int firstPair = SheduleSettings.SaturdayMaxCount - 2;
            bool hasWindows = false;

            for (int i = 0; i < filtered[0].Count; i++)
            {
                for (int j = filtered.Count - 1; j >= 0; j--)
                {
                    if (j != 0)
                    {
                        day = (int)filtered[j][i].Info.Day;
                        prevday = (int)filtered[j - 1][i].Info.Day;

                        if (day == prevday)
                        {
                            if (filtered[j][i].Item.Subject != null && filtered[j - 1][i].Item.Subject == null /*&& (filtered[j][i].Item.Ndindex == 0 || filtered[j][i].Item.Ndindex == 1)*/)
                            {
                                if (hasWindows)
                                {
                                    listOfErrors.Add(message);
                                }

                                message = string.Format("У группы {0} есть окно {1}!", filtered[j][i].Item.Group.Single().NameOfGroup, dayValue[day - 1]);
                                hasWindows = true;
                            }
                        }
                        else
                        {
                            hasWindows = false;
                        }
                    }
                }
            }
        }
    }
}
