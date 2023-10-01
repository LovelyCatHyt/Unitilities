using System.Collections.Generic;

namespace Unitilities
{
    /// <summary>
    /// 可撤回操作
    /// </summary>
    public interface IRecovableOperation
    {
        void Execute();
        void Undo();
        /// <summary>
        /// 将自身与新的操作合并, 可合并则返回true
        /// </summary>
        /// <param name="operation">待合并的操作</param>
        /// <returns></returns>
        bool Merge(IRecovableOperation operation);
    }

    /// <summary>
    /// 操作链
    /// </summary>
    public class OperationChain
    {
        private List<IRecovableOperation> operations;
        /// <summary>
        /// 指向当前最新[执行过的]操作
        /// </summary>
        private int operationPointer;
        private IRecovableOperation CurrentOper
        {
            get => operations[operationPointer];
        }
        /// <summary>
        /// 创建空的操作链
        /// </summary>
        public OperationChain()
        {
            operations = new List<IRecovableOperation>();
            operationPointer = -1;
        }
        /// <summary>
        /// 使用外部构建的操作列表来构建一个操作链
        /// </summary>
        /// <param name="operations">操作列表</param>
        /// <param name="pos">初始操作位置</param>
        public OperationChain(List<IRecovableOperation> operations, int pos = -1)
        {
            this.operations = operations;
            this.operationPointer = pos;
        }
        
        /// <summary>
        /// 当前位置, -1表示未执行任何操作
        /// </summary>
        public int CurrentPos => operationPointer;
        /// <summary>
        /// 已储存的操作数量
        /// </summary>
        public int StoredOperCount => operations.Count;
        /// <summary>
        /// 在起始位置
        /// </summary>
        public bool IsBegin => operationPointer == -1;
        /// <summary>
        /// 在结束位置
        /// </summary>
        public bool IsEnd => operationPointer + 1 == operations.Count;

        public void AddAndExcuteOperation(IRecovableOperation operation, bool merge = true)
        {
            // 若当前位置存在未来操作, 移除当前节点之后的所有操作(除非我闲得发疯搞了个git一样的管理系统, 不然当然是直接删除)
            if(operations.Count > operationPointer + 1)
            {
                operations.RemoveRange(operationPointer + 1, operations.Count - operationPointer - 1);
            }
            // 执行操作
            operation.Execute();
            // 前面至少有一个操作的情况下
            if(operationPointer >= 0)
            {
                if(merge && CurrentOper.Merge(operation))
                {
                    // 允许merge且成功merge的情况下直接结束 
                    return;
                }
            }
            operations.Add(operation);
            operationPointer++;
        }

        /// <summary>
        /// 撤回
        /// </summary>
        public bool Undo()
        {
            //起码有一个能撤回
            if(operationPointer >= 0)
            {
                operations[operationPointer].Undo();
                operationPointer--;
                return true;
            }
            return false;
        }
        /// <summary>
        /// 重做
        /// </summary>
        public bool Redo()
        {
            //起码多出一个操作
            if(operations.Count > operationPointer + 1)
            {
                operationPointer++;
                operations[operationPointer].Execute();
                return true;
            }
            return false;
        }
        /// <summary>
        /// 撤销全部操作, 同时移除所有记录
        /// </summary>
        public void UndoAll(bool removeAll = false)
        {
            while(Undo())
            {
            
            }
            if(removeAll)
            {
                operations.Clear();
            }
        }
    }
}