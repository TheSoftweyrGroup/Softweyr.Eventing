namespace Softweyr.Eventing
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.Serialization;

    [DataContract]
    public abstract class BaseEvent : IDisposable
    {
        private readonly List<IEventSubscription> _subscriptions = new List<IEventSubscription>();

        protected ICollection<IEventSubscription> Subscriptions
        {
            [DebuggerStepThrough]
            get { return this._subscriptions; }
        }

        protected virtual SubscriptionToken Subscribe(IEventSubscription eventSubscription)
        {
            eventSubscription.SubscriptionToken = new SubscriptionToken();

            lock (this._subscriptions)
            {
                this._subscriptions.Add(eventSubscription);
            }

            return eventSubscription.SubscriptionToken;
        }

        protected virtual void Publish(params object[] arguments)
        {
            var executionStrategies = this.PruneAndReturnStrategies();

            foreach (var executionStrategy in executionStrategies)
            {
                executionStrategy(arguments);
            }
        }

        public virtual void Unsubscribe(SubscriptionToken token)
        {
            lock (this._subscriptions)
            {
                IEventSubscription subscription = this._subscriptions.FirstOrDefault
                    (evt => evt.SubscriptionToken == token);

                if (subscription != null)
                {
                    this._subscriptions.Remove(subscription);
                }
            }
        }

        public virtual bool Contains(SubscriptionToken token)
        {
            lock (this._subscriptions)
            {
                IEventSubscription subscription = this._subscriptions.FirstOrDefault
                    (evt => evt.SubscriptionToken == token);

                return (subscription != null);
            }
        }

        private List<Action<object[]>> PruneAndReturnStrategies()
        {
            var returnList = new List<Action<object[]>>();

            // TODO: Use Upgradeable Read instead?
            lock (this._subscriptions)
            {
                for (int i = this._subscriptions.Count - 1; i >= 0; i--)
                {
                    Action<object[]> subscriptionAction = this._subscriptions[i].GetExecutionStrategy();

                    if (subscriptionAction == null)
                    {
                        this._subscriptions.RemoveAt(i);
                    }
                    else
                    {
                        returnList.Add(subscriptionAction);
                    }
                }
            }

            return returnList;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            lock (this._subscriptions)
            {
                this._subscriptions.Clear();
            }
        }
    }
}