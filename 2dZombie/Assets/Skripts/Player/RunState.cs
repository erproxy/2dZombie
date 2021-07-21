using Newtonsoft.Json.Converters;
using UnityEngine;
using UnityEngine.UIElements;

namespace Player
{
    public class RunState : BaseState
    {
        public RunState(Joystick attackJoystick, Joystick movementJoystick, Animator animator,
            IStationStateSwitcher stateSwitcher)
            : base(attackJoystick, movementJoystick, animator, stateSwitcher)
        {
        }

        public override void Idle(ref StationBehavior.Weapon weapon)
        {
            if (_movementJoystick.Horizontal==0 && _movementJoystick.Vertical == 0)
            {
                _stateSwitcher.SwitchState<IdleState>();
            }
        }

        public override void Run(Rigidbody2D rb, float moveSpeed, ref StationBehavior.Weapon weapon)
        {
            // if (weapon == StationBehavior.Weapon.KNIFE)_animator.Play("RunKnife");
            // else if (weapon==StationBehavior.Weapon.PISTOL)_animator.Play("RunPistol");
            // else if (weapon==StationBehavior.Weapon.AK47)_animator.Play("RunAk47");

           
            
            Vector2 movement;
            movement.x = _movementJoystick.Horizontal;
            movement.y = _movementJoystick.Vertical;
            
            rb.AddForce(movement * moveSpeed);
            
        }

        public override void Attack()
        {
            
        }
    }
}