namespace Compago.Service
{
    public interface ICacheService
    {
        void Set<T>(object value);

        T? Get<T>();
    }

    public class CacheService() : ICacheService
    {
        private readonly Dictionary<Type, object> cache = [];

        public void Set<T>(object value)
        {
            if (!cache.TryAdd(typeof(T), value))
            {
                cache[typeof(T)] = value;
            }
        }

        public T? Get<T>()
        {
            if (!cache.ContainsKey(typeof(T)))
            {
                return default;
            }

            return (T?)cache[typeof(T?)];
        }
    }
}
