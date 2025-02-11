using System;
using System.Threading.Tasks;
using Intech.FortnaRollerConveyor.Shared.Messages;

namespace Intech.FortnaRollerConveyor.Shared.Timeout
{
    public static class TimedExecution<T>
    {
        public static void ExecuteAction(Action action, TimeSpan timeout, Action onTimeout)
        {
            Task task = Task.Run(action);
            if (task.Wait(timeout))
            {
                // everything OK
            }
            else
            {
                onTimeout();
            }
        }

        public static T Execute(Func<T> function, TimeSpan timeout, Func<T> onTimeout)
        {
            Task<T> task = Task.Run(function);
            if (task.Wait(timeout))
            {
                // the function returned in time
                return task.Result;
            }
            else
            {
                // the function takes longer than the timeout
                return onTimeout();
            }
        }
    }
}
