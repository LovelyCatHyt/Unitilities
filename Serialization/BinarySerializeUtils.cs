using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Unitilities.Serialization
{
    public class BinarySerializeUtils
    {
        /// <summary>
        /// 将结构体数组写入到 <see cref="BinaryWriter"/> 中
        /// </summary>
        /// <param name="hasSizeHint">设置为 true 则在开头添加一个整型表示要写入的数据的长度</param>
        public static void Serialize<T>(T[] list, BinaryWriter writer, bool hasSizeHint = true) where T : struct
        {
            if (hasSizeHint) writer.Write(list.LongLength);
            writer.Write(MemoryMarshal.Cast<T, byte>(list));
        }

        /// <summary>
        /// 从 <see cref="BinaryReader"/> 中读取一个结构体数组
        /// </summary>
        /// <param name="sizeOfByte">提前确定的字节长度, 若指定不大于0的数则从 <see cref="BinaryReader"/> 中读取一个 int 长度的整数</param>
        public static T[] Deserialize<T>(BinaryReader reader, int sizeOfByte = -1) where T : struct
        {
            sizeOfByte = sizeOfByte > 0 ? sizeOfByte : reader.ReadInt32();
            // TODO: is it possible to avoid copying?
            return MemoryMarshal.Cast<byte, T>(reader.ReadBytes(sizeOfByte)).ToArray();
        }
    }
}
