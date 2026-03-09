namespace VertigoSpin.Project.Scripts.Utils.Loading
{
    public interface ILoadingStep
    {
        float Progress{ get; }
        bool IsComplete{ get; }
        
        void Begin();
        void Update();
    }
}
