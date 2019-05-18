using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SozdanieRaspisaniya.ViewModel.Rules
{
    public class RuleEngine
    {
        List<IRule> listOfRules = new List<IRule>();
        List<string> listOfErrors = new List<string>();
        private ObservableCollection<ObservableCollection<DropItem>> filtered { get; }

        public RuleEngine(ObservableCollection<ObservableCollection<DropItem>> filtered)
        {
            this.filtered = filtered;
        }

        public void AddRule(IRule rule)
        {
            listOfRules.Add(rule);
        }

        public void ApplyRules()
        {
            foreach (var rule in listOfRules)
            {
                if (listOfRules.Count > 0)
                {
                    rule.Check(filtered, ref listOfErrors);
                }
            }
        }

        public List<string> CreateStringErrors()
        {
            if (listOfErrors.Count > 0)
            {
                return listOfErrors;
            }
            else
            {
                listOfErrors.Add("Ошибки в расписании не обнаржены.");
                return listOfErrors;
            }
        }
    }
}
