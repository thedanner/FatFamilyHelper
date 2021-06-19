using System.IO;

namespace Left4DeadHelper.Sprays
{
    public class ConversionResult
    {
        public ConversionResult(Stream stream, string fileExtension)
        {
            if (string.IsNullOrEmpty(fileExtension))
            {
                throw new System.ArgumentException($"'{nameof(fileExtension)}' cannot be null or empty.", nameof(fileExtension));
            }

            Stream = stream ?? throw new System.ArgumentNullException(nameof(stream));
            FileExtension = fileExtension;
        }

        public Stream Stream { get; }
        public string FileExtension { get; }
    }
}
