namespace Softweyr.Eventing
{
    using System;
    using System.Linq;
    using System.Runtime.Serialization;

    [DataContract]
    public abstract class BaseEvent<TPayload> : BaseEvent
    {
        public virtual SubscriptionToken Subscribe(Action<TPayload> action)
        {
            return this.Subscribe(action, true);
        }

        public virtual SubscriptionToken Subscribe(Action<TPayload> action, bool keepSubscriberReferenceAlive)
        {
            return this.Subscribe(action, keepSubscriberReferenceAlive, delegate { return true; });
        }

        public virtual SubscriptionToken Subscribe
            (Action<TPayload> action, bool keepSubscriberReferenceAlive, Predicate<TPayload> filter)
        {
            IDelegateReference actionReference = new DelegateReference(action, keepSubscriberReferenceAlive);
            IDelegateReference filterReference = new DelegateReference(filter, keepSubscriberReferenceAlive);

            EventSubscription<TPayload> subscription = new EventSubscription<TPayload>(actionReference, filterReference);

            return base.Subscribe(subscription);
        }

        public virtual void Publish(TPayload payload)
        {
            base.Publish(payload);
        }

        public virtual void Unsubscribe(Action<TPayload> subscriber)
        {
            lock (this.Subscriptions)
            {
                IEventSubscription eventSubscription =
                    this.Subscriptions.Cast<EventSubscription<TPayload>>().FirstOrDefault
                        (evt => evt.Action == subscriber);

                if (eventSubscription != null)
                {
                    this.Subscriptions.Remove(eventSubscription);
                }
            }
        }

        public virtual bool Contains(Action<TPayload> subscriber)
        {
            IEventSubscription eventSubscription;

            lock (this.Subscriptions)
            {
                eventSubscription = this.Subscriptions.Cast<EventSubscription<TPayload>>().FirstOrDefault
                    (evt => evt.Action == subscriber);
            }

            return (eventSubscription != null);
        }
    }
}