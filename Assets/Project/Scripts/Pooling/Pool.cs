using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace VertigoSpin.Project.Scripts.Pooling
{
    public sealed class Pool<T> where T : Component
    {
        private readonly IObjectPool<T> _pool;
        private readonly T _prefab;
        private readonly Transform _poolParent;

        public Pool(PoolStats<T> stats, Transform parent = null)
        {
            if (!stats.Prefab) return;

            _prefab = stats.Prefab;
            _poolParent = parent;

            int defaultCapacity = stats.DefaultPoolSize == 0 ? 1 : stats.DefaultPoolSize;
            int maxSize = stats.MaxPoolSize <= stats.DefaultPoolSize ? defaultCapacity * 2 : stats.MaxPoolSize;

            _pool = new ObjectPool<T>(
                OnObjectCreate,
                OnObjectGet,
                OnObjectRelease,
                OnObjectDestroy,
                true,
                defaultCapacity,
                maxSize);

            if (stats.PreGenerate)
                PreGenerate(stats.DefaultPoolSize);
        }

        private T OnObjectCreate()
        {
            T instance = Object.Instantiate(_prefab);

            if (_poolParent)
                instance.transform.SetParent(_poolParent);

            return instance;
        }

        private static void OnObjectGet(T obj)
        {
            obj.gameObject.SetActive(true);

            if (obj is IPoolable poolable)
                poolable.OnSpawn();
        }

        private void OnObjectRelease(T obj)
        {
            if (obj is IPoolable poolable)
                poolable.OnReturn();

            if (_poolParent)
                obj.transform.SetParent(_poolParent);

            obj.gameObject.SetActive(false);
        }

        private static void OnObjectDestroy(T obj) => Object.Destroy(obj.gameObject);

        private void PreGenerate(int amount)
        {
            T[] generated = new T[amount];

            for (int i = 0; i < amount; i++)
            {
                generated[i] = _pool?.Get();
            }

            foreach (T obj in generated)
            {
                _pool?.Release(obj);
            }
        }

        public T Spawn() => _pool != null ? _pool.Get() : default;

        public void Return(T obj)
        {
            if (_pool != null) _pool.Release(obj);
        }
    }
}
