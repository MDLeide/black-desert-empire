using System.Collections.Generic;
using System.Linq;
using BDO.Domain;
using BDO.Persistence.Repo;
using BDO.WPF.V.Win;
using BDO.WPF.VM.Finders;

namespace BDO.WPF.VM
{
    static class GetItemHelper
    {
        public static Item GetItem()
        {
            Item item = null;
            var vm = new ItemFinderViewModel();
            vm.ItemSelected += (s, e) => item = vm.SelectedObject.DomainObject;
            var w = new ItemFinderWindow();
            w.DataContext = vm;
            vm.Canceled += (s, e) => w.Close();
            w.ShowDialog();
            return item;
        }
    }
}