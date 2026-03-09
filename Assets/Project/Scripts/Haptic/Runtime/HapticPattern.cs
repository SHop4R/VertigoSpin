namespace VertigoSpin.Project.Scripts.Haptic.Runtime
{
    [System.Serializable]
    public class HapticPattern
    {
        public long[] Timings = { 0, 50 };
        public int[] Amplitudes = { 0, 150 };
        public int Repeat = -1;
    }
}