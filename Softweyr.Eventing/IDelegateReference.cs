namespace Softweyr.Eventing
{
    using System;

    public interface IDelegateReference
    {
        Delegate Target { get; }
    }
}