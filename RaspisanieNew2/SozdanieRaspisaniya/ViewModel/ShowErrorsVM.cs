using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using ViewModule;
using ViewModule.CSharp;

namespace SozdanieRaspisaniya.ViewModel
{
    public class ShowErrorsVM : ViewModelBase
    {
        private readonly INotifyCommand close;

        public ShowErrorsVM(List<string> stringErrors)
        {
            StringErrors = stringErrors;
            close = this.Factory.CommandSyncParam<Window>(Close);
        }

        private void Close(Window obj)
        {
            obj.Close();
        }

        public List<string> StringErrors { get; }
        public ICommand CloseCommand => close;

    }
}
