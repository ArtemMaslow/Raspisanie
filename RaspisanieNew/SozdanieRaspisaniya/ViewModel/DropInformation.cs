using GongSolutions.Wpf.DragDrop;
using Raspisanie.Models;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

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
        public DropItem(string key, Type typekey, PairInfo info)
        {
            Key = key;
            KeyType = typekey;
            Info = info;
            Item = new DropInformation();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnNotify([CallerMemberName]string property = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            var data = dropInfo.Data;
            var sourceItem = data is Subject || data is Teacher || data is Group || data is ClassRoom;


            if (sourceItem && data.GetType() != KeyType)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = DragDropEffects.Copy;
            }
        }

        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            if (dropInfo.Data is Subject )
                Item.Subject = (dropInfo.Data as Subject).NameOfSubject;
            else if (dropInfo.Data is Group )
                Item.Group = (dropInfo.Data as Group).NameOfGroup;
            else if (dropInfo.Data is Teacher )
                Item.Teacher = (dropInfo.Data as Teacher).FIO;
            else if (dropInfo.Data is ClassRoom )
                Item.NumberOfClassroom = (dropInfo.Data as ClassRoom).NumberOfClassroom;
            Item = item.Copy();
        }
    }

}
