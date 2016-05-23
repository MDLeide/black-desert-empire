using BDO.Analysis;
using NTC.WPF.MVVM;

namespace BDO.WPF.VM.Domain
{
    public class SkillViewModel : ViewModelBase
    {
        public SkillViewModel(string skillName, ref int skillLevel, ref int skillProgress)
        {
            SkillName = skillName;
            SkillLevel = skillLevel;
            SkillProgress = skillProgress;
        }

        string _skillName;

        public string SkillName
        {
            get { return _skillName; }
            set
            {
                if (Equals(value, _skillName)) return;
                _skillName = value;
                OnPropertyChanged(nameof(SkillName));
            }
        }

        SkillRank _skillRank;

        public SkillRank SkillRank
        {
            get { return _skillRank; }
            set
            {
                if (Equals(value, _skillRank)) return;
                _skillRank = value;
                OnPropertyChanged(nameof(SkillRank));
            }
        }

        int _rankLevel;

        public int RankLevel
        {
            get { return _rankLevel; }
            set
            {
                if (Equals(value, _rankLevel)) return;
                _rankLevel = value;
                OnPropertyChanged(nameof(RankLevel));
            }
        }

        int _skillLevel;

        public int SkillLevel
        {
            get { return _skillLevel; }
            set
            {
                if (Equals(value, _skillLevel)) return;
                _skillLevel = value;
                OnPropertyChanged(nameof(SkillLevel));
            }
        }

        int _skillProgress;

        public int SkillProgress
        {
            get { return _skillProgress; }
            set
            {
                if (Equals(value, _skillProgress)) return;
                _skillProgress = value;
                OnPropertyChanged(nameof(SkillProgress));
            }
        }

    }
}