using Microsoft.FSharp.Core;
using System.Linq;
using System.Windows.Input;
using ViewModule;
using ViewModule.CSharp;
using static ViewModule.Validation.CSharp.Validators;

namespace SozdanieRaspisaniya.ViewModel
{
    class SaveScheduleVM : ViewModelBase
    {
        private readonly INotifyingValue<string> saveWithNewName;
        private readonly INotifyingValue<string> rewriteExistSchedule;

        private readonly INotifyCommand saveSchedule;

        public SaveScheduleVM(string[] existSchedule)
        {
            ExistSchedule = existSchedule;

            var defaultName = "default" + ExistSchedule.Length;
            var validator = Custom<string>(p => !ExistSchedule.Contains(p), "Shouldn't be as existing value");

            rewriteExistSchedule = this.Factory.Backing<string>(nameof(RewriteExistScheduleName), null);
            saveWithNewName = this.Factory.Backing<string>(nameof(SaveWithNewName), defaultName, validator);

            saveSchedule = this.Factory.CommandSyncParam<bool>(SaveAndClose);
        }

        private void SaveAndClose(bool isNewName)
        {   
            if (isNewName)
                Name = FSharpChoice<string, string>.NewChoice1Of2(SaveWithNewName);
            else
                Name = FSharpChoice<string, string>.NewChoice2Of2(RewriteExistScheduleName);
        }
    

        public ICommand SaveCommand => saveSchedule;

        public string RewriteExistScheduleName { get { return rewriteExistSchedule.Value; } set { rewriteExistSchedule.Value = value; } }
        public string SaveWithNewName { get { return saveWithNewName.Value; } set { saveWithNewName.Value = value; } }

        public string[] ExistSchedule { get; }

        public FSharpChoice<string, string> Name { get; private set; }
    }
}
