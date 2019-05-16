using Models;
using SozdanieRaspisaniya.ViewModel.Rules;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ViewModule;
using ViewModule.CSharp;

namespace SozdanieRaspisaniya.ViewModel
{
    public class ChooseRulesVM : ViewModelBase
    {
        private readonly INotifyCommand saveChooseRules;

        public ChooseRulesVM(List<IRule> rules)
        {
            HashSet<IRule> rl = new HashSet<IRule>(rules.Select(r => r));
            Rules = rules.Select(r => new ChooseViewHelper<IRule>
            {
                IsSelected = rl.Contains(r),
                Value = r

            }).ToArray();

            saveChooseRules = this.Factory.CommandSyncParam<Window>(SaveAndClose);
        }

        public ChooseViewHelper<IRule>[] Rules { get; }
        public IRule[] SelectedRules { get; private set; }

        public void SaveAndClose(Window obj)
        {
            if (Rules.Where(r => r.IsSelected).Count() > 0)
            {
                SelectedRules = Rules.Where(r => r.IsSelected).Select(r => r.Value).ToArray();
                obj.DialogResult = true;
                obj.Close();
            }
            else
            {
                MessageBox.Show("Выберите минимум одно правило или закройте окно!");
            }
        }

        public ICommand SaveCommand => saveChooseRules;
    }
}
