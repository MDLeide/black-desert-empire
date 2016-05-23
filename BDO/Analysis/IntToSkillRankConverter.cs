using System;

namespace BDO.Analysis
{
    public static class IntToSkillRankConverter
    {
        public static SkillRank Convert(int skillLevel, out int level)
        {
            if (skillLevel < 0)
                throw new InvalidOperationException();

            if (skillLevel <= 10)
            {
                level = skillLevel;
                return SkillRank.Beginner;
            }
            if (skillLevel <= 20)
            {
                level = skillLevel - 10;
                return SkillRank.Apprentice;
            }
            if (skillLevel <= 30)
            {
                level = skillLevel - 20;
                return SkillRank.Skilled;
            }
            if (skillLevel <= 40)
            {
                level = skillLevel - 30;
                return SkillRank.Professional;
            }
            if (skillLevel <= 50)
            {
                level = skillLevel - 40;
                return SkillRank.Artisan;
            }
            if (skillLevel <= 70)
            {
                level = skillLevel - 50;
                return SkillRank.Master;
            }

            level = skillLevel - 70;
            return SkillRank.Guru; 
        }
    }
}