using NTC.WPF.MVVM;

namespace BDO.WPF.VM.Domain
{
    class ItemQuantityPair : ViewModelBase
    {
        ItemViewModel _item;
        int _quantity;

        public ItemViewModel Item
        {
            get { return _item; }
            set
            {
                if (Equals(value, _item)) return;
                _item = value;
                OnPropertyChanged(nameof(Item));
            }
        }

        public bool Craft
        {
            get { return Item.DomainObject.Craft; }
            set
            {
                if (Equals(value, Item.DomainObject.Craft)) return;
                Item.DomainObject.Craft = value;

                OnPropertyChanged(nameof(Craft));
                Item.Save.Execute(this);
                OnPropertyChanged(nameof(ExtendedCost));
                OnPropertyChanged(nameof(Item));
            }
        }

        public int Quantity
        {
            get { return _quantity; }
            set
            {
                if (Equals(value, _quantity)) return;
                _quantity = value;
                OnPropertyChanged(nameof(Quantity));
            }
        }

        public int ExtendedCost
        {
            get { return _item.UnitCost*Quantity; }
        }
    }
}