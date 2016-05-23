using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BDO.Domain.Enum
{
    public enum MarketCategory
    {
        [Description("General Goods")]
        General = 0,
        [Description("Ore & Gemstones")]
        Ore,
        Plants,
        [Description("Seeds & Fruit")]
        Seeds,
        Hide,
        Seafood,
        [Description("Main Weapon")]
        MainWeapon,
        [Description("Secondary Weapon")]
        SecondaryWeapon,
        [Description("Defense Gear")]
        DefenseGear,
        Accessory,
        [Description("Black Stone")]
        BlackStone,
        Crystal,
        [Description("Potion & Elixir")]
        Potions,
        Cooking,
        Dye,
        Housing,
        [Description("Mount & Pet")]
        Mount,
        [Description("Special Deals")]
        Special
    }
}
