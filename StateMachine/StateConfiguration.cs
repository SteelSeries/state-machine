using System;
using System.Collections.Generic;
using System.Text;

namespace StateMachine
{
    public partial class StateMachine<TState, TTrigger>
    {
        public partial class StateConfiguration
        {
            readonly StateRepresentation _representation;

            internal StateConfiguration(StateRepresentation representation)
            {
                _representation = representation;
            }

            #region Internal Transition
            public StateConfiguration InternalTransition(TTrigger trigger)
            {
                return InternalTransitionIf(trigger, () => true);
            }

            public StateConfiguration InternalTransition(TTrigger trigger, Action action)
            {
                return InternalTransitionIf(trigger, () => true, action);
            }

            public StateConfiguration InternalTransitionIf(TTrigger trigger, Func<bool> guardCondition)
            {
                _representation.AddTransition(new Transition(
                    _representation.UnderlyingState,
                    _representation.UnderlyingState,
                    trigger,
                    guardCondition,
                    () => { },
                    true
                ));
                return this;
            }

            public StateConfiguration InternalTransitionIf(TTrigger trigger, Func<bool> guardCondition, Action action)
            {
                _representation.AddTransition(new Transition(
                    _representation.UnderlyingState,
                    _representation.UnderlyingState,
                    trigger,
                    guardCondition,
                    action,
                    true
                ));
                return this;
            }
            #endregion

            #region Transition to different state
            public StateConfiguration Permit(TTrigger trigger, TState destinationState)
            {
                EnforceNotIdentityTransition(destinationState);

                PermitIf(trigger, destinationState, () => true);
                return this;
            }

            public StateConfiguration Permit(TTrigger trigger, TState destinationState, Action action)
            {
                EnforceNotIdentityTransition(destinationState);

                PermitIf(trigger, destinationState, () => true, action);
                return this;
            }

            public StateConfiguration PermitDynamic(TTrigger trigger, Func<TState> destinationStateSelector)
            {
                if (destinationStateSelector == null) throw new ArgumentNullException(nameof(destinationStateSelector));

                TState destinationState = destinationStateSelector();
                EnforceNotIdentityTransition(destinationState);
                _representation.AddTransition(new Transition(
                    _representation.UnderlyingState,
                    destinationState,
                    trigger,
                    () => true,
                    () => { },
                    false
                ));
                return this;
            }

            public StateConfiguration PermitDynamic(TTrigger trigger, Func<TState> destinationStateSelector, Action action)
            {
                if (destinationStateSelector == null) throw new ArgumentNullException(nameof(destinationStateSelector));

                TState destinationState = destinationStateSelector();
                EnforceNotIdentityTransition(destinationState);
                _representation.AddTransition(new Transition(
                    _representation.UnderlyingState,
                    destinationState,
                    trigger,
                    () => true,
                    action,
                    false
                ));
                return this;
            }

            public StateConfiguration PermitIf(TTrigger trigger, TState destinationState, Func<bool> guardCondition)
            {
                EnforceNotIdentityTransition(destinationState);
                _representation.AddTransition(new Transition(
                    _representation.UnderlyingState,
                    destinationState,
                    trigger,
                    guardCondition,
                    () => { },
                    false
                ));
                return this;
            }

            public StateConfiguration PermitIf(TTrigger trigger, TState destinationState, Func<bool> guardCondition, Action action)
            {
                EnforceNotIdentityTransition(destinationState);
                _representation.AddTransition(new Transition(
                    _representation.UnderlyingState,
                    destinationState,
                    trigger,
                    guardCondition,
                    action,
                    false
                ));
                return this;
            }

            public StateConfiguration PermitDynamicIf(TTrigger trigger, Func<TState> destinationStateSelector, Func<bool> guardCondition)
            {
                if (destinationStateSelector == null) throw new ArgumentNullException(nameof(destinationStateSelector));

                TState destinationState = destinationStateSelector();
                EnforceNotIdentityTransition(destinationState);
                _representation.AddTransition(new Transition(
                    _representation.UnderlyingState,
                    destinationState,
                    trigger,
                    guardCondition,
                    () => { },
                    false
                ));
                return this;
            }

            public StateConfiguration PermitDynamicIf(TTrigger trigger, Func<TState> destinationStateSelector, Func<bool> guardCondition, Action action)
            {
                if (destinationStateSelector == null) throw new ArgumentNullException(nameof(destinationStateSelector));

                TState destinationState = destinationStateSelector();
                EnforceNotIdentityTransition(destinationState);
                _representation.AddTransition(new Transition(
                    _representation.UnderlyingState,
                    destinationState,
                    trigger,
                    guardCondition,
                    action,
                    false
                ));
                return this;
            }
            #endregion

            #region Transition to same state
            public StateConfiguration PermitReentry(TTrigger trigger)
            {
                _representation.AddTransition(new Transition(
                    _representation.UnderlyingState,
                    _representation.UnderlyingState,
                    trigger,
                    () => true,
                    () => {},
                    false
                ));
                return this;
            }

            public StateConfiguration PermitReentry(TTrigger trigger, Action action)
            {
                _representation.AddTransition(new Transition(
                    _representation.UnderlyingState,
                    _representation.UnderlyingState,
                    trigger,
                    () => true,
                    action,
                    false
                ));
                return this;
            }

            public StateConfiguration PermitReentryIf(TTrigger trigger, Func<bool> guard)
            {
                _representation.AddTransition(new Transition(
                    _representation.UnderlyingState,
                    _representation.UnderlyingState,
                    trigger,
                    guard,
                    () => true,
                    false
                ));
                return this;
            }

            public StateConfiguration PermitReentryIf(TTrigger trigger, Action action, Func<bool> guard)
            {
                _representation.AddTransition(new Transition(
                    _representation.UnderlyingState,
                    _representation.UnderlyingState,
                    trigger,
                    guard,
                    action,
                    false
                ));
                return this;
            }
            #endregion

            #region activate, entry & exit actions
            public StateConfiguration OnActivate(Action activateAction)
            {
                if (activateAction == null) throw new ArgumentNullException(nameof(activateAction));

                _representation.AddActivateAction(activateAction);
                return this;
            }

            public StateConfiguration OnEntry(Action entryAction)
            {
                if (entryAction == null) throw new ArgumentNullException(nameof(entryAction));

                _representation.AddEntryAction((t) => entryAction());
                return this;
            }

            public StateConfiguration OnEntry(Action<Transition> entryAction)
            {
                if (entryAction == null) throw new ArgumentNullException(nameof(entryAction));

                _representation.AddEntryAction(entryAction);
                return this;
            }

            public StateConfiguration OnExit(Action exitAction)
            {
                if (exitAction == null) throw new ArgumentNullException(nameof(exitAction));

                _representation.AddExitAction((t) => exitAction());
                return this;
            }
            
            public StateConfiguration OnExit(Action<Transition> exitAction)
            {
                _representation.AddExitAction(exitAction);
                return this;
            }
            #endregion

            void EnforceNotIdentityTransition(TState destination)
            {
                if (destination.Equals(_representation.UnderlyingState))
                {
                    throw new ArgumentException("Not allowed! Please use PermitReentry");
                }
            }
        }
    }
}
