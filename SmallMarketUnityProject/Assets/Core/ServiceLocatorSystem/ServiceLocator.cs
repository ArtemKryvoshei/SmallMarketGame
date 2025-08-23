using System;
using System.Collections.Generic;

namespace Core.ServiceLocatorSystem
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> _services = new();

        /// <summary>
        /// Регистрирует сервис в локаторе.
        /// </summary>
        public static void Register<T>(T service)
        {
            var type = typeof(T);
            if (_services.ContainsKey(type))
            {
                UnityEngine.Debug.LogWarning($"Service {type.Name} already registered. Rebind.");
            }
            _services[type] = service;
        }

        /// <summary>
        /// Возвращает сервис по типу.
        /// </summary>
        public static T Get<T>()
        {
            var type = typeof(T);
            if (_services.TryGetValue(type, out var service))
            {
                return (T)service;
            }

            throw new Exception($"Service {type.Name} not found. be sure it registered.");
        }

        /// <summary>
        /// Убирает сервис из локатора.
        /// </summary>
        public static void Unregister<T>()
        {
            var type = typeof(T);
            if (_services.ContainsKey(type))
            {
                _services.Remove(type);
            }
        }

        /// <summary>
        /// Полностью очищает локатор (ex: при смене сцены).
        /// </summary>
        public static void Clear()
        {
            _services.Clear();
        }
    }
}