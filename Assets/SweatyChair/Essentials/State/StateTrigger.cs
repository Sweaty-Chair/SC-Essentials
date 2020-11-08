using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace SweatyChair.StateManagement
{

    public class StateTrigger : EventTrigger
    {

        [SerializeField] private State _targetState = State.None;

        protected override void CallEvent()
        {
            StateManager.Set(_targetState);
        }

    }

}