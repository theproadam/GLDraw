using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace GLDraw
{
    public static class GLThread
    {
        static AutoResetEvent waitEvent = new AutoResetEvent(false);
        static Thread glThread = new Thread(ThreadWait);
        static object threadLock = new object();
        static GLThreadTask currentTask;

        public static void Invoke(Action triggerFunc)
        {
            if (Thread.CurrentThread.ManagedThreadId == glThread.ManagedThreadId)
            {
                triggerFunc();
                return;
            }

            lock (threadLock)
            {
                currentTask = new GLThreadTask(triggerFunc);
                waitEvent.Set();
                currentTask.Wait();
            }
        }

        static void ThreadWait()
        {
            while (true)
            {
                waitEvent.WaitOne();
                currentTask.delegateInvoke.Invoke();
                currentTask.Free();
            }
        }

        static GLThread()
        {
            lock (threadLock)
            {
                glThread.Start();
            }
        }

    }

    struct GLThreadTask
    {
        public AutoResetEvent callback;
        public Action delegateInvoke;

        public GLThreadTask(Action action)
        {
            callback = new AutoResetEvent(false);
            delegateInvoke = action;
        }

        public void Wait()
        {
            callback.WaitOne();
        }

        public void Free()
        {
            callback.Set();
        }
    }

}
