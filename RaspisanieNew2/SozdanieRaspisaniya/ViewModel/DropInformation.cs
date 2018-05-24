using GongSolutions.Wpf.DragDrop;
using Raspisanie.Models;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Windows;
using Gu.Wpf.DataGrid2D;
using ViewModule;

namespace SozdanieRaspisaniya.ViewModel
{
    public class DropInformation
    {
        public string Subject { get; set; }
        public string Teacher { get; set; }
        public string Group { get; set; }
        public string Specifics { get; set; }
        public string NumberOfClassroom { get; set; }

        public DropInformation Copy()
        {
            return new DropInformation
            {
                Subject = this.Subject,
                Teacher = this.Teacher,
                Group = this.Group,
                Specifics = this.Specifics,
                NumberOfClassroom = this.NumberOfClassroom
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

    public class DropItem : INotifyPropertyChanged, IDropTarget
    {
        public string Key { get; }
        public Type KeyType { get; }
        public PairInfo Info { get; }
        private DropInformation item;
        private DropInformation itemTwo;
        private int n_dIndex;

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

        public DropItem(string key, Type typekey, PairInfo info)
        {
            Key = key;
            KeyType = typekey;
            Info = info;
           Item = new DropInformation();
           ItemTwo = new DropInformation();
            N_DIndex = 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnNotify([CallerMemberName]string property = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            var data = dropInfo.Data;//объявляем переменную данных перетаскиваемого элемента
            var sourceItem = data is Subject || data is Teacher || data is Group || data is ClassRoom;
            //объявляем переменную и смотрим на соответствие одного из 4 шаблонов

            if (sourceItem && data.GetType() != KeyType)//если шаблон данных и тип перетаскиваемого элемента не равен типу ключа 
            {   //устанавливаем цель на копирование элемента
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = DragDropEffects.Copy;
            }
        }

        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            // если перетаскиваемый элемент соответсвует шаблону предмет устанавливаем в категорию имени предмета в ячейке
            //название перетаскиваемого предмета
            if ((N_DIndex == 0) || (N_DIndex == 1))
            {
                if (dropInfo.Data is Subject)
                    Item.Subject = (dropInfo.Data as Subject).NameOfSubject;
                else if (dropInfo.Data is Group)
                    Item.Group = (dropInfo.Data as Group).NameOfGroup;
                else if (dropInfo.Data is Teacher)
                    Item.Teacher = (dropInfo.Data as Teacher).FIO;
                else if (dropInfo.Data is ClassRoom)
                    Item.NumberOfClassroom = (dropInfo.Data as ClassRoom).NumberOfClassroom;
                // копируем перетаскиваемые данные в ячейу над которой находится курсор.
                Item = item.Copy();
            }
            else
            {
                if (dropInfo.Data is Subject)
                    ItemTwo.Subject = (dropInfo.Data as Subject).NameOfSubject;
                else if (dropInfo.Data is Group)
                    ItemTwo.Group = (dropInfo.Data as Group).NameOfGroup;
                else if (dropInfo.Data is Teacher)
                    ItemTwo.Teacher = (dropInfo.Data as Teacher).FIO;
                else if (dropInfo.Data is ClassRoom)
                    ItemTwo.NumberOfClassroom = (dropInfo.Data as ClassRoom).NumberOfClassroom;
                // копируем перетаскиваемые данные в ячейу над которой находится курсор.
                ItemTwo = itemTwo.Copy();

            }
        }

    }

}
