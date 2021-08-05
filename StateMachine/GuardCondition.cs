using System;

namespace StateMachine
{
    public partial class StateMachine<TState, TTrigger>
    {
        public class GuardCondition
        {
            internal GuardCondition(Func<bool> guard)
                : this(args => guard())
            {
            }

            internal GuardCondition(Func<object[], bool> guard)
            {
                Guard = guard ?? throw new ArgumentNullException(nameof(guard));
            }

            internal Func<object[], bool> Guard { get; }
        }
    }
}