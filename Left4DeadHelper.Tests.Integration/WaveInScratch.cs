using NAudio.Wave;
using NUnit.Framework;

namespace Left4DeadHelper.Tests.Integration
{
    [Explicit]
    [TestFixture]
    public class WaveInScratch
    {
        [Test]
        public void GetDevices()
        {
            var devices = new WaveInCapabilities[WaveIn.DeviceCount];
            for (int n = 0; n < WaveIn.DeviceCount; n++)
            {
                devices[n] = WaveIn.GetCapabilities(n);
            }
        }
    }
}
