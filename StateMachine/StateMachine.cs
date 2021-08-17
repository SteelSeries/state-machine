using System.Collections.Generic;

namespace StateMachine
{
    public partial class StateMachine<TState, TTrigger>
    {
        private readonly IDictionary<TState, StateRepresentation> _stateConfiguration; 
        private readonly Queue<TTrigger> _triggerQueue;
        private bool _firing;

        public StateMachine(TState initialState)
        {
            _stateConfiguration = new Dictionary<TState, StateRepresentation>();
            _triggerQueue = new Queue<TTrigger>();
            _firing = false;
            State = initialState;
            GetRepresentation(initialState);
        }

        public TState State { get; set; } // current state
        public bool IsActivated { get; set; }

        // return an existing state if found
        // return a new state if not found
        private StateRepresentation GetRepresentation(TState state)
        {
            if (!_stateConfiguration.TryGetValue(state, out StateRepresentation result))
            {
                result = new StateRepresentation(state);
                _stateConfiguration.Add(state, result);
            }

            return result;
        }

        private StateRepresentation CurrentStateRepresentation => GetRepresentation(State);

        public StateConfiguration Configure(TState state)
        {
            return new StateConfiguration(GetRepresentation(state));
        }

        public void Activate()
        {
            if (!IsActivated)
            {
                IsActivated = true;
                GetRepresentation(State).Activate();
            }                
        }

        public void Deactivate()
        {
            IsActivated = false;
        }

        public void Fire(TTrigger trigger)
        {
            if (!IsActivated) return;
            
            _triggerQueue.Enqueue(trigger);

            if (_firing)
            {
                return;
            }

            try
            {
                _firing = true;

                // Empty queue for triggers
                while (_triggerQueue.Count > 0)
                {
                    InternalFire(_triggerQueue.Dequeue());
                }
            }
            finally
            {
                _firing = false;
            }
        }

        private void InternalFire(TTrigger trigger)
        {
            var source = GetRepresentation(State);
            if (source.TryFindTrigger(trigger, out Transition transition))
            {
                transition.Action();

                var destination = GetRepresentation(transition.Destination);
                if (!transition.IsInternal)
                {
                    source.Exit(transition);
                    destination.Enter(transition);
                }

                State = transition.Destination;
            }
        }

        #region tests
        public IEnumerable<TState> AllStates => _stateConfiguration.Keys;

        public IEnumerable<TTrigger> AllTriggersOfCurrentState => CurrentStateRepresentation.AllTriggers;

        public IEnumerable<TTrigger> PermittedTriggersOfCurrentState => CurrentStateRepresentation.PermittedTriggers;
        #endregion
    }
}
