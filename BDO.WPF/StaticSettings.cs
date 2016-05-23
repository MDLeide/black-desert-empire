using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using BDO.Domain;
using BDO.Persistence.Repo;

namespace BDO.WPF
{
    static class StaticSettings
    {
        //todo: persist these to Properties

        public static Character Character { get; set; }
        public static CharacterRepository CharacterRepository { get; set; }
        public static bool KeepOnTopDefault { get; set; } = true;
        public static bool AllowKeepOnTop { get; set; } = true;
        public static bool AutoSave { get; set; } = true;
        public static int DefaultEnergyPerTick { get; set; } = 1;

        public static string LogFile { get; set; } =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BDO", "log.txt");
    }
}
