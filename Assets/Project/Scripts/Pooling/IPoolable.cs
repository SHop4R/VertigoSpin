namespace VertigoSpin.Project.Scripts.Pooling
{
    public interface IPoolable
    {
        void OnSpawn();
        void OnReturn();
    }
}
