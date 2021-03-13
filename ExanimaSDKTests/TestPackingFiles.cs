using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ExanimaSDK.Resources;
using ExanimaSDK.Resources.Interfaces;

using Xunit;

namespace ExanimaSDKTests {
    public class TestPackingFiles {
        public string MakeMockFiles(int numberOfFiles, int fileSize = 350) {
            var packedFileInfo = new IPackedFileInfo[numberOfFiles];

            var rng = new Random();
            Directory.CreateDirectory("unpackedFiles");
            var randomBuffer = new byte[fileSize];
            for (var x = 0; x < numberOfFiles; x++) {
                packedFileInfo[x] = new PackedFileInfo();
                rng.NextBytes(randomBuffer);

                packedFileInfo[x].Name = $"testfile{x}";
                packedFileInfo[x].UnpackedFileLocation = $"unpackedFiles\\{packedFileInfo[x].Name}.bin";
                File.WriteAllBytes(packedFileInfo[x].UnpackedFileLocation, randomBuffer);
            }

            var packedFile = "testFile.rpk";

            ResourcePackFile.PackFiles(packedFileInfo, packedFile);
            Directory.Delete("unpackedFiles", true);
            return packedFile;
        }

        [Fact]
        public void TestPackingRoundTrip() {
            var fileCount = 200;

            var packedRpkFile = MakeMockFiles(fileCount);
            var resourceFile = new ResourcePackFile(packedRpkFile);
            var packedFiles = resourceFile.ReadPackedFiles();

            Assert.True(resourceFile.IsRpkFile);
            Assert.Equal(fileCount, resourceFile.PackedFileCount);

            var unpackLocation = "unpackedTestdir";
            Directory.CreateDirectory(unpackLocation);

            var runningTaskList = new Task[packedFiles.Length];
            for (var x = 0; x < packedFiles.Length; x++) {
                Assert.StartsWith("testfile", packedFiles[x].Name);

                runningTaskList[x] = resourceFile.UnpackFile(packedFiles[x], unpackLocation);
            }

            Task.WaitAll(runningTaskList);

            var repackedRpkFile = "repackTest.rpk";
            ResourcePackFile.PackFiles(packedFiles, repackedRpkFile);
            var repackedRpk = new ResourcePackFile(repackedRpkFile);

            Assert.True(repackedRpk.IsRpkFile);
            Assert.Equal(fileCount, repackedRpk.PackedFileCount);

            Assert.Equal(new FileInfo(packedRpkFile).Length,
                new FileInfo(repackedRpkFile).Length);

            resourceFile.Dispose();
            repackedRpk.Dispose();

            File.Delete(packedRpkFile);
            File.Delete(repackedRpkFile);
            Directory.Delete(unpackLocation, true);
        }
    }
}