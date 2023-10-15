using System.IO;

namespace Unitilities.Serialization
{
    public interface ISerializer
    {
        string FileExtension { get; }
    }

    public interface ISerializable
    {
        string FileExtension { get; }
    }

    public interface ITextSerializable : ISerializable
    {
        /// <summary>
        /// 对象本身序列化为文本
        /// </summary>
        /// <returns></returns>
        string Serialize();
        /// <summary>
        /// 从文本反序列化到对象本身
        /// </summary>
        /// <param name="data"></param>
        void Deserialize(string data);
    }

    public interface IBinarySerializable : ISerializable
    {
        /// <summary>
        /// 当前状态序列化后的字节长度, 包括长度描述数据本身
        /// </summary>
        long SerializeByteLength { get; }
        void Serialize(BinaryWriter writer);
        void Deserialize(BinaryReader reader);
    }

    /// <summary>
    /// 文本序列化接口
    /// </summary>
    public interface ITextSerializer : ISerializer
    {
        string SerializeToText(object obj);
        T DeserializeFromText<T>(string text) where T : new();
    }

    /// <summary>
    /// 二进制序列化接口
    /// </summary>
    public interface IBinarySerializer : ISerializer
    {
        bool IsTypeSupported<T>();
        void SerializeToBinary(object obj, BinaryWriter binaryWriter);
        T DeserializeFromBinary<T>(BinaryReader binaryReader);
    }
}
