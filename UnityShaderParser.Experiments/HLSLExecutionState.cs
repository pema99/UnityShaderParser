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
        Block,
    }

    public class HLSLExecutionState
    {
        private enum ThreadState : byte
        {
            Active,    // Alive
            Inactive,  // Helper lane or disabled by return, break
            Suspended, // Disabled by continue
        }

        private int threadsX, threadsY;
        private Stack<(ExecutionScope scope, ThreadState[] mask)> executionMask;

        public HLSLExecutionState(int threadsX, int threadsY)
        {
            this.threadsX = threadsX;
            this.threadsY = threadsY;
            executionMask = new Stack<(ExecutionScope, ThreadState[])>();

            var initial = new ThreadState[threadsX * threadsY];
            Array.Fill(initial, ThreadState.Active);
            executionMask.Push((ExecutionScope.Function, initial));
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
                var threadState = executionMask.Peek().mask[threadIndex];
                if (threadState == ThreadState.Active)
                    DisableThread(threadIndex);
                else if (threadState == ThreadState.Inactive)
                    EnableThread(threadIndex);
            }
        }

        public bool IsThreadActive(int threadIndex)
        {
            return executionMask.Peek().mask[threadIndex] == ThreadState.Active;
        }

        public void DisableThread(int threadIndex)
        {
            executionMask.Peek().mask[threadIndex] = ThreadState.Inactive;
        }

        public void EnableThread(int threadIndex)
        {
            executionMask.Peek().mask[threadIndex] = ThreadState.Active;
        }

        // Kill thread for the entire execution, i.e. 'discard'
        public void KillThreadGlobally(int threadIndex)
        {
            foreach (var level in executionMask)
            {
                level.mask[threadIndex] = ThreadState.Inactive;
            }
        }

        // Kill thread for the current function, i.e. 'return'
        public void KillThreadInFunction(int threadIndex)
        {
            foreach (var level in executionMask)
            {
                level.mask[threadIndex] = ThreadState.Inactive;
                if (level.scope == ExecutionScope.Function)
                    break;
            }
        }

        // Kill thread for the current loop, i.e. 'break'
        public void KillThreadInLoop(int threadIndex)
        {
            foreach (var level in executionMask)
            {
                level.mask[threadIndex] = ThreadState.Inactive;
                if (level.scope == ExecutionScope.Loop)
                    break;
            }
        }

        // Kill thread for the current conditional, used for switch statements
        public void KillThreadInConditional(int threadIndex)
        {
            foreach (var level in executionMask)
            {
                level.mask[threadIndex] = ThreadState.Inactive;
                if (level.scope == ExecutionScope.Conditional)
                    break;
            }
        }

        // Suspend thread for the current loop, i.e. 'continue'
        public void SuspendThreadInLoop(int threadIndex)
        {
            foreach (var level in executionMask)
            {
                if (level.mask[threadIndex] == ThreadState.Active)
                    level.mask[threadIndex] = ThreadState.Suspended;

                if (level.scope == ExecutionScope.Loop)
                    break;
            }
        }

        // Resume previously suspended threads in loop body
        public void ResumeSuspendedThreadsInLoop()
        {
            foreach (var level in executionMask)
            {
                for (int threadIndex = 0; threadIndex < GetThreadCount(); threadIndex++)
                {
                    if (level.mask[threadIndex] == ThreadState.Suspended)
                        level.mask[threadIndex] = ThreadState.Active;
                }

                if (level.scope == ExecutionScope.Loop)
                    break;
            }
        }

        public bool IsUniformExecution() => executionMask.Peek().mask.All(x => x == ThreadState.Active);
        public bool IsVaryingExecution() => !IsUniformExecution();

        public int GetThreadIndex(int threadX, int threadY) => threadY * threadsX + threadX;
        public (int threadX, int threadY) GetThreadPosition(int threadIndex) => (threadIndex % threadsX, threadIndex / threadsX);
        public int GetThreadCount() => threadsX * threadsY;
        public int GetThreadsX() => threadsX;
        public int GetThreadsY() => threadsY;
    }
}
