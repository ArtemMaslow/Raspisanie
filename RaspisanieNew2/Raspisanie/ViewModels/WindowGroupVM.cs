using Raspisanie.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using ViewModule;
using ViewModule.CSharp;

namespace Raspisanie.ViewModels
{
    class WindowGroupVM : ViewModelBase
    {
        private readonly INotifyingValue<int> index;

        private readonly INotifyCommand addCommand;
        private readonly INotifyCommand removeCommand;
        private readonly INotifyCommand editCommand;
        
        public WindowGroupVM(ObservableCollection<Group> classGroup,
            ObservableCollection<Department> departments)
        {
            addCommand = this.Factory.CommandSync(Add);
            removeCommand = this.Factory.CommandSync(Remove);
            editCommand = this.Factory.CommandSync(Edit);

            ClassGroups = classGroup;
            this.departments = departments;

            index = this.Factory.Backing(nameof(Index), -1);
        }

        private void Add()
        {
            var context = new GroupVM(departments.ToArray());
            var wing = new NewGroup()
            {
                DataContext = context
            };
            wing.ShowDialog();
            System.Console.WriteLine(context.Group != null);
            if (context.Group != null)
                if (RequestToDataBase.Instance.requestInsertIntoGroup(context.Group))
                {
                    ClassGroups.Add(context.Group);
                }
        }

        private void Edit()
        {
            if (Index >= 0)
            {
                var group = ClassGroups[Index];
                var context = new GroupVM(group,departments.ToArray());
                var wind = new NewGroup()
                {
                    DataContext = context
                };
                wind.ShowDialog();
                if (context.Group != null)
                {
                    if (RequestToDataBase.Instance.requestUpdateGroup(context.Group, ClassGroups, Index))
                    {
                        ClassGroups[Index] = context.Group;
                    }
                }
            }
        }

        private void Remove()
        {
            if (Index >= 0)
                if (RequestToDataBase.Instance.requestDeleteFromGroup(ClassGroups, Index))
                {
                    ClassGroups.RemoveAt(Index);
                }
        }

        private ObservableCollection<Department> departments { get; }

        public ObservableCollection<Group> ClassGroups { get; }

        public ICommand AddCommand => addCommand;
        public ICommand RemoveCommand => removeCommand;
        public ICommand EditCommand => editCommand;

        public int Index { get { return index.Value; } set { index.Value = value; } }
    }
}
