using StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace StateMachine.Tests
{
    public class StateMachineFixture
    {
        [Fact]
        public void InitialStateIsCurrent()
        {
            var initial = State.B;
            var sm = new StateMachine<State, Trigger>(initial);
            Assert.Equal(initial, sm.State);
        }

        [Fact]
        public void StatesConfigured()
        {
            var initial = State.B;
            var sm = new StateMachine<State, Trigger>(initial);
            sm.Configure(State.A);
            sm.Configure(State.C);

            IEnumerable<State> expected = new List<State> { State.A, State.B, State.C };
            Assert.True(Enumerable.SequenceEqual(sm.AllStates.OrderBy(t => t), expected.OrderBy(t => t)));
        }

        [Fact]
        public void ActivateActionExecuted()
        {
            var initial = State.B;
            var sm = new StateMachine<State, Trigger>(initial);
            sm.Configure(State.B)
                .Permit(Trigger.X, State.A)
                .OnActivate(() => sm.Fire(Trigger.X));
            sm.Activate();
            Assert.Equal(State.A, sm.State);
        }

        [Fact]
        public void TriggersConfigured()
        {
            var initial = State.B;
            var sm = new StateMachine<State, Trigger>(initial);
            sm.Configure(State.B)
                .Permit(Trigger.X, State.A)
                .Permit(Trigger.Y, State.C);

            IEnumerable<Trigger> expected = new List<Trigger> { Trigger.X, Trigger.Y };
            Assert.True(Enumerable.SequenceEqual(sm.AllTriggersOfCurrentState.OrderBy(t => t), expected.OrderBy(t => t)));
        }

        [Fact]
        public void TransitionActionIsExecuted()
        {
            var initial = State.B;
            var sm = new StateMachine<State, Trigger>(initial);
            int value = 0;
            sm.Configure(State.B)
                .Permit(Trigger.X, State.A, () => value = 1);
            sm.Fire(Trigger.X);
            Assert.Equal(1, value);
        }

        [Fact]
        public void PermittedTriggersRespectGuards()
        {
            var sm = new StateMachine<State, Trigger>(State.B);

            sm.Configure(State.B)
                .PermitIf(Trigger.X, State.A, () => false);

            Assert.Empty(sm.PermittedTriggersOfCurrentState);
        }

        [Fact]
        public void WhenDiscriminatedByGuard_ChoosesPermitedTransition()
        {
            var sm = new StateMachine<State, Trigger>(State.B);
            int value = 0;

            sm.Configure(State.B)
                .PermitIf(Trigger.X, State.A, () => false, () => value = 1)
                .PermitIf(Trigger.X, State.C, () => true, () => value = 2);

            sm.Fire(Trigger.X);

            Assert.Equal(State.C, sm.State);
            Assert.Equal(2, value);
        }

        [Fact]
        public void InternalTransition_CurrentStateDoNotChange()
        {
            var sm = new StateMachine<State, Trigger>(State.B);
            sm.Configure(State.B)
                .InternalTransition(Trigger.X);
            sm.Fire(Trigger.X);

            Assert.Equal(State.B, sm.State);
        }

        [Fact]
        public void InternalTransition_EntryAndExitActionsNotExecuted()
        {
            var sm = new StateMachine<State, Trigger>(State.B);
            int value = 0;
            sm.Configure(State.B)
                .OnEntry(() => value = 1)
                .InternalTransition(Trigger.X)
                .OnExit(() => value = 2);
            sm.Fire(Trigger.X);

            Assert.Equal(0, value);
        }

        [Fact]
        public void FireInStateEntryAction()
        {
            var sm = new StateMachine<State, Trigger>(State.B);
            sm.Configure(State.B)
                .Permit(Trigger.X, State.A);
            sm.Configure(State.A)
                .OnEntry(() => sm.Fire(Trigger.Y))
                .Permit(Trigger.Y, State.C);
            sm.Fire(Trigger.X);
            Assert.Equal(State.C, sm.State);
        }

        [Fact]
        public void WhenStateReentered_ExitEntryActionsExecuted()
        {
            var sm = new StateMachine<State, Trigger>(State.B);
            int value1 = 0;
            int value2 = 1;

            sm.Configure(State.B)
                .OnEntry(() => value1 = 1)
                .OnEntry(() => value2 = 2)
                .PermitReentry(Trigger.X);
            sm.Fire(Trigger.X);

            Assert.Equal(1, value1);
            Assert.Equal(2, value2);
        }
    }
}
