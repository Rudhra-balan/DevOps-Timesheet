using System;
using System.Threading;

namespace HI.DevOps.DomainCore.Helper
{
    public class TaskHelper
    {
        public static void ExecuteParallel(params Action[] tasks)
        {
            // Initialize the reset events to keep track of completed threads
            var resetEvents = new ManualResetEvent[tasks.Length];

            // Launch each method in it's own thread
            for (var i = 0; i < tasks.Length; i++)
            {
                resetEvents[i] = new ManualResetEvent(false);
                ThreadPool.QueueUserWorkItem(index =>
                {
                    var taskIndex = (int) index;

                    // Execute the method
                    tasks[taskIndex]();

                    // Tell the calling thread that we're done
                    resetEvents[taskIndex].Set();
                }, i);
            }

            // Wait for all threads to execute
            WaitHandle.WaitAll(resetEvents);
        }
    }
}