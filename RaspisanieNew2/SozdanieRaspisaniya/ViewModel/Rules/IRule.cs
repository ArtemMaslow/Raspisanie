using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SozdanieRaspisaniya.ViewModel.Rules
{
    public interface IRule
    {
        void Check(ObservableCollection<ObservableCollection<DropItem>> filtered, ref List<string> listOfErrors);
    }
}
