using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public abstract class BaseState
    {
        protected readonly Joystick _movementJoystick;
        protected readonly Joystick _attackJoystick;
        protected readonly IStationStateSwitcher _stateSwitcher;
        protected readonly Animator _animator;
        
        protected BaseState(Joystick attackJoystick, Joystick movementJoystick, Animator animator, IStationStateSwitcher stateSwitcher)
        {
            _movementJoystick = movementJoystick;
            _animator = animator;
            _attackJoystick = attackJoystick;
            _stateSwitcher = stateSwitcher;
        }

        public abstract void Idle(ref StationBehavior.Weapon weapon);

        public abstract void Run(Rigidbody2D transform, float moveSpeed, ref StationBehavior.Weapon weapon);

        public abstract void Attack();


    }
}