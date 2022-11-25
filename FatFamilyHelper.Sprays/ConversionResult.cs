using System;

namespace FatFamilyHelper.Sprays;

public class ConversionResult
{
    public bool IsSuccessful { get; private set; }
    public string? Message { get; private set; }
    public string? FileExtension { get; private set; }

    public static ConversionResult Pass(string fileExtension)
    {
        if (string.IsNullOrEmpty(fileExtension))
        {
            throw new ArgumentException($"'{nameof(fileExtension)}' cannot be null or empty.", nameof(fileExtension));
        }

        return new ConversionResult
        {
            IsSuccessful = true,
            FileExtension = fileExtension
        };
    }

    public static ConversionResult Fail(string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            throw new ArgumentException($"'{nameof(message)}' cannot be null or empty.", nameof(message));
        }

        return new ConversionResult
        {
            IsSuccessful = false,
            Message = message
        };
    }
}
