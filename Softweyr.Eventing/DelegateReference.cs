namespace Softweyr.Eventing
{
    using System;
    using System.Diagnostics;
    using System.Reflection;

    public class DelegateReference : IDelegateReference
    {
        private readonly Delegate _delegate;

        private readonly Type _delegateType;

        private readonly MethodInfo _method;

        private readonly WeakReference _weakReference;

        public DelegateReference(Delegate @delegate, bool keepReferenceAlive)
        {
            // Check.Argument.IsNotNull(@delegate, "@delegate");

            if (keepReferenceAlive)
            {
                this._delegate = @delegate;
            }
            else
            {
                this._weakReference = new WeakReference(@delegate.Target);
                this._method = @delegate.Method;
                this._delegateType = @delegate.GetType();
            }
        }

        #region IDelegateReference Members

        public Delegate Target
        {
            [DebuggerStepThrough]
            get { return this._delegate ?? this.TryGetDelegate(); }
        }

        #endregion

        private Delegate TryGetDelegate()
        {
            if (this._method.IsStatic)
            {
                return Delegate.CreateDelegate(this._delegateType, null, this._method);
            }

            object target = this._weakReference.Target;

            return (target != null) ? Delegate.CreateDelegate(this._delegateType, target, this._method) : null;
        }
    }
}