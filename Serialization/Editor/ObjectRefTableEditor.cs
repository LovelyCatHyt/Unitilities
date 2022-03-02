using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
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
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var objectRefTable = (ObjectRefTable)target;
            var field = typeof(ObjectRefTable).GetField("refList", BindingFlags.NonPublic | BindingFlags.Instance);
            var refList = (List<Object>) field.GetValue(objectRefTable);
            if (refList.Exists(o => o && o.name == "null"))
            {
                EditorGUILayout.HelpBox("Shouldn't ref an object named \"null\"!", MessageType.Error);
            }

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

            if (dupList.Count > 0)
            {
                EditorGUILayout.HelpBox(errorMsgBuilder.ToString(), MessageType.Error);
            }
        }
    }

}
