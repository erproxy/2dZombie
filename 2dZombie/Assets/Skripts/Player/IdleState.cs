using UnityEngine;

namespace Player
{
    public class IdleState : BaseState
    {
        public IdleState(Joystick attackJoystick, Joystick movementJoystick, Animator animator,
            IStationStateSwitcher stateSwitcher) 
            : base(attackJoystick, movementJoystick, animator, stateSwitcher)
        {
            
        }

        public override void Idle(ref StationBehavior.Weapon weapon)
        {
            // if (_attackJoystick.Horizontal==0 && _attackJoystick.Vertical == 0)
            // {
            //     if (weapon == StationBehavior.Weapon.KNIFE)_animator.Play("IdleKnife");
            // }
            // if (weapon==StationBehavior.Weapon.PISTOL)_animator.Play("IdlePistol");
            // else if (weapon==StationBehavior.Weapon.AK47)_animator.Play("IdleAk47");
        }

        public override void Run(Rigidbody2D rb, float moveSpeed, ref StationBehavior.Weapon weapon)
        {
            if (_movementJoystick.Horizontal!=0 && _movementJoystick.Vertical != 0)
            {
                _stateSwitcher.SwitchState<RunState>();
            }
        }

        public override void Attack()
        {
            
        }
    }
}