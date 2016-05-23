using System;
using System.Collections.Generic;
using System.Linq;
using BDO.Domain;
using BDO.Domain.Enum;
using BDO.Domain.Observation;

namespace BDO.Analysis
{
    public static class RecipeYieldCalculator
    {
        /// <summary>
        /// Uses expected yield regardless of other settings.
        /// </summary>
        public static bool UseExpectedYield { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating the calculator should calculate observations based
        /// on skill rank (beginner, apprentice, etc). Ignored if a skill level is not provided. If
        /// there are insufficient iterations available, will fall back to all observations.
        /// </summary>
        public static bool CalculateBySkillRank { get; set; }

        /// <summary>
        /// The minimum number of observed iterations required to perform a calculation, otherwise
        /// the <see cref="Recipe.ExpectedYield"/> is returned.
        /// </summary>
        public static int MinimumObservationIterationsForCalculation { get; set; } = 50;

        public static double GetYield(Recipe recipe)
        {
            return GetYield(recipe, -1);
        }

        public static double GetYield(Recipe recipe, int skillLevel)
        {
            if (UseExpectedYield)
                return recipe.ExpectedYield;
            
            IEnumerable<RecipeObservation> observations;

            if (skillLevel > 0 && CalculateBySkillRank)
            {
                int lvl;
                var rank = IntToSkillRankConverter.Convert(skillLevel, out lvl);
                observations =
                    recipe.Observations.Where(
                        p => p.SkillLevel > 0 && IntToSkillRankConverter.Convert(p.SkillLevel, out lvl) == rank)
                        .ToArray();
                if (observations.Sum(p => p.Iterations) < MinimumObservationIterationsForCalculation)
                    observations = recipe.Observations;
            }
            else
            {
                observations = recipe.Observations;
            }

            if (observations.Sum(p => p.Iterations) < MinimumObservationIterationsForCalculation)
                return recipe.ExpectedYield;

            return CalculateYield(observations);
        }

        public static double GetYield(Recipe recipe, Character character)
        {
            switch (recipe.Type)
            {
                case RecipeType.Craft:
                    switch (recipe.SubType)
                    {
                        case "Alchemy":
                            return GetYield(recipe, character.AlchemyLevel);
                        case "Cooking":
                            return GetYield(recipe, character.CookingLevel);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case RecipeType.Processing:
                    return GetYield(recipe, character.ProcessingLevel);
                case RecipeType.Workshop:
                    return GetYield(recipe);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        static double CalculateYield(IEnumerable<RecipeObservation> observations)
        {
            var iterations = observations.Sum(p => p.Iterations);
            var totalYield = observations.Sum(p => p.Yield);
            return (double)totalYield / iterations;
        }
    }
}