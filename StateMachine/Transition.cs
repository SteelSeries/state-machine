using System;

namespace StateMachine
{
    public partial class StateMachine<TState, TTrigger>
    {
        public class Transition
        {
            public Transition(
                TState source, 
                TState destination, 
                TTrigger trigger,
                Func<bool> guardCondition,
                Action action,
                bool isInternal
            )
            {
                Source = source;
                Destination = destination;
                Trigger = trigger;
                IsInternal = isInternal;
                Action = action;
                GuardCondition = guardCondition;
                if (IsInternal == true && IsReentry == false)
                    throw new Exception("A transition cannot be internal is its source and destination are not the same!");
            }
            
            public TState Source { get; }
            
            public TState Destination { get; }
            
            public TTrigger Trigger { get; }

            public bool IsInternal { get; }

            public Func<bool> GuardCondition { get; }

            public Action Action { get; }
            
            public bool IsReentry => Source.Equals(Destination);
        }
    }
}