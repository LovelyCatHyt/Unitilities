using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Unitilities.DebugUtil;
using UnityEditor;
using UnityEngine;

namespace Unitilities.Serialization.Editor
{
    /// <summary>
    /// 对象引用表的编辑器
    /// </summary>
    [CustomEditor(typeof(ObjectRefTable))]
    public class ObjectRefTableEditor : UnityEditor.Editor
    {

        private SerializedProperty _refListProp;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            _refListProp = serializedObject.FindProperty("refList");

            var objectRefTable = (ObjectRefTable)target;
            // 反射暴力取字段
            var field = typeof(ObjectRefTable).GetField("refList", BindingFlags.NonPublic | BindingFlags.Instance);
            var refList = (List<Object>)field!.GetValue(objectRefTable);
            // 不允许引用一个叫 "null" 的对象
            if (refList.Exists(o => o && o.name == "null"))
            {
                EditorGUILayout.HelpBox("Shouldn't ref an object named \"null\"!", MessageType.Error);
            }

            // 查重, 但是不查 null 的重
            var dupList = refList
                .GroupBy(x => x != null ? x.name : "null")
                .Where(g => g.Count() > 1 && g.Count(o => o != null) > 1)
                .ToList();
            var errorMsgBuilder = new StringBuilder();
            for (var i = 0; i < dupList.Count; i++)
            {
                errorMsgBuilder.Append($"Object name \"{dupList[i].Key}\" conflict!");
                if (i < dupList.Count - 1) errorMsgBuilder.AppendLine();
            }
            // 重了就报错
            if (dupList.Count > 0)
            {
                EditorGUILayout.HelpBox(errorMsgBuilder.ToString(), MessageType.Error);
            }

            // 一键获取同目录下的对象引用, 
            if (GUILayout.Button("Set list as assets in directory"))
            {
                // 使用 Unity 的 API 获取当前对象的路径
                var tablePath = AssetDatabase.GetAssetPath(target);
                // 使用系统的文件目录 API 获取有效的表示 Object 的文件
                var folder = Path.GetDirectoryName(tablePath);
                var allAssetsExceptSelf = Directory.GetFiles(folder!)
                    .Where(p => !p.EndsWith(".meta"))
                    .ToList().Select(AssetDatabase.LoadAssetAtPath<Object>)
                    .Where(o => o != target).ToList();

                _refListProp.arraySize = allAssetsExceptSelf.Count;
                for (var i = 0; i < allAssetsExceptSelf.Count; i++)
                {
                    _refListProp.GetArrayElementAtIndex(i).objectReferenceValue = allAssetsExceptSelf[i];
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }

}
