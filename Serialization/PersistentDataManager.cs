using System.IO;
using UnityEngine;
using UnityEditor;
using System.Diagnostics;

namespace Unitilities.Serialization
{
    public enum DataScope
    {
        /// <summary>
        /// 仅本地且与存档无关, 例如设备相关的画质设置, 按键映射等
        /// </summary>
        [Tooltip("仅本地且存档间共享")]
        LocalShared = 0,
        /// <summary>
        /// 与游戏存档/地图相关的数据, 例如存档本身和可选的存档附加数据. 每个存档会有一个专属的文件夹
        /// </summary>
        [Tooltip("存档数据")]
        Save = 1,
        /// <summary>
        /// 服务器相关的数据, 如服务器连接配置, 用户名, 缓存地图等. 每个服务器连接有一个专属的文件夹
        /// </summary>
        [Tooltip("服务器数据")]
        Server = 2,
    }

    /// <summary>
    /// 持久化数据管理器
    /// </summary>
    public static class PersistentDataManager
    {
        /* 关于路径相关的字符串: 
         所有表示文件夹和目录的变量都以 "Directory" 结尾, 字符串结尾有分隔符; 而所有具体到文件的变量都以 "Path" 结尾, 字符串结尾无分隔符.
         尽可能使用 System.IO.Path 与 System.IO.Directory 拼接和解析路径. 如有必要写出分隔符, 使用 '/'. (尽管 Path 的默认分隔符似乎是 '\')
         */

        /// <summary>
        /// 所有数据的根目录
        /// </summary>
        public static string RootDirectory { get; private set; }
        /// <summary>
        /// 存档数据的根目录
        /// </summary>
        public static string SaveRootDirectory { get; private set; }
        /// <summary>
        /// 服务器数据的根目录
        /// </summary>
        public static string ServerRootDirectory { get; private set; }

        public static string CurrentSave = "Default Save"; // TODO: 由于存档名 = 文件夹名, 可能需要校验或转义, 下同

        public static string CurrentServer = "Local";

        public static string CurrentSaveDir => Path.Combine(SaveRootDirectory, CurrentSave + '/');

        public static string CurrentServerDir => Path.Combine(ServerRootDirectory, CurrentServer + '/');

        public static ITextSerializer TextSerializer = JsonSerializer.Instance;

        public static IBinarySerializer BinarySerializer = null;

        static PersistentDataManager()
        {
            SetDefaultRootDirectory();
        }

        /// <summary>
        /// 设置默认的根目录
        /// </summary>
        public static void SetDefaultRootDirectory()
        {
#if UNITY_EDITOR
            SetRootDirectory(Path.GetFullPath("../PersistentData/", Application.dataPath));
#else
            SetRootDirectory(Path.GetFullPath("PersistentData/", Application.dataPath));
#endif
        }

        /// <summary>
        /// 设置并创建持久化数据根目录
        /// <para>不会影响设置前可能存在的有效根目录</para>
        /// </summary>
        public static void SetRootDirectory(string rootDir)
        {
            RootDirectory = Path.GetFullPath(rootDir);

            SaveRootDirectory = Path.GetFullPath("Saves/", RootDirectory);

            ServerRootDirectory = Path.GetFullPath("Servers/", RootDirectory);

            CreateRootDirectories();
        }

        [MenuItem("Unitilities/Persistent Data/Create PersistentData Root")]
        public static void CreateRootDirectories()
        {
            Directory.CreateDirectory(RootDirectory);
#if UNITY_EDITOR
            File.WriteAllText(Path.Combine(RootDirectory, ".gitignore"), "# Ignore All Local Data in this folder.\n*");
#endif

            Directory.CreateDirectory(SaveRootDirectory);
            Directory.CreateDirectory(CurrentSaveDir);

            Directory.CreateDirectory(ServerRootDirectory);
            Directory.CreateDirectory(CurrentServerDir);

            // Debug.Log("RootDirectories are created. This msg may be printed twiced, dont panic.");
        }

        public static string GetFullFilePath(string fileFullName, DataScope scope)
        {
            return scope switch
            {
                DataScope.LocalShared => Path.Combine(RootDirectory, fileFullName),
                DataScope.Save => Path.Combine(CurrentSaveDir, fileFullName),
                DataScope.Server => Path.Combine(CurrentServerDir, fileFullName),
                _ => Path.Combine(RootDirectory, fileFullName),
            };
        }

        /// <summary>
        /// 直接保存一段文本
        /// </summary>
        public static void SaveText(string content, string fileFullName, DataScope scope = DataScope.LocalShared)
        {
            var path = GetFullFilePath(fileFullName, scope);
            File.WriteAllText(path, content);
            // Debug.Log($"Text file \"{path}\" should have been saved.");
        }

        /// <summary>
        /// 以文本形式保存一个对象
        /// </summary>
        /// <param name="obj">待保存的对象. 若对象为 <see cref="ITextSerializable"/>, 则使用对象自己的序列化方法.</param>
        /// <param name="scope">数据的生存周期</param>
        public static void SaveObjectAsText(object obj, string fileNameNoExtend, DataScope scope = DataScope.LocalShared)
        {
            var file = fileNameNoExtend;
            if (obj is ITextSerializable serializable)
            {
                file += '.' + serializable.FileExtension;
                SaveText(serializable.Serialize(), file, scope);
            }
            else
            {
                file += '.' + TextSerializer.FileExtension;
                SaveText(TextSerializer.SerializeToText(obj), file, scope);
            }
        }

        /// <summary>
        /// 将自身保存到指定文本文件
        /// </summary>
        public static void SaveTo(this ITextSerializable obj, string fileNameNoExtend, DataScope scope = DataScope.LocalShared)
        {
            SaveText(obj.Serialize(), fileNameNoExtend + '.' + obj.FileExtension, scope);
        }

        /// <summary>
        /// 从指定文本文件读取一个对象
        /// </summary>
        public static T LoadObjectFromText<T>(string fileNameNoExtend, DataScope scope = DataScope.LocalShared) where T : new()
        {
            if (typeof(ITextSerializable).IsAssignableFrom(typeof(T)))
            {
                var temp = (ITextSerializable)new T();
                var file = fileNameNoExtend + '.' + temp.FileExtension;
                var path = GetFullFilePath(file, scope);
                if (!File.Exists(path)) return (T)temp;

                var text = File.ReadAllText(path);
                temp.Deserialize(text);
                return (T)temp;
            }
            else
            {
                var file = fileNameNoExtend + '.' + TextSerializer.FileExtension;
                var path = GetFullFilePath(file, scope);
                if (!File.Exists(path)) return new T();

                var text = File.ReadAllText(path);
                return TextSerializer.DeserializeFromText<T>(text);
            }

        }

        /// <summary>
        /// 从指定文本文件载入并覆盖自身的数据
        /// </summary>
        public static void LoadFrom(this ITextSerializable obj, string fileNameNoExtend, DataScope scope = DataScope.LocalShared)
        {
            string path = GetFullFilePath(fileNameNoExtend + '.' + obj.FileExtension, scope);
            obj.Deserialize(File.ReadAllText(path));
        }

        /// <summary>
        /// 直接保存一个字节数组
        /// </summary>
        public static void SaveBinary(byte[] data, string fileFullName, DataScope scope = DataScope.LocalShared)
        {
            var path = GetFullFilePath(fileFullName, scope);
            File.WriteAllBytes(path, data);
        }

        /// <summary>
        /// 以二进制形式保存一个对象
        /// <para><see cref="BinarySerializer"/> 为空时, 使用 <see cref="TextSerializer"/> 以文本保存</para>
        /// <param name="obj">若 obj 为 <see cref="IBinarySerializable"/>, 则使用对象自身的序列化方法</param>
        /// </summary>
        public static void SaveObjectAsBinary(object obj, string fileNameNoExtend, DataScope scope = DataScope.LocalShared)
        {
            if (obj is IBinarySerializable serializable)
            {
                var file = fileNameNoExtend + '.' + serializable.FileExtension;
                var path = GetFullFilePath(file, scope);
                using (var fileStream = File.OpenWrite(path))
                {
                    using (BinaryWriter writer = new BinaryWriter(fileStream))
                    {
                        serializable.Serialize(writer);
                    }
                }

            }
            else if (BinarySerializer == null)
            {
                SaveObjectAsText(obj, fileNameNoExtend, scope);
            }
            else
            {
                var file = fileNameNoExtend + '.' + BinarySerializer.FileExtension;
                var path = GetFullFilePath(file, scope);
                using (var fileStream = File.OpenWrite(path))
                {
                    using (BinaryWriter writer = new BinaryWriter(fileStream))
                    {
                        BinarySerializer.SerializeToBinary(obj, writer);
                    }
                }
            }
        }

        /// <summary>
        /// 将自身保存到指定二进制文件
        /// </summary>
        public static void SaveTo(this IBinarySerializable obj, string fileNameNoExtend, DataScope scope = DataScope.LocalShared)
        {
            var path = GetFullFilePath(fileNameNoExtend + '.' + obj.FileExtension, scope);
            using (var file = File.OpenWrite(path))
            {
                using (var writer = new BinaryWriter(file))
                {
                    obj.Serialize(writer);
                }
            }
        }

        /// <summary>
        /// 从指定二进制文件读取一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileNameNoExtend"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        public static T LoadObjectFromBinary<T>(string fileNameNoExtend, DataScope scope = DataScope.LocalShared) where T : new()
        {
            if (typeof(IBinarySerializable).IsAssignableFrom(typeof(T)))
            {
                var res = (IBinarySerializable)new T();
                var file = fileNameNoExtend + '.' + res.FileExtension;
                var path = GetFullFilePath(file, scope);
                if (!File.Exists(path)) return (T)res;

                using (var fileStream = File.OpenRead(path))
                {
                    using (var reader = new BinaryReader(fileStream))
                    {
                        res.Deserialize(reader);
                    }
                }
                return (T)res;
            }
            else if (BinarySerializer == null)
            {
                return LoadObjectFromText<T>(fileNameNoExtend, scope);
            }
            else
            {
                var file = fileNameNoExtend + '.' + BinarySerializer.FileExtension;
                var path = GetFullFilePath(file, scope);
                if (!File.Exists(path)) return new T();
                
                var res = new T();
                using (var fileStream = File.OpenRead(path))
                {
                    using (var reader = new BinaryReader(fileStream))
                    {
                        BinarySerializer.DeserializeFromBinary<T>(reader);
                    }
                }
                return res;
            }
        }

        /// <summary>
        /// 从指定二进制文件读取并覆盖自身数据
        /// </summary>
        public static void LoadFrom(this IBinarySerializable obj, string fileNameNoExtend, DataScope scope = DataScope.LocalShared)
        {
            var path = GetFullFilePath(fileNameNoExtend + '.' + obj.FileExtension, scope);
            using (var file = File.OpenRead(path))
            {
                using (var reader = new BinaryReader(file))
                {
                    obj.Deserialize(reader);
                }
            }
        }

        #region Menu Action
        [MenuItem("Unitilities/Persistent Data/Clear/All Data")]
        public static void ClearAllData()
        {
            Directory.Delete(RootDirectory, true);
            CreateRootDirectories();
        }

        [MenuItem("Unitilities/Persistent Data/Clear/Current Save")]
        public static void ClearCurrentSave()
        {
            Directory.Delete(CurrentSaveDir);
            Directory.CreateDirectory(CurrentSaveDir);
        }

        [MenuItem("Unitilities/Persistent Data/Clear/Current Server")]
        public static void ClearCurrentServer()
        {
            Directory.Delete(CurrentServerDir);
            Directory.CreateDirectory(CurrentServerDir);
        }

        [MenuItem("Unitilities/Persistent Data/Open In Explorer")]
        public static void OpenInExplorer()
        {
            Process.Start(RootDirectory);
        }
        #endregion

    }
}
