namespace Softweyr.Eventing
{
    using System;

    /// <summary>
    /// Utility class for eventing functionality.
    /// </summary>
    public static class Events
    {
        /// <summary>
        /// The event aggregator to use for non-unitofwork events.
        /// </summary>
        private static readonly IEventAggregator GlobalEvents = new EventAggregator();

        /// <summary>
        /// Raises the event with any subscribers of this event.
        /// </summary>
        /// <param name="init">
        /// The function to initialize the event data.
        /// </param>
        /// <typeparam name="TPayload">
        /// The event data.
        /// </typeparam>
        public static void Raise<TPayload>(Action<TPayload> init) where TPayload : BaseEvent<TPayload>, new()
        {
/*            if (EventingUnitOfWork.Exists)
            {
                EventingUnitOfWork.GetEventAggregator().Raise(init);
            } */

            GlobalEvents.Raise(init);
        }

        /// <summary>
        /// Gets the event from the global event aggregator.
        /// </summary>
        /// <typeparam name="TEventType">
        /// The type of event to get.
        /// </typeparam>
        /// <returns>
        /// The event.
        /// </returns>
        public static TEventType GetEvent<TEventType>() where TEventType : BaseEvent
        {
            return GlobalEvents.GetEvent<TEventType>();
        }
    }
}
