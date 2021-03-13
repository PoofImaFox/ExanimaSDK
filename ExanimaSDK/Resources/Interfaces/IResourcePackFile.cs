using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using ExanimaSDK.Resources.Interfaces;

namespace ExanimaSDK.Resources.Interfaces {
    public interface IResourcePackFile {
        /// <summary>
        /// This is the hard location of the file we are modifying.
        /// </summary>
        public string GameResourceFile { get; }

        /// <summary>
        /// This is the stream for the <see cref="GameResourceFile"/>
        /// </summary>
        public FileStream ResourcesFileStream { get; set; }

        /// <summary>
        /// Returns the number of files in the resource file.
        /// </summary>
        public int PackedFileCount { get; }

        /// <summary>
        /// Returns the offset to the beginning of the first packed file.
        /// </summary>
        public uint DataStartOffset { get; }

        /// <summary>
        /// Checks the file signature for a valid Rpk signature.
        /// </summary>
        public bool IsRpkFile { get; }

        /// <summary>
        /// Reads the packed file, and returns a byte[] of the file data.
        /// </summary>
        /// <param name="resourcePackFile"></param>
        /// <returns></returns>
        public byte[] ReadFileFromStream(IPackedFileInfo resourcePackFile);

        /// <summary>
        /// Reads the packed file, and returns a byte[] of the file data.
        /// </summary>
        /// <param name="resourcePackFile"></param>
        /// <returns></returns>
        public Task<byte[]> ReadFileFromStreamAsync(IPackedFileInfo resourcePackFile);

        /// <summary>
        /// This will read the contents on the packed resource file.
        /// </summary>
        /// <returns></returns>
        public IPackedFileInfo[] ReadPackedFiles();
    }
}