using Raspisanie.Models;
using System.Windows.Input;
using ViewModule;
using ViewModule.CSharp;
using System.Windows;
using System.Linq;
using static ViewModule.Validation.CSharp.Validators;

namespace SozdanieRaspisaniya.ViewModel
{
    class SaveScheduleVM : ViewModelBase
    {
        private readonly INotifyingValue<string> saveWithNewName;
        private readonly INotifyingValue<string> rewriteExistSchedule;
        private readonly INotifyCommand showHiddenElement;
        private readonly INotifyCommand saveSchedule;

        bool isNewName = true;

        public SaveScheduleVM(string[] existSchedule)
        {
            ExistSchedule = existSchedule;

            rewriteExistSchedule = this.Factory.Backing<string>(nameof(RewriteExistScheduleName), null);
            saveWithNewName = this.Factory.Backing<string>(nameof(SaveWithNewName), null);
            saveSchedule = this.Factory.CommandSyncParam<Window>(SaveAndClose);
            showHiddenElement = this.Factory.CommandSync(ShowHidenElement);

        }

        private void ShowHidenElement()
        {
            isNewName = false;
        }

        private void SaveAndClose(Window obj)
        {
            if (isNewName)
            {
                if (SaveWithNewName != null)
                    Name = SaveWithNewName;
            }
            else
            {
                if (RewriteExistScheduleName != null)
                {
                    Name = RewriteExistScheduleName;
                    RequestToDataBase.Instance.clearClasses(Name);
                }
            }
            obj.Close();
        }

        public ICommand SaveCommand => saveSchedule;
        public ICommand ShowCommand => showHiddenElement;

        public string RewriteExistScheduleName { get { return rewriteExistSchedule.Value; } set { rewriteExistSchedule.Value = value; } }
        public string SaveWithNewName { get { return saveWithNewName.Value; } set { saveWithNewName.Value = value; } }

        public string[] ExistSchedule { get; }

        public string Name { get; private set; }
    }
}
