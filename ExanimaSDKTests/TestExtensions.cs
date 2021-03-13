using System;
using System.IO;

using ExanimaSDK.Extensions;

using Xunit;

namespace ExanimaSDKTests {
    public class TestExtensions {
        [Fact]
        public void TestPatternMatchingSwitch() {
            var verificationNumber = 8675309u;
            var stream = File.Open("testFile", FileMode.Create);
            stream.Write(BitConverter.GetBytes(verificationNumber));

            stream.Position = 0;
            var convertedBytes = stream.Read<uint>();

            Assert.Equal(verificationNumber, convertedBytes);

            stream.Dispose();
            File.Delete("testFile");
        }

        [Fact]
        public void TestWriteGenericTypeBytes() {
            var stream = File.Open("testFile", FileMode.Create);
            var verificationNumber = 8675309u;

            stream.Write(verificationNumber);

            stream.Position = 0;

            var foundValue = stream.Read<uint>();
            Assert.Equal(verificationNumber, foundValue);

            stream.Dispose();
            File.Delete("testFile");
        }

        [Fact]
        public void TestWriteBytesWithGeneric() {
            var stream = File.Open("testFile", FileMode.Create);
            var verificationNumber = new byte[]{
                0xAA,
                0xAF,
                0xFF,
                0x0A,
            };

            stream.Write(verificationNumber);

            stream.Position = 0;

            var foundValue = stream.ReadBytes(verificationNumber.Length);
            Assert.Equal(verificationNumber, foundValue);

            stream.Dispose();
            File.Delete("testFile");
        }
    }
}