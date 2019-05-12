using GongSolutions.Wpf.DragDrop;
using Models;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using ViewModule;
using ViewModule.CSharp;

namespace SozdanieRaspisaniya.ViewModel
{
    public class DropItem : ViewModelBase, IDropTarget
    {
        //ячейка
        public object Key { get; set; }
        public Type KeyType { get; set; }
        public PairInfo Info { get; set; }


        private INotifyingValue<int> n_dIndex;
        private INotifyingValue<int> state;
        private INotifyingValue<DropInformation> item;
        private INotifyingValue<DropInformation> itemTwo;
        private INotifyingValue<bool> isValid;

        public DropItem(object key, Type typekey, PairInfo info)
        {
            Key = key;
            KeyType = typekey;
            Info = info;

            state = this.Factory.Backing(nameof(State), 0);
            n_dIndex = this.Factory.Backing(nameof(N_DIndex), 0);
            item = this.Factory.Backing(nameof(Item), new DropInformation());
            itemTwo = this.Factory.Backing(nameof(ItemTwo), new DropInformation());
            isValid = this.Factory.Backing(nameof(IsValueValid), false);

        }

        public bool IsValueValid { get { return isValid.Value; } set { isValid.Value = value; } }

        public int State
        {
            get
            {
                return state.Value;
            }
            set
            {
                state.Value = value;
            }
        }

        public int N_DIndex
        {
            get
            {
                return n_dIndex.Value;
            }
            set
            {
                n_dIndex.Value = value;
            }
        }

        public DropInformation Item
        {
            get
            {
                return item.Value;
            }
            set
            {
                item.Value = value;
            }
        }

        public DropInformation ItemTwo
        {
            get
            {
                return itemTwo.Value;
            }
            set
            {
                itemTwo.Value = value;
            }
        }

        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            if (IsValueValid && dropInfo != null)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = DragDropEffects.Copy;
            }
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
                Item = Item.Copy();
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
                ItemTwo = ItemTwo.Copy();
            }
        }

    }

}
