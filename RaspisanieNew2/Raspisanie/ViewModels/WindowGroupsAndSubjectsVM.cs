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
            var context = new GroupsAndSubjectsVM(gAndS, ClassSubjects.ToArray());
            var wingands = new NewGroupsAndSubjects()
            {
                DataContext = context
            };
            wingands.ShowDialog();
            if (context.GroupsAndSubjects != null)
            {
                var si = JsonConvert.SerializeObject(context.SubjectInform);
                if (RequestToDataBase.Instance.requestInsertIntoGroupsAndSubjects(gAndS, si))
                {
                    RefreshGroupsAndSubjects();
                }
            }
        }

        public void Edit()
        {
            var gAndS = GroupsAndSubjects[GroupIndex];
            var context = new GroupsAndSubjectsVM(gAndS, ClassSubjects.ToArray());
            var wingands = new NewGroupsAndSubjects()
            {
                DataContext = context
            };
            wingands.ShowDialog();
            if (context.GroupsAndSubjects != null)
            {

            }
        }

        public void Remove()
        {
            if (GroupIndex >= 0)
            {

            }
        }

        private void RefreshGroupsAndSubjects()
        {
            GroupsAndSubjects.Clear();
            var dct = new Dictionary<int, GroupsAndSubjects>();
            foreach (var value in RequestToDataBase.Instance.ReadGroupsAndSubjects()) dct.Add(value.Group.CodeOfGroup, value);
            var all = ClassGroups.Select(g => dct.TryGetValue(g.CodeOfGroup, out GroupsAndSubjects gs) ? gs : CreateEmpty(g));
            foreach (var value in all)
            {
                Console.WriteLine("key teacher: " + value.Group.CodeOfGroup);
            }
            foreach (var value in all)
                GroupsAndSubjects.Add(value);
        }

        private GroupsAndSubjects CreateEmpty(Group group)
        {
            return new GroupsAndSubjects
            {
                Group = group,
                Semester = 0,
                SubjectInform = Enumerable.Empty<SubjectInform>().ToArray()
            };
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
