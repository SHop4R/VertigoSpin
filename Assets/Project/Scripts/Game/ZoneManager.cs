using VertigoSpin.Project.Scripts.Data;

namespace VertigoSpin.Project.Scripts.Game
{
    public sealed class ZoneManager
    {
        private const int MaxZone = 41;
        private const int SafeZoneInterval = 5;
        private const int SuperZoneInterval = 30;

        public int CurrentZone{ get; private set; } = 1;

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

        public bool CanCollect =>
            CurrentZone > 1 && CurrentWheelType is WheelType.Silver or WheelType.Gold;

        public bool IsLastZone => CurrentZone == MaxZone;

        public int NextSafeZone
        {
            get
            {
                int next = CurrentZone + (SafeZoneInterval - CurrentZone % SafeZoneInterval) % SafeZoneInterval;
                return next == CurrentZone ? next + SafeZoneInterval : next;
            }
        }

        public int NextSuperZone
        {
            get
            {
                int next = CurrentZone + (SuperZoneInterval - CurrentZone % SuperZoneInterval) % SuperZoneInterval;
                return next == CurrentZone ? next + SuperZoneInterval : next;
            }
        }

        public void AdvanceZone()
        {
            if (CurrentZone < MaxZone)
                CurrentZone++;
        }

        public void Reset() => CurrentZone = 1;
    }
}
