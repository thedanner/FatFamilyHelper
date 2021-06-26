using Left4DeadHelper.Helpers.Extensions;
using NUnit.Framework;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Left4DeadHelper.Tests.Unit
{
    [TestFixture]
    public class RngExtensionsTests
    {
        private RNGCryptoServiceProvider _random;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _random = new RNGCryptoServiceProvider();
        }

        [Test]
        [Timeout(10000)] // Don't wait longer than 10 seconds.
        [TestCaseSource(nameof(GetByteIndexRange))]
        public void PickRandom_AllArraySizes_Returns(byte size)
        {
            // Arrange
            var list = new byte[size];
            for (var i = 1; i < list.Length; i++)
            {
                list[i] = (byte)i;
            }

            // Act
            _random.PickRandom(list);

            // Assert
            // The [Timeout] will stop the test if gets stuck in a loop.
            // We don't need to explicitly check for anything.
        }

        private static IEnumerable<byte> GetByteIndexRange()
        {
            for (var i = 2; i < byte.MaxValue; i++)
            {
                yield return (byte) i;
            }
        }
    }
}
