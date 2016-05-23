using System.Collections.Generic;
using System.Linq;
using BDO.Domain.Enum;
using BDO.Domain.Interfaces;
using BDO.Domain.Observation;
using NHibernate.Util;

namespace BDO.Domain
{ 
    public class Recipe : BdoDomainObject, IValidatesSave, INamedEntity
    {
        public virtual string Name => Result?.Name ?? string.Empty;

        /// <summary>
        /// The primary item being created.
        /// </summary>
        public virtual Item Result { get; set; }
        public virtual IList<Item> SecondaryResults { get; set; } = new List<Item>();

        /// <summary>
        /// Other items that may occur as a result of creation.
        /// </summary>
        public virtual RecipeType Type { get; set; } = RecipeType.Processing;

        public virtual string SubType { get; set; }
        
        public virtual double ExpectedYield { get; set; } = 1;

        public virtual IDictionary<Item, int> Materials { get; set; } = new Dictionary<Item, int>();
        


        public virtual ValidationResult ValidateSave()
        {
            var results = new ValidationResult();

            if (Result == null)
                results.Violations.Add(new FieldViolation(nameof(Result), Result, "Must have a resulting item."));
            //if (Materials == null || !Materials.Any())
            //    results.Violations.Add(new FieldViolation(nameof(Materials), Materials, "Must have at least one material."));

            results.IsValid = !EnumerableExtensions.Any(results.Violations);
            return results;
        }

        protected internal virtual IList<ProcessingObservation> ProcessingObservations { get; set; } = new List<ProcessingObservation>();

        protected internal virtual IList<CraftObservation> CraftObservations { get; set; } = new List<CraftObservation>();

        public virtual IList<RecipeObservation> Observations
        {
            get { return ProcessingObservations.Cast<RecipeObservation>().Concat(CraftObservations.Cast<RecipeObservation>()).ToList(); }
        }

        public override string ToString()
        {
            return Type + ": " + Result.Name;
        }
    }
}