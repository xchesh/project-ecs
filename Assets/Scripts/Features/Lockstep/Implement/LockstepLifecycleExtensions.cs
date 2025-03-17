using System;
using Features.Lockstep;

namespace Features.Lockstep
{
    public static class LockstepLifecycleExtensions
    {
        public static uint GetNextStep(this ILockstepLifecycle lockstepLifecycle, float seconds)
        {
            return lockstepLifecycle.GetNextStep((long)(seconds * TimeSpan.TicksPerSecond));
        }

        public static uint GetNextStep(this ILockstepLifecycle lockstepLifecycle, int milliseconds)
        {
            return lockstepLifecycle.GetNextStep(milliseconds * TimeSpan.TicksPerMillisecond);
        }

        public static uint CalculateSteps(this ILockstepLifecycle lockstepLifecycle, float seconds)
        {
            return lockstepLifecycle.CalculateSteps((long)(seconds * TimeSpan.TicksPerSecond));
        }

        public static uint CalculateSteps(this ILockstepLifecycle lockstepLifecycle, int milliseconds)
        {
            return lockstepLifecycle.CalculateSteps(milliseconds * TimeSpan.TicksPerMillisecond);
        }
    }
}
