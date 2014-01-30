namespace Softweyr.Eventing
{
    using System;
    using System.Diagnostics;

    public class SubscriptionToken : IEquatable<SubscriptionToken>
    {
        private readonly Guid token = Guid.NewGuid();

        #region IEquatable<SubscriptionToken> Members

        [DebuggerStepThrough]
        public bool Equals(SubscriptionToken other)
        {
            return (other != null) && Equals(this.token, other.token);
        }

        #endregion

        [DebuggerStepThrough]
        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || this.Equals(obj as SubscriptionToken);
        }

        [DebuggerStepThrough]
        public override int GetHashCode()
        {
            return this.token.GetHashCode();
        }

        [DebuggerStepThrough]
        public override string ToString()
        {
            return this.token.ToString();
        }
    }
}