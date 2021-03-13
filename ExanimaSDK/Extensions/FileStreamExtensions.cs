using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ExanimaSDK.Extensions {
    public static class FileStreamExtensions {
        /// <summary>
        /// Reads bytes from the stream. <c>offset</c> will offset the stream position. This will increment the stream position; <c>count</c> + <c>offset</c>
        /// </summary>
        /// <param name="fileStream"></param>
        /// <param name="count"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static async Task<byte[]> ReadBytesAsync(this FileStream fileStream, int count, long offset = 0) {
            var buffer = new byte[count];
            fileStream.Position += offset;
            await fileStream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
            return buffer;
        }

        /// <summary>
        /// Reads bytes from the stream. <c>offset</c> will offset the stream position. This will increment the stream position; <c>count</c> + <c>offset</c>
        /// </summary>
        /// <param name="fileStream"></param>
        /// <param name="count"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static byte[] ReadBytes(this FileStream fileStream, int count, long offset = 0) {
            var buffer = new byte[count];
            fileStream.Position += offset;
            fileStream.Read(buffer, 0, buffer.Length);
            return buffer;
        }

        /// <summary>
        /// Reads the specified data type to a variable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileStream"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static T Read<T>(this FileStream fileStream, long offset = 0) {
            var convertedVariable = Activator.CreateInstance<T>();

            var convertedData = convertedVariable switch {
                ushort => BitConverter.ToUInt16(fileStream.ReadBytes(2, offset), 0),
                short => BitConverter.ToInt16(fileStream.ReadBytes(2, offset), 0),
                uint => BitConverter.ToUInt32(fileStream.ReadBytes(4, offset), 0),
                int => BitConverter.ToInt32(fileStream.ReadBytes(4, offset), 0),
                ulong => BitConverter.ToUInt64(fileStream.ReadBytes(8, offset), 0),
                long => BitConverter.ToInt64(fileStream.ReadBytes(8, offset), 0),
                float => BitConverter.ToSingle(fileStream.ReadBytes(8, offset), 0),
                _ => throw new NotImplementedException()
            };

            return (T)Convert.ChangeType(convertedData, typeof(T));
        }

        /// <summary>
        /// Converts the write the value to the stream.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileStream"></param>
        /// <param name="data"></param>
        public static void Write<T>(this FileStream fileStream, T data) {
            _ = data ?? throw new ArgumentNullException(nameof(data));

            if (data is byte[]) {
                var dataArray = (byte[])(object)data;
                fileStream.Write(dataArray, 0, dataArray.Length);
                return;
            }

            var getBytesMethod = typeof(BitConverter).GetMethod("GetBytes", new[] { typeof(T) })
                ?? throw new Exception($"Could not convert {nameof(data)}: '{data}' to type '{typeof(T).Name}'");

            var dataBuffer = (byte[])getBytesMethod.Invoke(null, new object[] { data });
            fileStream.Write(dataBuffer, 0, dataBuffer.Length);
        }
    }
}