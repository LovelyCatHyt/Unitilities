using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Unitilities.Serialization
{
    public class BinarySerializeUtils
    {
        public static void Serialize<T>(T[] list, BinaryWriter writer) where T : unmanaged
        {
            writer.Write(list.LongLength);
            writer.Write(MemoryMarshal.Cast<T, byte>(list));
        }

        public static T[] Deserialize<T>(BinaryReader reader) where T : unmanaged
        {
            var length = reader.ReadInt32();
            // TODO: is it possible to avoid copying?
            return MemoryMarshal.Cast<byte, T>(reader.ReadBytes(length)).ToArray();
        }
    }
}
