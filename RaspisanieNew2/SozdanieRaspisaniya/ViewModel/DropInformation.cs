using GongSolutions.Wpf.DragDrop;
using Raspisanie.Models;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Windows;
using Gu.Wpf.DataGrid2D;
using ViewModule;
using System.Collections.ObjectModel;
using System.Linq;
using ModelLibrary;
using ViewModule.CSharp;

namespace SozdanieRaspisaniya.ViewModel
{
    public class DropInformation
    {
        //информация об уроке       
        public Subject Subject { get; set; }
        public Teacher Teacher { get; set; }
        public List<Group> Group { get; set; }
        public ClassRoom NumberOfClassroom { get; set; }
        public string Specifics { get; set; }
        public int Ndindex { get; set; }

        public DropInformation()
        {
            Group = new List<Group>();
        }

        public DropInformation(List<Group> group, Teacher teacher, Subject subject, string specific, ClassRoom numberOfClassroom, int ndindex)
        {
            Group = group;
            Teacher = teacher;
            Subject = subject;
            Specifics = specific;
            NumberOfClassroom = numberOfClassroom;
            Ndindex = ndindex;
        }

        public DropInformation Copy()
        {
            return new DropInformation
            {
                Subject = this.Subject,
                Teacher = this.Teacher,
                Group = new List<Group>(this.Group),
                Specifics = this.Specifics,
                NumberOfClassroom = this.NumberOfClassroom,
                Ndindex = this.Ndindex
            };
        }
    }

    public class PairInfo
    {
        public DayOfWeek Day { get; }
        public int Pair { get; }
        public PairInfo(int pair, DayOfWeek day)
        {
            Pair = pair;
            Day = day;
        }
    }

    public class DropItem : ViewModelBase, INotifyPropertyChanged, IDropTarget
    {
        //ячейка
        public object Key { get; set; }
        public Type KeyType { get; set; }
        public PairInfo Info { get; set; }
        private DropInformation item;
        private DropInformation itemTwo;
        private int n_dIndex;
        private int state = 0;

        private INotifyingValue<bool> isValid;

        public bool IsValueValid { get { return isValid.Value; } set { isValid.Value = value; } }

    //    public ObservableCollection<TeachersAndSubjects> AllTeachersAndSubjects { get; }
    //    public ObservableCollection<GroupsAndSubjects> AllGroupsAndSubjects { get; }

        public int State
        {
            get { return state; }
            set
            {
                if (value != state)
                {
                    state = value;
                    OnNotify(nameof(State));
                }
            }
        }

        public int N_DIndex
        {
            get { return n_dIndex; }
            set
            {
                if (value != n_dIndex)
                {
                    n_dIndex = value;
                    OnNotify(nameof(N_DIndex));
                }
            }
        }

        public DropInformation Item
        {
            get
            {
                return item;
            }
            set
            {
                if (value != item)
                {
                    item = value;
                    OnNotify(nameof(Item));
                }
            }
        }

        public DropInformation ItemTwo
        {
            get
            {
                return itemTwo;
            }
            set
            {
                if (value != itemTwo)
                {
                    itemTwo = value;
                    OnNotify(nameof(ItemTwo));
                }
            }
        }

        public DropItem(object key, Type typekey, PairInfo info)
        {
            Key = key;
            KeyType = typekey;
            Info = info;
            Item = new DropInformation();
            ItemTwo = new DropInformation();
            N_DIndex = state;

            isValid = this.Factory.Backing<bool>(nameof(IsValueValid), false);

            //AllTeachersAndSubjects = new ObservableCollection<TeachersAndSubjects>();
            //AllGroupsAndSubjects = new ObservableCollection<GroupsAndSubjects>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnNotify([CallerMemberName]string property = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        void IDropTarget.DragOver(IDropInfo dropInfo)
        {

            if (IsValueValid && dropInfo != null)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = DragDropEffects.Copy;
            }

            //AllTeachersAndSubjects.Clear();
            //AllGroupsAndSubjects.Clear();
            //foreach (var value in RequestToDataBase.Instance.ReadTeacherAndSubjects())
            //{
            //    AllTeachersAndSubjects.Add(value);
            //}

            //foreach (var value in RequestToDataBase.Instance.ReadGroupsAndSubjects())
            //{
            //    AllGroupsAndSubjects.Add(value);
            //}

            //var data = dropInfo.Data;//объявляем переменную данных перетаскиваемого элемента
            //var sourceItem = data is Subject || data is Teacher || data is Group || data is ClassRoom || data is string;
            ////объявляем переменную и смотрим на соответствие одного из 4 шаблонов
            //if (sourceItem && data.GetType() != KeyType)//если шаблон данных и тип перетаскиваемого элемента не равен типу ключа 
            //{
            //    if (dropInfo.Data is Teacher teacher)
            //    {
            //        foreach (var value in AllTeachersAndSubjects)
            //        {
            //            foreach (var sl in value.SubjectList)
            //            {
            //                foreach (var gr in AllGroupsAndSubjects)
            //                {
            //                    foreach (var subj in gr.InformationAboutSubjects)
            //                    {
            //                        if (teacher.CodeOfTeacher == value.Teacher.CodeOfTeacher
            //                            && value.Teacher.Department.CodeOfDepartment == teacher.Department.CodeOfDepartment
            //                                && sl.CodeOfSubject == subj.Subject.CodeOfSubject
            //                                    && value.DayList.ToList().Exists(t => t == Info.Day)
            //                                        && item.Group.Exists(g => g.CodeOfGroup == gr.Group.CodeOfGroup))
            //                        {
            //                            IsValid = true;
            //                        }
            //                    }
            //                }
            //            }

            //            if (value.Teacher.CodeOfTeacher == teacher.CodeOfTeacher
            //                && value.Teacher.Department.CodeOfDepartment == teacher.Department.CodeOfDepartment)
            //            {
            //                if (value.DayList.ToList().Exists(t => t == Info.Day))
            //                {
            //                    dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
            //                    dropInfo.Effects = DragDropEffects.Copy;
            //                }
            //            }
            //        }
            //    }
            //    else
            //    if (dropInfo.Data is Subject subject)
            //    {
            //        if (item.Teacher != null && (N_DIndex == 0 || N_DIndex == 1))
            //        {
            //            foreach (var groupvalue in AllGroupsAndSubjects)
            //            {
            //                foreach (var value in AllTeachersAndSubjects)
            //                    if (value.Teacher.CodeOfTeacher == item.Teacher.CodeOfTeacher
            //                        && value.Teacher.Department.CodeOfDepartment == item.Teacher.Department.CodeOfDepartment
            //                            && item.Group.Exists(g => g.CodeOfGroup == groupvalue.Group.CodeOfGroup))
            //                    {
            //                        if ((value.SubjectList.ToList().Exists(t => t.CodeOfSubject == subject.CodeOfSubject)) && (groupvalue.InformationAboutSubjects.ToList().Exists(g => g.Subject.CodeOfSubject == subject.CodeOfSubject)))
            //                        {
            //                            dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
            //                            dropInfo.Effects = DragDropEffects.Copy;
            //                        }
            //                    }
            //            }
            //        }
            //        else
            //        if (itemTwo.Teacher != null && N_DIndex == -1)
            //        {
            //            foreach (var groupvalue in AllGroupsAndSubjects)
            //            {
            //                foreach (var value in AllTeachersAndSubjects)
            //                    if (value.Teacher.CodeOfTeacher == itemTwo.Teacher.CodeOfTeacher
            //                        && value.Teacher.Department.CodeOfDepartment == itemTwo.Teacher.Department.CodeOfDepartment
            //                            && item.Group.Exists(g => g.CodeOfGroup == groupvalue.Group.CodeOfGroup))
            //                    {
            //                        if ((value.SubjectList.ToList().Exists(t => t.CodeOfSubject == subject.CodeOfSubject)) && (groupvalue.InformationAboutSubjects.ToList().Exists(g => g.Subject.CodeOfSubject == subject.CodeOfSubject)))
            //                        {
            //                            dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
            //                            dropInfo.Effects = DragDropEffects.Copy;
            //                        }
            //                    }
            //            }
            //        }
            //    }
            //    else
            //    {
            //        //устанавливаем цель на копирование элемента
            //        dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
            //        dropInfo.Effects = DragDropEffects.Copy;
            //    }

            //}
        }

        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            State = N_DIndex;
            // если перетаскиваемый элемент соответсвует шаблону предмет устанавливаем в категорию имени предмета в ячейке
            //название перетаскиваемого предмета
            if ((State == 0) || (State == 1))
            {
                if (dropInfo.Data is Subject)
                    Item.Subject = (dropInfo.Data as Subject);
                else if (dropInfo.Data is Group)
                    Item.Group.Add(dropInfo.Data as Group);
                else if (dropInfo.Data is Teacher)
                    Item.Teacher = (dropInfo.Data as Teacher);
                else if (dropInfo.Data is ClassRoom)
                    Item.NumberOfClassroom = (dropInfo.Data as ClassRoom);
                else if (dropInfo.Data is string)
                    Item.Specifics = (dropInfo.Data as string);
                //установка индекса
                Item.Ndindex = State;
                // копируем перетаскиваемые данные в ячейу над которой находится курсор.
                Item = item.Copy();
            }
            else
            {
                if (dropInfo.Data is Subject)
                    ItemTwo.Subject = (dropInfo.Data as Subject);
                else if (dropInfo.Data is Group)
                    ItemTwo.Group.Add((dropInfo.Data as Group));
                else if (dropInfo.Data is Teacher)
                    ItemTwo.Teacher = (dropInfo.Data as Teacher);
                else if (dropInfo.Data is ClassRoom)
                    ItemTwo.NumberOfClassroom = (dropInfo.Data as ClassRoom);
                else if (dropInfo.Data is string)
                    ItemTwo.Specifics = (dropInfo.Data as string);
                ItemTwo.Ndindex = State;
                // копируем перетаскиваемые данные в ячейу над которой находится курсор.
                ItemTwo = itemTwo.Copy();

            }
        }

    }

}
