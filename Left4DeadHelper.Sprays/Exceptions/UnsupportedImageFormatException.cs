using System;
using System.Collections.Generic;
using System.Linq;

namespace Left4DeadHelper.Sprays.Exceptions
{
    [Serializable]
    public class UnsupportedImageFormatException : Exception
    {
        public UnsupportedImageFormatException(IEnumerable<string> inputMimeTypes) : this(inputMimeTypes, null)
        {
        }

        public UnsupportedImageFormatException(string? message, IEnumerable<string> inputMimeTypes) : this(message, inputMimeTypes, null)
        {
        }

        public UnsupportedImageFormatException(IEnumerable<string> inputMimeTypes, Exception? innerException) :
            this($"None of the following MIME types are currently supported: \"{string.Join("\", \"", inputMimeTypes)}\".", inputMimeTypes, innerException)
        {
        }

        public UnsupportedImageFormatException(string? message, IEnumerable<string> inputMimeTypes, Exception? innerException) :
            base(message, innerException)
        {
            if (inputMimeTypes == null) throw new ArgumentNullException(nameof(inputMimeTypes));
            InputMimeTypes = inputMimeTypes.ToList().AsReadOnly();
        }

        public IReadOnlyCollection<string> InputMimeTypes { get; private set; }
    }
}
