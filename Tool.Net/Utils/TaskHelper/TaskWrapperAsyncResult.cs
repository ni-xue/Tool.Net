using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tool.Utils.TaskHelper
{

    internal sealed class TaskWrapperAsyncResult : IAsyncResult
    {
        internal TaskWrapperAsyncResult(Task task, object asyncState)
        {
            this.Task = task;
            this.AsyncState = asyncState;
        }

        public object AsyncState
        {
            get;
            private set;
        }

        public WaitHandle AsyncWaitHandle
        {
            get
            {
                return ((IAsyncResult)this.Task).AsyncWaitHandle;
            }
        }

        public bool CompletedSynchronously
        {
            get
            {
                return this._forceCompletedSynchronously || ((IAsyncResult)this.Task).CompletedSynchronously;
            }
        }

        public bool IsCompleted
        {
            get
            {
                return ((IAsyncResult)this.Task).IsCompleted;
            }
        }

        internal Task Task
        {
            get;
            private set;
        }

        internal void ForceCompletedSynchronously()
        {
            this._forceCompletedSynchronously = true;
        }

        private bool _forceCompletedSynchronously;
    }
}
