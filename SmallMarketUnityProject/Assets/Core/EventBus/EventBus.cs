using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.EventBus
{
    public class EventBus : IEventBus
    {
        private readonly Dictionary<Type, List<Delegate>> _subscribers = new();

        public void Subscribe<T>(Action<T> handler)
        {
            var type = typeof(T);
            if (!_subscribers.ContainsKey(type))
            {
                _subscribers[type] = new List<Delegate>();
            }
            _subscribers[type].Add(handler);
        }

        public void Unsubscribe<T>(Action<T> handler)
        {
            var type = typeof(T);
            if (_subscribers.ContainsKey(type))
            {
                _subscribers[type].Remove(handler);
            }
        }

        public void Publish<T>(T eventData)
        {
            var type = typeof(T);
            if (_subscribers.TryGetValue(type, out var handlers))
            {
                foreach (var handler in handlers)
                {
                    ((Action<T>)handler)?.Invoke(eventData);
                }
            }
        }
    }
}