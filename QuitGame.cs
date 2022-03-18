using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unitilities
{
    /// <summary>
    /// 退出游戏
    /// </summary>
    public static class QuitGame
    {
        public static void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }

}
