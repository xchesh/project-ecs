using System;

namespace Features.Lockstep
{
    public interface ILockstepLifecycle : IDisposable
    {
        const string TAG = "LockstepLifecycle";

        const float STEP_DURATION = 0.03f; // 30ms
        const float STEP_DURATION_MIN = 0.015f; // 15ms
        const long STEP_DURATION_TICKS = 300000; // 30ms

        event Action Play;
        event Action Stop;
        event Action Pause;
        event Action Resume;

        event Action<uint> Warp;
        event Action<uint> Forward;
        event Action<uint> Backward;

        event Action<uint> Step;
        event Action<uint> StepLate;

        uint StepCurrent { get; }
        uint StepNext { get; }

        bool IsPlaying { get; }
        bool IsPaused { get; }

        void OnPlay();
        void OnStop();

        void OnPause();
        void OnResume();

        uint GetNextStep(long ticks);
        uint CalculateSteps(long ticks);

        void OnWarp(uint step);
        void OnRewind(uint step);
        void OnRewindLerp(uint step);
    }
}
