using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExanimaSDK.Resources {
    public static class ResourcePackType {
        public static string GetFileType(byte[] signature) {
            if (signature.Take(4).SequenceEqual(FilePackedType.Wave)) {
                return "wav";
            }

            if (signature.Take(4).SequenceEqual(FilePackedType.Texture)) {
                return "dds";
            }

            return default!;
        }

        public static byte[] Rpk = new byte[] {
            0x01,
            0x0C,
            0xBF,
            0xAF
        };
    }
}