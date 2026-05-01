namespace SadaqaAccounting.Shared.ReportHelpers
{
    public interface IImageService
    {
        public byte[] GetImageInBytes(string url);

        public byte[] GenerateQrCode(string qrCodeUrl);

        public byte[] GetCompanyLogo(string logoPath);
    }
}
