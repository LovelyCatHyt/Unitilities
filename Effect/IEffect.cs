using System;

namespace Unitilities.Effect
{
    /// <summary>
    /// 特效
    /// </summary>
    public interface IEffect
    {
        // 播放
        void Play();
        // 强制停止
        void Stop();
        // 添加结束监听器, 在正常结束或强制停止时调用
        void AddFinishListener(Action action);
    }
}
