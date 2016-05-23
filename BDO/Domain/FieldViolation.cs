namespace BDO.Domain
{
    public class FieldViolation
    {
        public FieldViolation()
        {
        }

        public FieldViolation(string propertyName, object currentPropertyValue, string ruleViolated)
        {
            PropertyName = propertyName;
            CurrentPropertyValue = currentPropertyValue;
            RuleViolated = ruleViolated;
        }

        public string PropertyName { get; set; }
        public object CurrentPropertyValue { get; set; }
        public string RuleViolated { get; set; }
    }
}