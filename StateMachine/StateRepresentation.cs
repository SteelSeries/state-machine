using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StateMachine
{
    public partial class StateMachine<TState, TTrigger>
    {
        internal partial class StateRepresentation
        {
            readonly TState _state;

            internal ICollection<Transition> Transitions { get; } = new List<Transition>();
            internal ICollection<Action<Transition>> EntryActions { get; } = new List<Action<Transition>>();
            internal ICollection<Action<Transition>> ExitActions { get; } = new List<Action<Transition>>();
            internal ICollection<Action> ActivateActions { get; } = new List<Action>();

            public StateRepresentation(TState state)
            {
                _state = state;
            }

            public TState UnderlyingState => _state;

            #region configuration purpose
            public void AddTransition(Transition transition) => Transitions.Add(transition);

            public void AddEntryAction(Action<Transition> action) => EntryActions.Add(action);

            public void AddExitAction(Action<Transition> action) => ExitActions.Add(action);

            public void AddActivateAction(Action action) => ActivateActions.Add(action);
            #endregion

            #region behavior
            public void Activate()
            {
                foreach (var action in ActivateActions)
                    action();
            }

            public void Enter(Transition transition)
            {
                if (!transition.IsInternal)
                {
                    ExecuteEntryActions(transition);
                }
            }

            public Transition Exit(Transition transition)
            {
                if (!transition.IsInternal)
                {
                    ExecuteExitActions(transition);
                }
                return transition;
            }

            void ExecuteEntryActions(Transition transition)
            {
                foreach (var action in EntryActions)
                    action(transition);
            }

            void ExecuteExitActions(Transition transition)
            {
                foreach (var action in ExitActions)
                    action(transition);
            }

            public bool TryFindTrigger(TTrigger trigger, out Transition transition)
            {
                var result = Transitions
                    .Where((t) => t.Trigger.Equals(trigger) && t.GuardCondition())
                    .DefaultIfEmpty(null)
                    .First();
                transition = result;
                return result != null;
            }
            #endregion

            #region tests
            public IEnumerable<TTrigger> AllTriggers        => Transitions
                                                                .Select(t => t.Trigger);

            public IEnumerable<TTrigger> PermittedTriggers  => Transitions
                                                                .Where(t => t.GuardCondition())
                                                                .Select(t => t.Trigger);
            #endregion
        }
    }
}
