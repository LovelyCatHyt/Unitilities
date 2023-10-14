using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Unitilities.Serialization;
using System.IO;
using System;
using UnityEngine.TestTools;
using UnityEditor;
using System.Diagnostics;

namespace Unitilities.Test
{
    public struct TestObj
    {
        public string str;
    }

    public struct TextObject : ITextSerializable
    {
        public string content;

        public string FileExtension => "txt";
        public void Deserialize(string data) => content = data;
        public string Serialize() => content;

        public override bool Equals(object obj)
        {
            if (obj is TextObject other) return content == other.content;
            return false;
        }

        public override int GetHashCode() => content.GetHashCode();
    }

    public struct BinaryObject : IBinarySerializable
    {
        public int[] data;

        public long SerializeByteLength => data.LongLength;

        public string FileExtension => "dat";

        public void Deserialize(BinaryReader reader)
        {
            data = BinarySerializeUtils.Deserialize<int>(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            BinarySerializeUtils.Serialize(data, writer);
        }

        public override bool Equals(object obj)
        {
            if(obj is BinaryObject other)
            {
                if (data == null) return other.data == null;
                if (other.data == null) return false;
                if(data.Length != other.data.Length) return false;
                for (int i = 0; i < data.Length; i++)
                {
                    if (data[i] != other.data[i]) return false;
                }
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return data.GetHashCode();
        }

        public BinaryObject Copy()
        {
            BinaryObject res = new BinaryObject{ data=new int[data.Length]};
            Array.Copy(data, res.data, data.Length);
            return res;
        }
    }

    public class PersistentDataManager_Test : IPrebuildSetup, IPostBuildCleanup
    {
        public void Setup()
        {
            PersistentDataManager.SetRootDirectory(Path.GetFullPath("../TestPersistentData/", Application.dataPath));
            PersistentDataManager.ClearAllData();
        }

        public void Cleanup()
        {
            PersistentDataManager.SetDefaultRootDirectory();
        }

        [Test]
        public void Test_TextSerializer()
        {
            TestObj abc = new TestObj { str = "qwerty\n123456"};
            PersistentDataManager.SaveObjectAsText(abc, nameof(Test_TextSerializer));
            var loaded = PersistentDataManager.LoadObjectFromText<TestObj>(nameof(Test_TextSerializer));
            Assert.AreEqual(abc, loaded);
        }

        [Test]
        public void Test_BinarySerializer()
        {
            TestObj abc = new TestObj { str = "qwerty\n123456" };
            PersistentDataManager.SaveObjectAsBinary(abc, nameof(Test_BinarySerializer));
            var loaded = PersistentDataManager.LoadObjectFromBinary<TestObj>(nameof(Test_BinarySerializer));
            Assert.AreEqual(abc, loaded);
        }

        [Test]
        public void Test_TextSerializable()
        {
            TextObject obj = new TextObject { content = "123qwe\t456rty" };
            var copy = obj;
            obj.SaveTo(nameof(Test_TextSerializable));
            obj.LoadFrom(nameof(Test_TextSerializable));
            Assert.AreEqual(copy, obj);
        }

        [Test]
        public void Test_BinarySerializable()
        {
            BinaryObject obj = new BinaryObject();
            const int Length = 16;
            obj.data = new int[Length];
            for (int i = 0; i < Length; i++)
            {
                obj.data[i] = i.GetHashCode();
            }
            var copy = obj.Copy();
            obj.SaveTo(nameof(Test_BinarySerializable));
            obj.LoadFrom(nameof(Test_BinarySerializable));
            Assert.AreEqual(copy, obj);
        }

        [MenuItem("Unitilities/Persistent Data/Test/Open In Explorer")]
        public static void OpenInExplorer()
        {
            Process.Start("explorer.exe", Path.GetFullPath("../TestPersistentData/", Application.dataPath));
        }
    }
}
