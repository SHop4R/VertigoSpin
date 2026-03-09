using System;
using UnityEngine;

namespace VertigoSpin.Project.Scripts.Pooling
{
    [Serializable]
    public struct PoolStats<T> where T : Component
    {
        [field: SerializeField] public T Prefab{ get; private set; }
        [field: SerializeField] public int DefaultPoolSize{ get; private set; }
        [field: SerializeField] public int MaxPoolSize{ get; private set; }
        [field: SerializeField] public bool PreGenerate{ get; private set; }
    }
}
