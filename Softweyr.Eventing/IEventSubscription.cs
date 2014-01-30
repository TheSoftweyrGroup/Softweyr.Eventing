namespace Softweyr.Eventing
{
    using System;

    public interface IEventSubscription
    {
        SubscriptionToken SubscriptionToken { get; set; }

        Action<object[]> GetExecutionStrategy();
    }
}