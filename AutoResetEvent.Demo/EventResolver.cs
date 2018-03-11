using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace ResetEventAuto.Demo
{
    public class EventResolver<TKey>
    {
        public EventResolver()
        {
            eventCollection = new ConcurrentDictionary<TKey, AutoResetEvent>();
        }

        private ConcurrentDictionary<TKey, AutoResetEvent> eventCollection { get; set; }

        public Event<TKey> RequestEvent(TKey eventId, params TKey[] dependencies)
        {
            eventCollection.TryGetValue(eventId, out var autoResetEventValue);

            if(autoResetEventValue != null)
                InternalRequestNewEvent(eventId);

            foreach (var dependency in dependencies)
            {
                InternalRequestNewEvent(dependency);
            }

            return new Event<TKey>
            {
                Id = eventId,
                Dependencies = dependencies
            };
        }

        public void WaitEvent(Event<TKey> eventData, bool waitForDependencies)
        {
            if(waitForDependencies)
            {
                var eventDependencies = new List<AutoResetEvent>();

                foreach(var dependency in eventData.Dependencies)
                {
                    eventCollection.TryGetValue(dependency, out var dependencyAutoReset);
                    eventDependencies.Add(dependencyAutoReset);
                }

                WaitHandle.WaitAll(eventDependencies.ToArray());
            }

            eventCollection.TryGetValue(eventData.Id, out var autoReset);
            autoReset.WaitOne();
        }

        public void SetEvent(TKey id)
        {
            eventCollection.TryGetValue(id, out var autoReset);

            if(autoReset == null)
            {
                InternalRequestNewEvent(id);
                eventCollection.TryGetValue(id, out autoReset);
            }

            autoReset.Set();
        }

        public void SetEvent(Event<TKey> eventData, bool waitForDependencies)
        {
            if (waitForDependencies)
            {
                var eventDependencies = new List<AutoResetEvent>();

                foreach (var dependency in eventData.Dependencies)
                {
                    eventCollection.TryGetValue(dependency, out var dependencyAutoReset);
                    eventDependencies.Add(dependencyAutoReset);
                }

                WaitHandle.WaitAll(eventDependencies.ToArray());
            }

            SetEvent(eventData.Id);
        }

        private void InternalRequestNewEvent(TKey eventId)
        {
            eventCollection.TryAdd(eventId, new AutoResetEvent(false));
        }
    }
}
