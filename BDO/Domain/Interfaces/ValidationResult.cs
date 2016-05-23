using System.Collections.Generic;

namespace BDO.Domain.Interfaces
{
    public class ValidationResult
    {
        public bool IsValid { get; internal set; }
        public List<FieldViolation> Violations { get; internal set; } = new List<FieldViolation>();
    }
}