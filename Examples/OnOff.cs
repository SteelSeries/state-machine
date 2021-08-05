using StateMachine;
using System;

namespace StateMachine.Examples
{
    class OnOff
    {
        public enum State
        {
            On,
            Off,
            Unknown
        }

        static void Main(string[] args)
        {
            const char space = ' ';
            const char a = 'a';
            const char b = 'b';
            const char c = 'c';
            const char d = 'd';

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

            Console.WriteLine("Press <space> to toggle the switch. Any other key will exit the program.");

            while (true)
            {
                Console.WriteLine("Switch is in state: " + machine.State);
                var key = Console.ReadKey(true).KeyChar;
                if (key == 's') break;
                machine.Fire(key);
            }
        }
    }
}
