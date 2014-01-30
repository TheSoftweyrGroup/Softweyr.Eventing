namespace Softweyr.Eventing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    public class EventAggregator : IEventAggregator
    {
        private readonly List<BaseEvent> events = new List<BaseEvent>();

        private readonly ReaderWriterLockSlim rwl = new ReaderWriterLockSlim();

        #region IEventAggregator Members

        public TEventType GetEvent<TEventType>() where TEventType : BaseEvent
        {
            this.rwl.EnterUpgradeableReadLock();

            try
            {
                TEventType eventInstance =
                    this.events.SingleOrDefault(evt => evt.GetType() == typeof (TEventType)) as TEventType;

                if (eventInstance == null)
                {
                    this.rwl.EnterWriteLock();

                    try
                    {
                        eventInstance =
                            this.events.SingleOrDefault(evt => evt.GetType() == typeof (TEventType)) as TEventType;

                        if (eventInstance == null)
                        {
                            eventInstance = Activator.CreateInstance<TEventType>();
                            this.events.Add(eventInstance);
                        }
                    }
                    finally
                    {
                        this.rwl.ExitWriteLock();
                    }
                }

                return eventInstance;
            }
            finally
            {
                this.rwl.ExitUpgradeableReadLock();
            }
        }

        #endregion

        public void Raise<TPayload>(Action<TPayload> init) where TPayload : BaseEvent<TPayload>, new()
        {
            var domainEvent = GetEvent<TPayload>();
            var payload = new TPayload();
            init.Invoke(payload);
            domainEvent.Publish(payload);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.rwl.EnterUpgradeableReadLock();

            try
            {
                this.rwl.EnterWriteLock();

                try
                {
                    this.events.ForEach(evt => evt.Dispose());
                    this.events.Clear();
                }
                finally
                {
                    this.rwl.ExitWriteLock();
                }
            }
            finally
            {
                this.rwl.ExitUpgradeableReadLock();
            }
        }
    } 
}