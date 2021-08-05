A simple conventional state machine, inspired by Scott Hanselman's [stateless](https://github.com/dotnet-state-machine/stateless).

```csharp
var machine = new StateMachine<State, char>(State.Off);
machine.Configure(State.Off)
        .OnEntry(() => { Console.WriteLine("Entering Off"); })
        .Permit(space, State.On, () => { Console.WriteLine("action 'space'"); })
        .PermitIf(a, State.On, () => true, () => { Console.WriteLine("action 'a'"); })
        .PermitIf(b, State.On, () => false, () => { Console.WriteLine("You should NOT see this string"); })
        .PermitDynamic(c, () => 
        {
            return State.Unknown;
        })
        .PermitDynamicIf(d, () => State.Unknown, () => true)
        .OnExit(() => { Console.WriteLine("Exiting Off"); });
machine.Configure(State.On)
        .Permit(space, State.Off, () => { Console.WriteLine("On to Off action"); });
machine.Configure(State.Unknown)
        .OnEntry(() => { Console.WriteLine("Entering Unknown"); })
        .InternalTransition(space, () => { Console.WriteLine("Unknown action due to internal transtion"); })
        .PermitReentry(a, () => { Console.WriteLine("Unknown action due to reentry transtion"); })
        .OnExit(() => { Console.WriteLine("Exiting Unknown"); });
```

## Features
* Generic support for states and triggers of any .NET type (numbers, strings, enums, etc.)
* Actions for transition
* Entry/exit actions for states
* Guard clauses to support conditional transitions

## Internal transitions
Sometimes a trigger does needs to be handled, but the state shouldn't change. This is an internal transition. Use `InternalTransition` for this.

## Activation 
It might be necessary to perform some code before storing the object state.
Activation should only be called once before normal operation starts, and once before state storage.

## Guard Clauses
The state machine will choose between multiple transitions based on guard clauses.
```csharp
// when trigger X is fired, the transition from state B to state A happens
sm.Configure(State.B)
    .PermitIf(Trigger.X, State.A, () => false)
    .PermitIf(Trigger.X, State.C, () => true);
```
## Reentrant States
A state can be marked reentrant so its entry and exit actions will fire even when transitioning from/to itself:


## Notes
StateMachine is single-threaded and may not be used concurrently by multiple threads.