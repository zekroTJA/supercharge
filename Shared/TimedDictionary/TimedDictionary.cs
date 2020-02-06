using System;
using System.Collections.Generic;
using System.Timers;

namespace Shared.TimedDictionary
{
    public class TimedMapValue<T>
    {
        public T Value { get; private set; }
        public DateTime Expires { get; private set; }

        public TimedMapValue(T value, DateTime expires)
        {
            Value = value;
            Expires = expires;
        }

        public bool IsExpired() =>
            DateTime.Now >= Expires;
    }

    public class TimedDictionary<K, V>
    {
        private readonly Dictionary<K, TimedMapValue<V>> values = new Dictionary<K, TimedMapValue<V>>();
        private readonly Timer cleanupTimer;

        public TimedDictionary(TimeSpan cleanupInterval)
        {
            cleanupTimer = new Timer(cleanupInterval.TotalMilliseconds);
            cleanupTimer.Elapsed += OnCleanup;
            cleanupTimer.Start();
        }

        public TimedMapValue<V> Set(K key, V value, DateTime expires)
        {
            var val = new TimedMapValue<V>(value, expires);
            values[key] = val;

            return val;
        }

        public TimedMapValue<V> Set(K key, V value, TimeSpan expiresIn) =>
            Set(key, value, DateTime.Now.Add(expiresIn));

        public TimedMapValue<V> GetRaw(K key)
        {
            if (!values.ContainsKey(key))
                return null;

            var val = values[key];

            if (val.IsExpired())
            {
                Remove(key);
                return null;
            }

            return val;
        }

        public V Get(K key) 
        {
            var val = GetRaw(key);
            return val != null ? val.Value : default;
        }

        public void Remove(K key)
        {
            if (values.ContainsKey(key))
                values.Remove(key);
        }

        public bool ContainsKey(K key)
        {
            if (!values.ContainsKey(key))
                return false;

            var val = values[key];

            return !val.IsExpired();
        }

        private void OnCleanup(object source, ElapsedEventArgs e)
        {
            foreach (var kv in values)
            {
                if (kv.Value.IsExpired())
                    Remove(kv.Key);
            }
        }
    }
}
