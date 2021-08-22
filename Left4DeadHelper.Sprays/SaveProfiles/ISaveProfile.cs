using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Left4DeadHelper.Sprays.SaveProfiles
{
    public interface ISaveProfile<TImageConfiguration>
    {
        string Extension { get; }

        void Validate();

        Task ConvertAsync(TImageConfiguration imageConfiguration, Stream outputStream, CancellationToken cancellationToken);
    }
}
