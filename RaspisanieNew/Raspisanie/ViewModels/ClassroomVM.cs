using Raspisanie.Models;
using System;
using System.Windows.Input;
using ViewModule;
using ViewModule.CSharp;
using System.Windows;

namespace Raspisanie.ViewModels
{
    class ClassroomVM : ViewModelBase
    {
        private readonly INotifyingValue<string> numberOfClassroom;
        private readonly INotifyingValue<int> capacity;
        private readonly INotifyingValue<int> codeOfClassroom;
        private readonly INotifyingValue<string> specifics;

        private readonly INotifyCommand saveClassroom;

        public ClassRoom ClassRoom
        {
            get; private set;
        }

        public ClassroomVM()
        {
            numberOfClassroom = this.Factory.Backing(nameof(NumberOfClassroom),"");
            capacity = this.Factory.Backing(nameof(Capacity), 0);
            codeOfClassroom = this.Factory.Backing(nameof(CodeOfClassroom), 0);
            specifics = this.Factory.Backing(nameof(Specifics),"");

            saveClassroom = this.Factory.CommandSyncParam<Window>(SaveAndClose);
        }

        public ClassroomVM(ClassRoom classroom) : this()
        {
            numberOfClassroom.Value = classroom.NumberOfClassroom;
            capacity.Value = classroom.Capacity;
            codeOfClassroom.Value = classroom.CodeOfClassroom;
            specifics.Value = classroom.Specifics;
        }

        private void SaveAndClose(Window obj)
        {
            if (!string.IsNullOrWhiteSpace(Specifics) && CodeOfClassroom > 0 && Capacity > 0 && !string.IsNullOrWhiteSpace(NumberOfClassroom))
                ClassRoom = new ClassRoom {
                    NumberOfClassroom = NumberOfClassroom,
                    CodeOfClassroom = CodeOfClassroom,
                    Capacity= Capacity,
                    Specifics = Specifics };
            obj.Close();
        }

        public ICommand SaveCommand => saveClassroom;
        public string NumberOfClassroom { get { return numberOfClassroom.Value; } set { numberOfClassroom.Value = value; } }
        public int Capacity { get { return capacity.Value; } set { capacity.Value = value; } }
        public int CodeOfClassroom { get { return codeOfClassroom.Value; } set { codeOfClassroom.Value = value; } }
        public string Specifics { get { return specifics.Value; } set { specifics.Value = value; } }
    }
}
