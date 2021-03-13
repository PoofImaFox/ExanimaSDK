using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ExanimaSDK.Resources.Interfaces;

namespace ExanimaSDK.Resources {
    public class PackedFileInfo : IPackedFileInfo {
        public string Name { get; set; } = default!;

        public string? UnpackedFileLocation { get; set; }

        public uint Size { get; set; }

        public uint Offset { get; set; }

        public string FileType { get; set; } = default!;

        public string Extension { get; set; } = default!;
    }
}
