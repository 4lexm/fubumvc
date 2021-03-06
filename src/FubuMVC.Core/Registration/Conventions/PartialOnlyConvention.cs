namespace FubuMVC.Core.Registration.Conventions
{
    [Policy]
    public class PartialOnlyConvention : Policy
    {
        public const string Partial = "Partial";

        public PartialOnlyConvention()
        {
            Where.AnyActionMatches(call => call.Method.Name.EndsWith("Partial"));
            ModifyBy(chain => chain.IsPartialOnly = true, configurationType: ConfigurationType.Policy);
        }
    }
}