using System.Drawing;
using System.Drawing.Imaging;

namespace SadaqaAccounting.Application.Configurations.UploadImages
{
    public static class UploadImageProvider
    {
        public static string UploadImageFile(this IWebHostEnvironment _webHostEnvironment, IFormFile file, string folderName = "", string subFolderName = "", bool isFileName = false)
        {
            var uploadPath = _webHostEnvironment.WebRootPath;

            if (folderName is not null) uploadPath = Path.Combine(uploadPath, folderName);
            if (subFolderName is not null) uploadPath = Path.Combine(uploadPath, subFolderName);
            if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

            string filePath;

            if (isFileName)
                filePath = Guid.NewGuid().ToString("N") + "_" + file.FileName;
            else
                filePath = Guid.NewGuid().ToString("N") + Path.GetExtension(file.FileName);

            filePath = Path.Combine(uploadPath, filePath);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

            var contentRootPath = _webHostEnvironment.ContentRootPath;

            var relativePath = filePath.Replace(contentRootPath, "");
            return relativePath.Replace(@"\wwwroot", "").Replace(@"\", "/");
        }

        public static string UploadImageFileThumbnail(this IWebHostEnvironment _webHostEnvironment, IFormFile file, string folderName = "", string subFolderName = "", bool isFileName = false)
        {
            var uploadPath = _webHostEnvironment.WebRootPath;

            if (folderName is not null) uploadPath = Path.Combine(uploadPath, folderName);
            if (subFolderName is not null) uploadPath = Path.Combine(uploadPath, subFolderName);
            if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

            string filePath;

            if (isFileName)
                filePath = Guid.NewGuid().ToString("N") + "_" + file.FileName;
            else
                filePath = Guid.NewGuid().ToString("N") + Path.GetExtension(file.FileName);

            filePath = Path.Combine(uploadPath, filePath);

            Image? sourceimage = Image.FromStream(file.OpenReadStream());
            Bitmap? b = new Bitmap(sourceimage);

            ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
            System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
            var encoderParameters = new EncoderParameters(1);
            var parameter = new EncoderParameter(myEncoder, 20L);
            encoderParameters.Param[0] = parameter;

            b.Save(filePath, jpgEncoder, encoderParameters);
            b.Dispose();
            sourceimage.Dispose();

            var contentRootPath = _webHostEnvironment.ContentRootPath;

            var relativePath = filePath.Replace(contentRootPath, "");
            return relativePath.Replace(@"\wwwroot", "").Replace(@"\", "/");
        }

        //JPEG Encoder Method
        public static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }


        public static IFormFile ConvertBase64ToIFormFile(string base64String, string fileName)
        {
            if (string.IsNullOrWhiteSpace(base64String))
                throw new ArgumentException("Base64 string is null or empty.");

            var base64Parts = Regex.Match(base64String, @"data:(?<type>.+?);base64,(?<data>.+)").Groups;
            string actualData = base64Parts.Count > 0 && base64Parts["data"].Success
                ? base64Parts["data"].Value
                : base64String;

            byte[] fileBytes = Convert.FromBase64String(actualData);

            var stream = new MemoryStream(fileBytes);
            var formFile = new FormFile(stream, 0, stream.Length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = GetContentType(fileName)
            };

            return formFile;
        }

        private static string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".pdf" => "application/pdf",
                ".txt" => "text/plain",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                _ => "application/octet-stream",
            };
        }
    }
}
