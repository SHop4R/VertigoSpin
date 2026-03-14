using VertigoSpin.Project.Scripts.Data;

namespace VertigoSpin.Project.Scripts.Game
{
    public sealed class ZoneManager
    {
        public const int MaxZone = 61;
        public const int SafeZoneInterval = 5;
        public const int SuperZoneInterval = 30;

        public int CurrentZone { get; private set; } = 1;

        public WheelType CurrentWheelType
        {
            get
            {
                if (CurrentZone % SuperZoneInterval == 0)
                    return WheelType.Gold;

                return CurrentZone % SafeZoneInterval == 0
                    ? WheelType.Silver
                    : WheelType.Bronze;
            }
        }

        public bool IsLastZone => CurrentZone == MaxZone;

        public void AdvanceZone()
        {
            if (CurrentZone < MaxZone)
                CurrentZone++;
        }

        public void Reset() => CurrentZone = 1;
    }
}
