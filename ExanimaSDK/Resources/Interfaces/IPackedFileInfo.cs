using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ExanimaSDK.Resources.Interfaces {
    public interface IPackedFileInfo {
        /// <summary>
        /// The name/game ID of the file.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The unpacked file location. <see cref="UnpackedFileLocation"/> will be null if the file is not unpacked.
        /// </summary>
        public string? UnpackedFileLocation { get; set; }

        /// <summary>
        /// This is the size of the file data.
        /// </summary>
        public uint Size { get; set; }

        /// <summary>
        /// The offset of the packed file in the resource file. Starting at the end of the list files info head.
        /// </summary>
        public uint Offset { get; set; }

        /// <summary>
        /// The mime type of the packed file.
        /// </summary>
        public string FileType { get; set; }

        /// <summary>
        /// The file path extension of the packed file. <see cref="Extension"/> is found from the <see cref="FileType"/> property.
        /// </summary>
        public string Extension { get; set; }
    }
}
