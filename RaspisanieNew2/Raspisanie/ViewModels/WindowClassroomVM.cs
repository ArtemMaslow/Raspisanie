using Raspisanie.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
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

        public WindowClassroomVM(ObservableCollection<ClassRoom> classClassroom, ObservableCollection<Department> departments)
        {
            addCommand = this.Factory.CommandSync(Add);
            removeCommand = this.Factory.CommandSync(Remove);
            editCommand = this.Factory.CommandSync(Edit);

            ClassClassroom = classClassroom;
            this.departments = departments;

            index = this.Factory.Backing(nameof(Index), -1);
        }


        private void Add()
        {
            var context = new ClassroomVM(departments.ToArray());
            var wind = new NewClassroom()
            {
                DataContext = context
            };
            wind.ShowDialog();
            if (context.ClassRoom != null)
                if (RequestToDataBase.Instance.requestInsertIntoClassroom(context.ClassRoom))
                {
                    ClassClassroom.Add(context.ClassRoom);
                }
        }

        private void Edit()
        {
            if (Index >= 0)
            {
                var classroom = ClassClassroom[Index];
                var context = new ClassroomVM(classroom, departments.ToArray());
                var wind = new NewClassroom()
                {
                    DataContext = context
                };
                wind.ShowDialog();
                if (context.ClassRoom != null)
                {
                    if (RequestToDataBase.Instance.requestUpdateClassroom(context.ClassRoom, ClassClassroom, Index))
                    {
                        ClassClassroom[Index] = context.ClassRoom;
                    }
                }
            }
        }

        private void Remove()
        {
            if (Index >= 0)
                if (RequestToDataBase.Instance.requestDeleteFromClassroom(ClassClassroom, Index))
                {
                    ClassClassroom.RemoveAt(Index);
                }
        }

        private ObservableCollection<Department> departments { get; }

        public ObservableCollection<ClassRoom> ClassClassroom { get; }
        
        public ICommand AddCommand => addCommand;
        public ICommand RemoveCommand => removeCommand;
        public ICommand EditCommand => editCommand;

        public int Index { get { return index.Value; } set { index.Value = value; } }
    }
}
