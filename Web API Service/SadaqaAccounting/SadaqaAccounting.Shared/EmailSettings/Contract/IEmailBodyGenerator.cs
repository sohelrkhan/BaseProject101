namespace SadaqaAccounting.Shared.EmailSettings.Contract
{
    public interface IEmailBodyGenerator
    {
        string GenerateBody(ApplicationInformation info);
    }
}