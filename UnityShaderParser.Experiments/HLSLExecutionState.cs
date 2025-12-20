using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityShaderParser.Test
{
    public class HLSLExecutionState
    {
        private int threadsX, threadsY;
        private Stack<bool[]> executionMask;

        public HLSLExecutionState(int threadsX, int threadsY)
        {
            this.threadsX = threadsX;
            this.threadsY = threadsY;
            executionMask = new Stack<bool[]>();

            var initial = new bool[threadsX * threadsY];
            Array.Fill(initial, true);
            executionMask.Push(initial);
        }

        public void PushExecutionMask(bool[] mask)
        {
            executionMask.Push(mask);
        }

        public void PushExecutionMask()
        {
            executionMask.Push(executionMask.Peek().ToArray());
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
            return executionMask.Peek()[threadIndex];
        }

        public void DisableThread(int threadIndex)
        {
            executionMask.Peek()[threadIndex] = false;
        }

        public void EnableThread(int threadIndex)
        {
            executionMask.Peek()[threadIndex] = true;
        }

        public void SetThreadState(int threadIndex, bool state)
        {
            executionMask.Peek()[threadIndex] = state;
        }

        public bool IsUniformExecution() => executionMask.Peek().All(x => x);
        public bool IsVaryingExecution() => !IsUniformExecution();

        public int GetThreadIndex(int threadX, int threadY) => threadY * threadsX + threadX;
        public (int threadX, int threadY) GetThreadPosition(int threadIndex) => (threadIndex % threadsX, threadIndex / threadsX);
        public int GetThreadCount() => threadsX * threadsY;
    }
}
