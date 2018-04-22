using Raspisanie.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using ViewModule;
using ViewModule.CSharp;

namespace Raspisanie.ViewModels
{
    class WindowSubjectVM : ViewModelBase
    {
        private readonly INotifyingValue<int> index;

        private readonly INotifyCommand addCommand;
        private readonly INotifyCommand removeCommand;
        private readonly INotifyCommand editCommand;

        public WindowSubjectVM(ObservableCollection<Subject> classSubject, ObservableCollection<Department> departments)
        {
            addCommand = this.Factory.CommandSync(Add);
            removeCommand = this.Factory.CommandSync(Remove);
            editCommand = this.Factory.CommandSync(Edit);

            ClassSubject = classSubject;
            this.departments = departments;
            index = this.Factory.Backing(nameof(Index), -1);
        }

        private void Add()
        {
            var context = new SubjectVM(departments.ToArray());
            var wins = new NewSubject()
            {
                DataContext = context
            };
            wins.ShowDialog();
            System.Console.WriteLine(context.Subject != null);
            if (context.Subject != null)
                if (RequestToDataBase.Instance.requestInsertIntoSubject(context.Subject))
                {
                    ClassSubject.Add(context.Subject);
                }
        }

        private void Edit()
        {
            if (Index >= 0)
            {
                var subject = ClassSubject[Index];
                var context = new SubjectVM(subject, departments.ToArray());
                var wins = new NewSubject()
                {
                    DataContext = context
                };
                wins.ShowDialog();
                if (context.Subject != null)
                {
                    if (RequestToDataBase.Instance.requestUpdateSubject(context.Subject,ClassSubject, Index))
                    {
                        ClassSubject[Index] = context.Subject;
                    }
                }
            }
        }

        private void Remove()
        {
            if (Index >= 0)
                if (RequestToDataBase.Instance.requestDeleteFromSubject(ClassSubject, Index))
                {
                    ClassSubject.RemoveAt(Index);
                }
        }

        private ObservableCollection<Department> departments { get; }

        public ObservableCollection<Subject> ClassSubject { get; }

        public ICommand AddCommand => addCommand;
        public ICommand RemoveCommand => removeCommand;
        public ICommand EditCommand => editCommand;

        public int Index { get { return index.Value; } set { index.Value = value; } }
    }
}
