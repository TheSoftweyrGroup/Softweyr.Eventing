namespace Softweyr.Eventing
{
    using System;

    public interface IEventAggregator : IDisposable
    {
        TEventType GetEvent<TEventType>() where TEventType : BaseEvent;
        void Raise<TPayload>(Action<TPayload> init) where TPayload : BaseEvent<TPayload>, new();
    }
}