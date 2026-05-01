namespace SadaqaAccounting.Shared.ReportHelpers
{
    public class ImageService : IImageService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ImageService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public byte[] GetImageInBytes(string url)
        {
            string imagePath;

            if (!string.IsNullOrWhiteSpace(url))
            {
                imagePath = Path.Combine(
                    _webHostEnvironment.WebRootPath,
                    url.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString())
                );
            }
            else
            {
                imagePath = Path.Combine(
                    _webHostEnvironment.WebRootPath,
                    "images", "dummy.jpg"
                );
            }

            return File.Exists(imagePath)
                ? File.ReadAllBytes(imagePath)
                : Array.Empty<byte>();
        }


        public byte[] GenerateQrCode(string qrCodeUrl)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(qrCodeUrl, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);
            return qrCode.GetGraphic(20);
        }

        public byte[] GetCompanyLogo(string logoPath)
        {
            if (string.IsNullOrWhiteSpace(logoPath))
                return Array.Empty<byte>();

            string imagePath = Path.Combine(
                _webHostEnvironment.WebRootPath,
                logoPath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString())
            );

            return File.Exists(imagePath) ? File.ReadAllBytes(imagePath) : Array.Empty<byte>();
        }
    }
}
