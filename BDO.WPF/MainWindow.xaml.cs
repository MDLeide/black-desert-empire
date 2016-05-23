using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BDO.Domain;
using BDO.Persistence.Config;
using BDO.Persistence.Repo;
using BDO.WPF.V;
using BDO.WPF.VM;

namespace BDO.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            SetupCharacter();

            var vm = new MainWindowViewModel();
            Closing += vm.WindowClosing;
            this.DataContext = vm;   
        }


        //todo: prompt for character stats
        void SetupCharacter()
        {
            StaticSettings.CharacterRepository = new CharacterRepository();

            Character character = null;

            if (Properties.Settings.Default.DefaultCharacter != Guid.Empty)
                character = StaticSettings.CharacterRepository.Get(Properties.Settings.Default.DefaultCharacter);

            if (character == null)
            {
                character = new Character();
                StaticSettings.CharacterRepository.Save(character);
                Properties.Settings.Default.DefaultCharacter = character.Id;
                Properties.Settings.Default.Save();
            }
            
            StaticSettings.Character = character;
        }
    }
}
