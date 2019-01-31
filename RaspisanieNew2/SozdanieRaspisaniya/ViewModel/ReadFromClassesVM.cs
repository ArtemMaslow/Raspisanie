using Raspisanie.Models;
using System.Windows.Input;
using ViewModule;
using ViewModule.CSharp;
using System.Windows;
using System.Linq;
using static ViewModule.Validation.CSharp.Validators;

namespace SozdanieRaspisaniya.ViewModel
{
    class ReadFromClassesVM : ViewModelBase
    {
        private readonly INotifyingValue<string> nameOfSchedule;
        private readonly INotifyCommand readSchedule;

        public ReadFromClassesVM(string[] namesOfSchedule)
        {
            NamesOfSchedule = namesOfSchedule;

            nameOfSchedule = this.Factory.Backing<string>(nameof(NameOfSchedule),null);

            readSchedule = this.Factory.CommandSyncParam<Window>(ReadAndClose);
        }

        private void ReadAndClose(Window obj)
        {
            Name = NameOfSchedule;
            obj.Close();
        }

        public ICommand SaveCommand => readSchedule;
        public string NameOfSchedule { get { return nameOfSchedule.Value; } set { nameOfSchedule.Value = value; } }

        public string[] NamesOfSchedule { get; }

        public string Name { get; private set; }
    }
}
