using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityShaderParser.Test
{
    public enum ExecutionScope
    {
        Function,
        Conditional,
        Loop,
    }

    public class HLSLExecutionState
    {
        private int threadsX, threadsY;
        private Stack<(ExecutionScope scope, bool[] mask)> executionMask;

        public HLSLExecutionState(int threadsX, int threadsY)
        {
            this.threadsX = threadsX;
            this.threadsY = threadsY;
            executionMask = new Stack<(ExecutionScope, bool[])>();

            var initial = new bool[threadsX * threadsY];
            Array.Fill(initial, true);
            executionMask.Push((ExecutionScope.Function, initial));
        }

        public void PushExecutionMask(ExecutionScope scope, bool[] mask)
        {
            executionMask.Push((ExecutionScope.Function, mask));
        }

        public void PushExecutionMask(ExecutionScope scope)
        {
            executionMask.Push((scope, executionMask.Peek().mask.ToArray()));
        }

        public void PopExecutionMask()
        {
            executionMask.Pop();
        }

        public void FlipExecutionMask()
        {
            for (int threadIndex = 0; threadIndex < GetThreadCount(); threadIndex++)
            {
                SetThreadState(threadIndex, !IsThreadActive(threadIndex));
            }
        }

        public bool IsThreadActive(int threadIndex)
        {
            return executionMask.Peek().mask[threadIndex];
        }

        public void DisableThread(int threadIndex)
        {
            executionMask.Peek().mask[threadIndex] = false;
        }

        public void EnableThread(int threadIndex)
        {
            executionMask.Peek().mask[threadIndex] = true;
        }

        public void SetThreadState(int threadIndex, bool state)
        {
            executionMask.Peek().mask[threadIndex] = state;
        }

        // Kill thread for the entire execution, i.e. 'discard'
        public void KillThreadGlobally(int threadIndex)
        {
            foreach (var level in executionMask)
            {
                level.mask[threadIndex] = false;
            }
        }

        // Kill thread for the current function, i.e. 'return'
        public void KillThreadInFunction(int threadIndex)
        {
            foreach (var level in executionMask)
            {
                level.mask[threadIndex] = false;
                if (level.scope == ExecutionScope.Function)
                    break;
            }
        }

        // Kill thread for the current loop, i.e. 'break'
        public void KillThreadInLoop(int threadIndex)
        {
            foreach (var level in executionMask)
            {
                level.mask[threadIndex] = false;
                if (level.scope == ExecutionScope.Loop)
                    break;
            }
        }

        public bool IsUniformExecution() => executionMask.Peek().mask.All(x => x);
        public bool IsVaryingExecution() => !IsUniformExecution();

        public int GetThreadIndex(int threadX, int threadY) => threadY * threadsX + threadX;
        public (int threadX, int threadY) GetThreadPosition(int threadIndex) => (threadIndex % threadsX, threadIndex / threadsX);
        public int GetThreadCount() => threadsX * threadsY;
        public int GetThreadsX() => threadsX;
        public int GetThreadsY() => threadsY;
    }
}
