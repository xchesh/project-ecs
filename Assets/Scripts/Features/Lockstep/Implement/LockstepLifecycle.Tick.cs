using Cysharp.Threading.Tasks;
using UnityEngine;
using Features.Lockstep;

namespace Features.Lockstep
{
    public partial class LockstepLifecycle
    {
        private float LastTickTime { get; set; }
        private float LastTickNextTime { get; set; }

        private void Tick(AsyncUnit _)
        {
            if (IsPaused || !IsPlaying)
            {
                // Don't tick if paused or not ticking
                return;
            }

            TickNext(Time.deltaTime);
            TickCurrent(Time.deltaTime);

            // Restore step duration
            if (StepDuration < ILockstepLifecycle.STEP_DURATION && StepCurrent >= (StepNext - 1))
            {
                Debug.Log($"Restore step duration: {StepDuration} < {ILockstepLifecycle.STEP_DURATION}");

                StepDuration = ILockstepLifecycle.STEP_DURATION;
                LastTickTime = 0;
            }

            if (StepCurrent >= StepNext)
            {
                Debug.Log($"StepCurrent >= StepNext: {StepCurrent} >= {StepNext}; StepDuration: {StepDuration}");

                StepNext = StepCurrent + 1;
            }
        }

        private void TickNext(float deltaTime)
        {
            LastTickNextTime += deltaTime;

            if (LastTickNextTime > ILockstepLifecycle.STEP_DURATION)
            {
                LastTickNextTime -= ILockstepLifecycle.STEP_DURATION;

                StepNext += System.Math.Max(1, (uint)(deltaTime / ILockstepLifecycle.STEP_DURATION));
            }
        }

        private void TickCurrent(float deltaTime)
        {
            LastTickTime += deltaTime;

            if (LastTickTime >= StepDuration)
            {
                var stepsCount = StepDuration < float.Epsilon ? StepNext - StepCurrent : System.Math.Max(1, (uint)(deltaTime / StepDuration));

                LastTickTime -= StepDuration;

                TickCurrent(stepsCount);
            }
        }

        private void TickCurrent(uint steps)
        {
            for (var i = 0; i < steps; i++)
            {
                StepCurrent++;

                Step?.Invoke(StepCurrent);
                StepLate?.Invoke(StepCurrent);
            }
        }
    }
}
