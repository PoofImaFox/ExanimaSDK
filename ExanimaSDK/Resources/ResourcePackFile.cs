using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ExanimaSDK.Extensions;
using ExanimaSDK.Resources.Interfaces;

namespace ExanimaSDK.Resources {
    public class ResourcePackFile : IResourcePackFile, IDisposable {
        public string GameResourceFile { get; }
        public FileStream ResourcesFileStream { get; set; }

        public int PackedFileCount {
            get {
                return (int)(_fileListDataSize / 32u);
            }
        }
        public uint DataStartOffset { get; }

        public bool IsRpkFile {
            get {
                return _fileSignature.SequenceEqual(ResourcePackType.Rpk);
            }
        }

        private const uint FILE_LIST_OFFSET = 8;

        private readonly byte[] _fileSignature;
        private readonly uint _fileListDataSize;

        public ResourcePackFile(string gameFile) {
            GameResourceFile = gameFile;
            ResourcesFileStream = File.OpenRead(gameFile);

            _fileSignature = ResourcesFileStream.ReadBytes(4);
            _fileListDataSize = ResourcesFileStream.Read<uint>();
        }

        ~ResourcePackFile() {
            ResourcesFileStream.Dispose();
        }

        public IPackedFileInfo[] ReadPackedFiles() {
            ResourcesFileStream.Position = FILE_LIST_OFFSET;

            var packedFiles = new IPackedFileInfo[PackedFileCount];
            for (var x = 0; x < packedFiles.Length; x++) {
                packedFiles[x] = new PackedFileInfo {
                    Name = Encoding.UTF8.GetString(ResourcesFileStream.ReadBytes(16)).Split('\0')[0]
                    ?? throw new Exception("Packed file name could not be read."),

                    Offset = ResourcesFileStream.Read<uint>(),
                    Size = ResourcesFileStream.Read<uint>(),
                };

                var currentReadPosition = ResourcesFileStream.Position;
                ResourcesFileStream.Position = packedFiles[x].Offset;
                packedFiles[x].Extension = ResourcePackType.GetFileType(ResourcesFileStream.ReadBytes(16));
                ResourcesFileStream.Position = currentReadPosition;

                // Fix reader alignment.
                ResourcesFileStream.Read<long>();
            }
            return packedFiles;
        }

        public byte[] ReadFileFromStream(IPackedFileInfo resourcePackFile) {
            return ReadFileFromStreamAsync(resourcePackFile).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public async Task<byte[]> ReadFileFromStreamAsync(IPackedFileInfo resourcePackFile) {
            ResourcesFileStream.Position = FILE_LIST_OFFSET + _fileListDataSize + resourcePackFile.Offset;
            return await ResourcesFileStream.ReadBytesAsync((int)resourcePackFile.Size);
        }

        public async Task UnpackFile(IPackedFileInfo resourcePackFile, string unpackDirectory) {
            var fileData = await ReadFileFromStreamAsync(resourcePackFile);

            resourcePackFile.UnpackedFileLocation = $"{unpackDirectory}\\{resourcePackFile.Name}{resourcePackFile.Extension}";
            File.WriteAllBytes(resourcePackFile.UnpackedFileLocation, fileData);
        }

        public void Dispose() {
            ResourcesFileStream.Dispose();
        }

        /// <summary>
        /// This will pack all files supplied into a resource file. If none is specified it will overwrite the existing resource file.
        /// </summary>
        /// <param name="filesToPack"></param>
        /// <param name="resourceFileToProduce"></param>
        public static void PackFiles(IPackedFileInfo[] filesToPack, string resourceFileToProduce) {
            var fileToPack = File.Open(resourceFileToProduce, FileMode.Create);

            WriteFileBody(ref filesToPack, fileToPack);
            WriteFileInfoHead(filesToPack, fileToPack);
            fileToPack.Dispose();
        }

        /// <summary>
        /// Write the head of the file, with file property information.
        /// </summary>
        /// <param name="filesToPack"></param>
        /// <param name="fileStream"></param>
        private static void WriteFileInfoHead(IPackedFileInfo[] filesToPack, FileStream fileStream) {
            fileStream.Position = 0;
            fileStream.Write(ResourcePackType.Rpk);
            fileStream.Write((uint)filesToPack.Length * 32u);

            foreach (var fileInfo in filesToPack) {
                var nameBytes = Encoding.UTF8.GetBytes(fileInfo.Name);
                Array.Resize(ref nameBytes, 16);

                fileStream.Write(nameBytes);
                fileStream.Write(fileInfo.Offset);
                fileStream.Write(fileInfo.Size);

                // Fix file alignment.
                fileStream.Write(0ul);
            }
        }

        /// <summary>
        /// Write the file body, and update the size and offset of the files written.
        /// </summary>
        /// <param name="filesToPack"></param>
        private static void WriteFileBody(ref IPackedFileInfo[] filesToPack, FileStream fileStream) {
            fileStream.Position = (uint)filesToPack.Length * 32u;

            foreach (var fileInfo in filesToPack) {
                fileInfo.Offset = (uint)fileStream.Position;
                var fileData = File.ReadAllBytes(fileInfo.UnpackedFileLocation
                    ?? throw new Exception($"{nameof(fileInfo.UnpackedFileLocation)} was null when trying to read file location."));

                fileInfo.Size = (uint)fileData.Length;
                fileStream.Write(fileData);
            }
        }
    }
}