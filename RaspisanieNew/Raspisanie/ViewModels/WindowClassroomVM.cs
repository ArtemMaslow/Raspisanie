using Raspisanie.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ViewModule;
using ViewModule.CSharp;

namespace Raspisanie.ViewModels
{
    class WindowClassroomVM : ViewModelBase
    {
        private readonly INotifyingValue<int> index;

        private readonly INotifyCommand addCommand;
        private readonly INotifyCommand removeCommand;
        private readonly INotifyCommand editCommand;

        public WindowClassroomVM(ObservableCollection<ClassRoom> classClassroom)
        {
            addCommand = this.Factory.CommandSync(Add);
            removeCommand = this.Factory.CommandSync(Remove);
            editCommand = this.Factory.CommandSync(Edit);

            ClassClassroom = classClassroom;

            index = this.Factory.Backing(nameof(Index), -1);
        }


        private void Add()
        {
            var context = new ClassroomVM();
            var wind = new NewClassroom()
            {
                DataContext = context
            };
            wind.ShowDialog();
            if (context.ClassRoom != null)
                ClassClassroom.Add(context.ClassRoom);
        }

        private void Edit()
        {
            if (Index >= 0)
            {
                var classroom = ClassClassroom[Index];
                var context = new ClassroomVM(classroom);
                var wind = new NewClassroom()
                {
                    DataContext = context
                };
                wind.ShowDialog();
                if (context.ClassRoom != null)
                {
                    ClassClassroom[Index] = context.ClassRoom;
                }
            }
        }

        private void Remove()
        {
            if (Index >= 0)
                ClassClassroom.RemoveAt(Index);
        }

        public ObservableCollection<ClassRoom> ClassClassroom { get; }
        public ICommand AddCommand => addCommand;
        public ICommand RemoveCommand => removeCommand;
        public ICommand EditCommand => editCommand;

        public int Index { get { return index.Value; } set { index.Value = value; } }
    }
}
