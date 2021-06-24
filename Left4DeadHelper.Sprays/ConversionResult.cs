using System.IO;

namespace Left4DeadHelper.Sprays
{
    public class ConversionResult
    {
        public ConversionResult(string fileExtension)
        {
            if (string.IsNullOrEmpty(fileExtension))
            {
                throw new System.ArgumentException($"'{nameof(fileExtension)}' cannot be null or empty.", nameof(fileExtension));
            }

            FileExtension = fileExtension;
        }

        public string FileExtension { get; }
    }
}
