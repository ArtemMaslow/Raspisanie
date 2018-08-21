using FirebirdSql.Data.FirebirdClient;
using Raspisanie.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using ViewModule;
using ViewModule.CSharp;

namespace Raspisanie.ViewModels
{
    class WindowFacultyVM : ViewModelBase
    {
        private readonly INotifyingValue<int> index;

        private readonly INotifyCommand addCommand;
        private readonly INotifyCommand removeCommand;
        private readonly INotifyCommand editCommand;
        
        public WindowFacultyVM(ObservableCollection<Faculty> classFaculty)
        {
            addCommand = this.Factory.CommandSync(Add);
            removeCommand = this.Factory.CommandSync(Remove);
            editCommand = this.Factory.CommandSync(Edit);

            ClassFaculty = classFaculty;   

            index = this.Factory.Backing(nameof(Index), -1);
        }

        private void Add()
        {
            var context = new FacultyVM();
            var wind = new NewFaculty()
            {
                DataContext = context
            };
            wind.ShowDialog();
            if (context.Faculty != null)
            {               
                if (RequestToDataBase.Instance.requestInsertIntoFaculty(context.Faculty))
                {
                    ClassFaculty.Add(context.Faculty);                   
                }
            }
        }

        private void Edit()
        {
            if (Index >= 0)
            {
                var faculty = ClassFaculty[Index];
                var context = new FacultyVM(faculty);
                var wind = new NewFaculty()
                {
                    DataContext = context
                };
                wind.ShowDialog();
                if (context.Faculty != null)
                {                                     
                    if (RequestToDataBase.Instance.requestUpdateFaculty(context.Faculty, ClassFaculty,Index))
                    {
                        ClassFaculty[Index] = context.Faculty;
                    }
                }
            }
        }

        private void Remove()
        {
            
            if (Index >= 0)
            {
                if (RequestToDataBase.Instance.requestDeleteFromFaculty(ClassFaculty, Index))
                {
                    ClassFaculty.RemoveAt(Index);
                }
            }
        }

        public ObservableCollection<Faculty> ClassFaculty { get; }
        public ICommand AddCommand => addCommand;
        public ICommand RemoveCommand => removeCommand;
        public ICommand EditCommand => editCommand;

        public int Index { get { return index.Value; } set { index.Value = value; } }
    }
}
