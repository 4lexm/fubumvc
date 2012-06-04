namespace FubuMVC.Diagnostics.Runtime
{
    public class ExceptionReport : IBehaviorDetails
    {
        public string Text { get; set; }

        public void AcceptVisitor(IBehaviorDetailsVisitor visitor)
        {
            visitor.Exception(this);
        }
    }
}