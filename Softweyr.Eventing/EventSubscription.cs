namespace Softweyr.Eventing
{
    using System;
    using System.Diagnostics;

    public class EventSubscription<TPayload> : IEventSubscription
    {
        private readonly IDelegateReference _actionReference;

        private readonly IDelegateReference _filterReference;

        public EventSubscription(IDelegateReference actionReference, IDelegateReference filterReference)
        {
            /*
            Check.Argument.IsNotNull(actionReference, "actionReference");
            Check.Argument.IsNotNull(filterReference, "filterReference"); */

            if (!(actionReference.Target is Action<TPayload>))
            {
                throw new ArgumentException("Invalid delegate rerefence type.", "actionReference");
            }

            if (!(filterReference.Target is Predicate<TPayload>))
            {
                throw new ArgumentException("Invalid delegate rerefence type.", "filterReference");
            }

            this._actionReference = actionReference;
            this._filterReference = filterReference;
        }

        public Action<TPayload> Action
        {
            [DebuggerStepThrough]
            get { return (Action<TPayload>) this._actionReference.Target; }
        }

        public Predicate<TPayload> Filter
        {
            [DebuggerStepThrough]
            get { return (Predicate<TPayload>) this._filterReference.Target; }
        }

        #region IEventSubscription Members

        public SubscriptionToken SubscriptionToken { get; set; }

        public virtual Action<object[]> GetExecutionStrategy()
        {
            Action<TPayload> action = this.Action;
            Predicate<TPayload> filter = this.Filter;

            if (action != null &&
                filter != null)
            {
                return arguments =>
                           {
                               TPayload argument = default(TPayload);

                               if (arguments != null && arguments.Length > 0 &&
                                   arguments[0] != null)
                               {
                                   argument = (TPayload) arguments[0];
                               }

                               if (filter(argument))
                               {
                                   this.InvokeAction(action, argument);
                               }
                           };
            }

            return null;
        }

        #endregion

        [DebuggerStepThrough]
        protected virtual void InvokeAction(Action<TPayload> action, TPayload argument)
        {
            action(argument);
        }
    }
}