﻿using Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ViewModule;
using ViewModule.CSharp;

namespace Raspisanie.ViewModels
{
    public class WindowGroupsAndSubjectsVM : ViewModelBase
    {
        private readonly INotifyingValue<int> groupIndex;
        private readonly INotifyingValue<int> subjectIndex;

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
            subjectIndex = this.Factory.Backing(nameof(SubjectIndex), -1);
        }

        public void Add()
        {
            if (GroupIndex >= 0)
            {
                var gAndS = GroupsAndSubjects[GroupIndex];
                var context = new GroupsAndSubjectsVM(ClassSubjects.ToArray());
                var wind = new NewGroupsAndSubjects()
                {
                    DataContext = context
                };
                wind.ShowDialog();
                if (wind.DialogResult == true)
                {
                    bool exist = false;
                    foreach (var value in gAndS.InformationAboutSubjects)
                    {
                        if (value.Subject.CodeOfSubject == context.InformationAboutSubjects.Subject.CodeOfSubject)
                        {
                            exist = true;
                            MessageBox.Show("Такой предмет уже есть! Добавьте тот предмет которого ещё нету в списке","Ошибка добавления предмета");
                            break;
                        }
                    }
                    if ((context.InformationAboutSubjects != null) && (exist == false))
                    {
                        var items = gAndS.InformationAboutSubjects.Append(context.InformationAboutSubjects).ToArray();
                        foreach (var item in items)
                        {
                            if (RequestToDataBase.Instance.requestInsertIntoGroupsAndSubjects(gAndS, item))
                            {
                                RefreshGroupsAndSubjects();
                            }
                        }
                    }
                }
            }
        }

        public void Edit()
        {
            if (GroupIndex >= 0 && SubjectIndex >= 0)
            {
                var gAndS = GroupsAndSubjects[GroupIndex];
                var context = new GroupsAndSubjectsVM(gAndS.InformationAboutSubjects[SubjectIndex], ClassSubjects.ToArray());
                var wind = new NewGroupsAndSubjects()
                {
                    DataContext = context
                };
                wind.GAS.IsEnabled = false;
                wind.ShowDialog();
                if (wind.DialogResult == true)
                {
                    if ((context.InformationAboutSubjects != null))
                    {
                        gAndS.InformationAboutSubjects[SubjectIndex] = context.InformationAboutSubjects;
                        if (RequestToDataBase.Instance.requestUpdateGroupsAndSubjects(gAndS, gAndS.InformationAboutSubjects[SubjectIndex]))
                        {
                            RefreshGroupsAndSubjects();
                        }
                    }
                }
            }
        }

        public void Remove()
        {
            if (GroupIndex >= 0 && SubjectIndex >=0)
            {
                var gAndS = GroupsAndSubjects[GroupIndex];
                if (RequestToDataBase.Instance.requestDeleteElementFromGroupsAndSubjects(gAndS, gAndS.InformationAboutSubjects[SubjectIndex]))
                {
                    RefreshGroupsAndSubjects();
                }
            }
            else if (GroupIndex >= 0)
            {
                var gAndS = GroupsAndSubjects[GroupIndex];
                if (RequestToDataBase.Instance.requestDeleteAllElementsFromGroupsAndSubjects(gAndS))
                {
                    RefreshGroupsAndSubjects();
                }
            }
        }

        private void RefreshGroupsAndSubjects()
        {
            GroupsAndSubjects.Clear();
            var dct = new Dictionary<int, GroupsAndSubjects>();
            foreach (var value in RequestToDataBase.Instance.ReadGroupsAndSubjects()) dct.Add(value.Group.CodeOfGroup, value);
            var all = ClassGroups.Select(g => dct.TryGetValue(g.CodeOfGroup, out GroupsAndSubjects gs) ? gs : CreateEmpty(g));
            foreach (var value in all)
                GroupsAndSubjects.Add(value);
        }

        private GroupsAndSubjects CreateEmpty(Group group)
        {
            return new GroupsAndSubjects
            {
                Group = group,
                InformationAboutSubjects = Enumerable.Empty<SubjectInform>().ToArray()
            };
        }

        public ICommand AddCommand => addCommand;
        public ICommand RemoveCommand => removeCommand;
        public ICommand EditCommand => editCommand;

        public int GroupIndex { get { return groupIndex.Value; } set { groupIndex.Value = value; } }
        public int SubjectIndex { get { return subjectIndex.Value; } set { subjectIndex.Value = value; } }

        public ObservableCollection<Group> ClassGroups { get; }
        public ObservableCollection<GroupsAndSubjects> GroupsAndSubjects { get; }
        public ObservableCollection<Subject> ClassSubjects { get; }

    }
}
