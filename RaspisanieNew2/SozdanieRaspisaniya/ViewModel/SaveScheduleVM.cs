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

        bool isShow = false;

        public SaveScheduleVM(string[] existSchedule)
        {
            ExistSchedule = existSchedule;

            rewriteExistSchedule = this.Factory.Backing<string>(nameof(RewriteExistScheduleName), null);
            saveWithNewName = this.Factory.Backing<string>(nameof(SaveWithNewName), null);
            saveSchedule = this.Factory.CommandSyncParam<Window>(ReadAndClose);
            showHiddenElement = this.Factory.CommandSyncParam<Window>(ShowHidenElement);

        }

        private void ShowHidenElement(Window obj)
        {
            isShow = true;
        }

        private void ReadAndClose(Window obj)
        {
            Name = SaveWithNewName;
            //  RequestToDataBase.Instance.clearClasses(Name);

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
