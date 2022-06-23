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
    }

    public class ChannelMessageBroker : IMessageBroker
    {
        private Dictionary<int, Dictionary<Type, List<Delegate>>> channelSubScribers;

        public ChannelMessageBroker()
        {
            channelSubScribers = new Dictionary<int, Dictionary<Type, List<Delegate>>>();
            channelSubScribers.Add(default, new Dictionary<Type, List<Delegate>>());
        }

        private Dictionary<Type, List<Delegate>> GetSubscribersInChannelOrCreateNew(int channel)
        {
            if (!channelSubScribers.TryGetValue(channel, out var subscribers))
            {
                subscribers = new Dictionary<Type, List<Delegate>>();
                channelSubScribers.Add(channel, subscribers);
            }
            return subscribers;
        }

        public void Publish<T>(T message, int channel = default)
        {
            if (!channelSubScribers.TryGetValue(channel, out var subscribers))
            {
                return;
            }

            if (!subscribers.ContainsKey(typeof(T)))
            {
                return;
            }
            var delegates = subscribers[typeof(T)];
            if (delegates == null || delegates.Count == 0) return;
            foreach (var handler in delegates.Select(item => item as Action<T>))
            {
                handler?.Invoke(message);
            }
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
            if (!channelSubScribers.TryGetValue(channel, out var subscribers))
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

        public void Dispose()
        {
            channelSubScribers.Clear();
        }
    }
}