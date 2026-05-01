namespace SadaqaAccounting.Shared.ReportHelpers
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DisplayHeaderAttribute : Attribute
    {
        public string HeaderText { get; }

        public DisplayHeaderAttribute(string headerText)
        {
            HeaderText = headerText;
        }
    }
}
