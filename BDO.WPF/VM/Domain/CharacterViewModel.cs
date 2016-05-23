using System.Windows.Navigation;
using BDO.Analysis;
using BDO.Domain;
using BDO.Persistence.Repo;
using BDO.WPF.VM.Base;

namespace BDO.WPF.VM.Domain
{
    //public virtual int AlchemyLevel { get; set; }
    //public virtual int CookingLevel { get; set; }
    //public virtual int ProcessingLevel { get; set; }
    //public virtual int GatheringLevel { get; set; }
    //public virtual int FishingLevel { get; set; }

    //public virtual int AlchemyProgress { get; set; }
    //public virtual int CookingProgress { get; set; }
    //public virtual int ProcessingProgress { get; set; }
    //public virtual int GatheringProgress { get; set; }
    //public virtual int FishingProgress { get; set; }

    internal class CharacterViewModel : DomainObjectViewModel<Character>
    {
        SkillRank _alchemyRank;
        int _alchemyRankLevel;
        SkillRank _cookingRank;
        int _cookingRankLevel;
        SkillRank _fishingRank;
        int _fishingRankLevel;
        SkillRank _gatheringRank;
        int _gatheringRankLevel;
        SkillRank _processingRank;
        int _processingRankLevel;


        public CharacterViewModel(Character character, CharacterRepository repository) : base(character, repository)
        {
            AlchemyRank = IntToSkillRankConverter.Convert(character.AlchemyLevel, out _alchemyRankLevel);
            CookingRank = IntToSkillRankConverter.Convert(character.CookingLevel, out _cookingRankLevel);
            ProcessingRank = IntToSkillRankConverter.Convert(character.ProcessingLevel, out _processingRankLevel);
            GatheringRank = IntToSkillRankConverter.Convert(character.GatheringLevel, out _gatheringRankLevel);
            FishingRank = IntToSkillRankConverter.Convert(character.FishingLevel, out _fishingRankLevel);
        }

        public int Level
        {
            get { return DomainObject.Level; }
            set
            {
                if (Equals(value, DomainObject.Level)) return;
                DomainObject.Level = value;
                OnPropertyChanged(nameof(Level));
            }
        }

        public string Name
        {
            get { return DomainObject.Name; }
            set
            {
                if (Equals(value, DomainObject.Name)) return;
                DomainObject.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }


        public SkillRank AlchemyRank
        {
            get { return _alchemyRank; }
            set
            {
                if (Equals(value, _alchemyRank)) return;
                _alchemyRank = value;
                OnPropertyChanged(nameof(AlchemyRank));
            }
        }

        public int AlchemyRankLevel
        {
            get { return _alchemyRankLevel; }
            set
            {
                if (Equals(value, _alchemyRankLevel)) return;
                _alchemyRankLevel = value;
                OnPropertyChanged(nameof(AlchemyRankLevel));
            }
        }

        public int AlchemyLevel
        {
            get { return DomainObject.AlchemyLevel; }
            set
            {
                if (Equals(value, DomainObject.AlchemyLevel)) return;
                DomainObject.AlchemyLevel = value;
                AlchemyRank = IntToSkillRankConverter.Convert(value, out _alchemyRankLevel);
                OnPropertyChanged(nameof(AlchemyLevel));
                OnPropertyChanged(nameof(AlchemyRankLevel));
            }
        }

        public int AlchemyProgress
        {
            get { return DomainObject.AlchemyProgress; }
            set
            {
                if (Equals(value, DomainObject.AlchemyProgress)) return;
                DomainObject.AlchemyProgress = value;
                OnPropertyChanged(nameof(AlchemyProgress));
            }
        }


        public SkillRank CookingRank
        {
            get { return _cookingRank; }
            set
            {
                if (Equals(value, _cookingRank)) return;
                _cookingRank = value;
                OnPropertyChanged(nameof(CookingRank));
            }
        }

        public int CookingRankLevel
        {
            get { return _cookingRankLevel; }
            set
            {
                if (Equals(value, _cookingRankLevel)) return;
                _cookingRankLevel = value;
                OnPropertyChanged(nameof(CookingRankLevel));
            }
        }

        public int CookingLevel
        {
            get { return DomainObject.CookingLevel; }
            set
            {
                if (Equals(value, DomainObject.CookingLevel)) return;
                DomainObject.CookingLevel = value;
                CookingRank = IntToSkillRankConverter.Convert(value, out _cookingRankLevel);
                OnPropertyChanged(nameof(CookingRankLevel));
                OnPropertyChanged(nameof(CookingLevel));
            }
        }

        public int CookingProgress
        {
            get { return DomainObject.CookingProgress; }
            set
            {
                if (Equals(value, DomainObject.CookingProgress)) return;
                DomainObject.CookingProgress = value;
                OnPropertyChanged(nameof(CookingProgress));
            }
        }


        public SkillRank ProcessingRank
        {
            get { return _processingRank; }
            set
            {
                if (Equals(value, _processingRank)) return;
                _processingRank = value;
                OnPropertyChanged(nameof(ProcessingRank));
            }
        }

        public int ProcessingRankLevel
        {
            get { return _processingRankLevel; }
            set
            {
                if (Equals(value, _processingRankLevel)) return;
                _processingRankLevel = value;
                OnPropertyChanged(nameof(ProcessingRankLevel));
            }
        }

        public int ProcessingLevel
        {
            get { return DomainObject.ProcessingLevel; }
            set
            {
                if (Equals(value, DomainObject.ProcessingLevel)) return;
                DomainObject.ProcessingLevel = value;
                ProcessingRank = IntToSkillRankConverter.Convert(value, out _processingRankLevel);
                OnPropertyChanged(nameof(ProcessingLevel));
                OnPropertyChanged(nameof(ProcessingRankLevel));
            }
        }

        public int ProcessingProgress
        {
            get { return DomainObject.ProcessingProgress; }
            set
            {
                if (Equals(value, DomainObject.ProcessingProgress)) return;
                DomainObject.ProcessingProgress = value;
                OnPropertyChanged(nameof(ProcessingProgress));
            }
        }


        public SkillRank GatheringRank
        {
            get { return _gatheringRank; }
            set
            {
                if (Equals(value, _gatheringRank)) return;
                _gatheringRank = value;
                OnPropertyChanged(nameof(GatheringRank));
            }
        }

        public int GatheringRankLevel
        {
            get { return _gatheringRankLevel; }
            set
            {
                if (Equals(value, _gatheringRankLevel)) return;
                _gatheringRankLevel = value;
                OnPropertyChanged(nameof(GatheringRankLevel));
            }
        }

        public int GatheringLevel
        {
            get { return DomainObject.GatheringLevel; }
            set
            {
                if (Equals(value, DomainObject.GatheringLevel)) return;
                DomainObject.GatheringLevel = value;
                GatheringRank = IntToSkillRankConverter.Convert(value, out _gatheringRankLevel);
                OnPropertyChanged(nameof(GatheringLevel));
                OnPropertyChanged(nameof(GatheringRankLevel));
            }
        }

        public int GatheringProgress
        {
            get { return DomainObject.GatheringProgress; }
            set
            {
                if (Equals(value, DomainObject.GatheringProgress)) return;
                DomainObject.GatheringProgress = value;
                OnPropertyChanged(nameof(GatheringProgress));
            }
        }


        public SkillRank FishingRank
        {
            get { return _fishingRank; }
            set
            {
                if (Equals(value, _fishingRank)) return;
                _fishingRank = value;
                OnPropertyChanged(nameof(FishingRank));
            }
        }

        public int FishingRankLevel
        {
            get { return _fishingRankLevel; }
            set
            {
                if (Equals(value, _fishingRankLevel)) return;
                _fishingRankLevel = value;
                OnPropertyChanged(nameof(FishingRankLevel));
            }
        }

        public int FishingLevel
        {
            get { return DomainObject.FishingLevel; }
            set
            {
                if (Equals(value, DomainObject.FishingLevel)) return;
                DomainObject.FishingLevel = value;
                FishingRank = IntToSkillRankConverter.Convert(value, out _fishingRankLevel);
                OnPropertyChanged(nameof(FishingLevel));
                OnPropertyChanged(nameof(FishingRankLevel));
            }
        }

        public int FishingProgress
        {
            get { return DomainObject.FishingProgress; }
            set
            {
                if (Equals(value, DomainObject.FishingProgress)) return;
                DomainObject.FishingProgress = value;
                OnPropertyChanged(nameof(FishingProgress));
            }
        }
    }
}