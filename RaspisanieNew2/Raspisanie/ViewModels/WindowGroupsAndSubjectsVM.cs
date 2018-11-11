using ModelLibrary;
using Newtonsoft.Json;
using Raspisanie.Models;
using Raspisanie.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using ViewModule;
using ViewModule.CSharp;

namespace Raspisanie.ViewModels
{
    public class WindowGroupsAndSubjectsVM : ViewModelBase
    {
        private readonly INotifyingValue<int> groupIndex;

        private readonly INotifyCommand addCommand;
        private readonly INotifyCommand removeCommand;
        private readonly INotifyCommand editCommand;

        public WindowGroupsAndSubjectsVM(ObservableCollection<Group> classGroups, ObservableCollection<GroupsAndSubjects> groupsAndSubjects, ObservableCollection<Subject> classSubjects)
        {
            ClassGroups = classGroups;
            GroupsAndSubjects = groupsAndSubjects;
            ClassSubjects = classSubjects;

            addCommand = this.Factory.CommandSync(Add);
            removeCommand = this.Factory.CommandSync(Remove);
            editCommand = this.Factory.CommandSync(Edit);

            groupIndex = this.Factory.Backing(nameof(GroupIndex), -1);
        }

        public void Add()
        {
            var gAndS = GroupsAndSubjects[GroupIndex];
            var context = new GroupsAndSubjectsVM(subjects.ToArray());
            var wingands = new NewGroupsAndSubjects()
            {
                DataContext = context
            };
            wingands.ShowDialog();
            if (context.SubjectCons != null)
            {
                
            }
        }

        public void Edit()
        {
            var gAndS = GroupsAndSubjects[GroupIndex];
            var context = new GroupsAndSubjectsVM(subjects.ToArray());
            var wingands = new NewGroupsAndSubjects()
            {
                DataContext = context
            };
            wingands.ShowDialog();
            if (context.SubjectCons != null)
            {

            }
        }

        public void Remove()
        {
            if (GroupIndex >= 0)
            {

            }
        }

        private ObservableCollection<Subject> subjects { get; }

        public ICommand AddCommand => addCommand;
        public ICommand RemoveCommand => removeCommand;
        public ICommand EditCommand => editCommand;

        public int GroupIndex { get { return groupIndex.Value; } set { groupIndex.Value = value; } }
        public ObservableCollection<Group> ClassGroups { get; }
        public ObservableCollection<GroupsAndSubjects> GroupsAndSubjects { get; }
        public ObservableCollection<Subject> ClassSubjects { get; }
    }
}
