using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessageBroker
{
    public interface IMessageBroker : IDisposable
    {
        void Publish<T>(T message, int channel = default);
        void Subscribe<T>(Action<T> callback, int channel = default);
        void UnSubscribe<T>(Action<T> callback, int channel = default);
        bool Any<T>(int channel = default);
        bool Any(Type type, int channel = default);
    }

    public class ChannelMessageBroker : IMessageBroker
    {
        private readonly Dictionary<int, Dictionary<Type, List<Delegate>>> channelsubscribers;

        public ChannelMessageBroker()
        {
            channelsubscribers = new Dictionary<int, Dictionary<Type, List<Delegate>>> { { default, new Dictionary<Type, List<Delegate>>() } };
        }

        private Dictionary<Type, List<Delegate>> GetSubscribersInChannelOrCreateNew(int channel)
        {
            if (!channelsubscribers.TryGetValue(channel, out var subscribers))
            {
                subscribers = new Dictionary<Type, List<Delegate>>();
                channelsubscribers.Add(channel, subscribers);
            }
            return subscribers;
        }

        public void Publish<T>(T message, int channel = default)
        {
            var invocationList = GetInvocationList(message, channel);
            invocationList.ToList().ForEach(x => x.DynamicInvoke(message));
        }

        private Delegate[] GetInvocationList<T>(T message, int channel)
        {
            if (!channelsubscribers.TryGetValue(channel, out var subscribers))
            {
                return Array.Empty<Delegate>();
            }

            var type = message.GetType();
            if (!subscribers.ContainsKey(type))
            {
                return Array.Empty<Delegate>();
            }
            var delegates = subscribers[type];
            if (delegates == null || delegates.Count == 0) return Array.Empty<Delegate>();;
            return delegates.ToArray();
        }

        public void Subscribe<T>(Action<T> subscription, int channel = default)
        {
            var subscribers = GetSubscribersInChannelOrCreateNew(channel);
            var delegates = subscribers.ContainsKey(typeof(T)) ?
                            subscribers[typeof(T)] : new List<Delegate>();
            if (!delegates.Contains(subscription))
            {
                delegates.Add(subscription);
            }
            subscribers[typeof(T)] = delegates;
        }

        public void UnSubscribe<T>(Action<T> subscription, int channel = default)
        {
            if (!channelsubscribers.TryGetValue(channel, out var subscribers))
            {
                return;
            }
            if (!subscribers.ContainsKey(typeof(T))) return;
            var delegates = subscribers[typeof(T)];
            if (delegates.Contains(subscription))
                delegates.Remove(subscription);
            if (delegates.Count == 0)
                subscribers.Remove(typeof(T));
        }

        public bool Any<T>(int channel = default)
        {
            return Any(typeof(T));
        }

        public bool Any(Type type, int channel = default)
        {
            return channelsubscribers.TryGetValue(channel, out var subscribers) && subscribers.ContainsKey(type);
        }

        public void Dispose()
        {
            channelsubscribers.Clear();
        }
    }
}