using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using GameSdk.Core.Loggers;
using UnityEngine.Scripting;
using Features.Lockstep;

namespace Features.Lockstep
{
    public partial class LockstepLifecycle : ILockstepLifecycle
    {
        public event Action Play;
        public event Action Stop;
        public event Action Pause;
        public event Action Resume;
        public event Action<uint> Step;
        public event Action<uint> StepLate;

        public event Action<uint> Warp;
        public event Action<uint> Forward;
        public event Action<uint> Backward;

        public uint StepCurrent { get; private set; }
        public uint StepNext { get; private set; }

        public bool IsPlaying { get; private set; }
        public bool IsPaused { get; private set; }

        private float StepDuration { get; set; }
        private CancellationTokenSource TokenSource { get; set; }

        [Preserve]
        public LockstepLifecycle()
        {
            StepDuration = ILockstepLifecycle.STEP_DURATION;
            TokenSource = new CancellationTokenSource();

            UniTaskAsyncEnumerable.EveryUpdate().Subscribe(Tick).AddTo(TokenSource.Token);
        }

        public void OnPlay()
        {
            StepCurrent = 0;
            StepNext = 1;
            IsPlaying = true;
            IsPaused = false;

            Play?.Invoke();
        }

        public void OnStop()
        {
            StepCurrent = 0;
            StepNext = 1;
            IsPlaying = false;

            Stop?.Invoke();
        }

        public void OnPause()
        {
            IsPaused = true;

            Pause?.Invoke();
        }

        public void OnResume()
        {
            IsPaused = false;

            Resume?.Invoke();
        }

        public uint GetNextStep(long ticks)
        {
            return StepCurrent + (uint)(ticks / ILockstepLifecycle.STEP_DURATION_TICKS);
        }

        public uint CalculateSteps(long ticks)
        {
            return (uint)(ticks / ILockstepLifecycle.STEP_DURATION_TICKS);
        }

        public void OnWarp(uint step)
        {
            // Rewind to previous step
            step -= 1;

            StepCurrent = step;
            StepNext = step + 1;

            Warp?.Invoke(step);
        }

        public void OnRewind(uint step)
        {
            SystemLog.Log(ILockstepLifecycle.TAG, $"Rewind {StepCurrent} -> {step}");

            OnRewind(step, 0);
        }

        public void OnRewindLerp(uint step)
        {
            OnRewind(step, ILockstepLifecycle.STEP_DURATION_MIN);
        }

        private void OnRewind(uint step, float stepDuration)
        {
            StepDuration = stepDuration;

            // Rewind to previous step
            step -= 1;

            if (step < StepCurrent)
            {
                StepCurrent = step;

                Backward?.Invoke(step);

                return;
            }

            StepNext = step + 1;

            Forward?.Invoke(step);
        }

        public void Dispose()
        {
            TokenSource?.Cancel();
            TokenSource?.Dispose();
            TokenSource = null;
        }
    }
}
