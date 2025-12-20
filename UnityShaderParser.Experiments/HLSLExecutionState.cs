using System;
using System.Collections.Generic;
using System.Text;

namespace UnityShaderParser.Test
{
    public class HLSLExecutionState
    {
        private int threadsX, threadsY;
        private bool[] executionMask;

        public HLSLExecutionState(int threadsX, int threadsY)
        {
            this.threadsX = threadsX;
            this.threadsY = threadsY;
            executionMask = new bool[threadsX * threadsY];
        }

        public bool IsThreadActive(int threadIndex)
        {
            return executionMask[threadIndex];
        }

        public void DisableThread(int threadIndex)
        {
            executionMask[threadIndex] = false;
        }

        public void EnableThread(int threadIndex)
        {
            executionMask[threadIndex] = true;
        }

        public int GetThreadIndex(int threadX, int threadY) => threadY * threadsX + threadX;
        public (int threadX, int threadY) GetThreadPosition(int threadIndex) => (threadIndex % threadsX, threadIndex / threadsX);
        public int GetThreadCount() => threadsX * threadsY;
    }
}
